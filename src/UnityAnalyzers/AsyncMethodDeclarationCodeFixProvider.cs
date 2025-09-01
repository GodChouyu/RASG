using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnityAnalyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AsyncMethodDeclarationCodeFixProvider)), Shared]
public class AsyncMethodDeclarationCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticDescriptors.RuleU0001.Id);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.Single();
        var diagnosticSpan = diagnostic.Location.SourceSpan;
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnosticNode = root?.FindNode(diagnosticSpan);
        var methodDeclarationSyntax = diagnosticNode?.FirstAncestorOrSelf<MethodDeclarationSyntax>();
        if (methodDeclarationSyntax == null) return;

        context.RegisterCodeFix(
            CodeAction.Create(
                string.Format(Resources.U0001CodeFixTitle, methodDeclarationSyntax.Identifier.Text),
                token => WrapAsyncVoidToVoidAndAsyncUniTaskVoid(context.Document, methodDeclarationSyntax, token)),
            diagnostic);
    }

    private async Task<Document> WrapAsyncVoidToVoidAndAsyncUniTaskVoid(Document document,
        MethodDeclarationSyntax methodDeclarationSyntax, CancellationToken cancellationToken)
    {
        var methodName = methodDeclarationSyntax.Identifier.Text;
        var uniTaskVoidAsyncName = methodName + "Async";

        var args = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(
            methodDeclarationSyntax.ParameterList.Parameters.Select(p =>
                SyntaxFactory.Argument(SyntaxFactory.IdentifierName(p.Identifier)))));

        var wrapperBody = SyntaxFactory.Block(
            SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(uniTaskVoidAsyncName), args),
                        SyntaxFactory.IdentifierName("Forget")
                    )
                )
            )
        );

        var voidMethod = methodDeclarationSyntax
            .WithModifiers(SyntaxFactory.TokenList(
                methodDeclarationSyntax.Modifiers.Where(token => !token.IsKind(SyntaxKind.AsyncKeyword))))
            .WithReturnType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)))
            .WithIdentifier(methodDeclarationSyntax.Identifier)
            .WithBody(wrapperBody);

        var uniTaskVoidAsyncMethod = methodDeclarationSyntax
            .WithModifiers(SyntaxFactory.TokenList(methodDeclarationSyntax.Modifiers))
            .WithIdentifier(SyntaxFactory.Identifier(uniTaskVoidAsyncName))
            .WithReturnType(SyntaxFactory.ParseTypeName("UniTaskVoid"))
            .WithBody(methodDeclarationSyntax.Body);

        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null) return document;
        // todo 将光标置于wrapperMethod上
        var newRoot = root.ReplaceNode(methodDeclarationSyntax, [voidMethod, uniTaskVoidAsyncMethod]);
        return document.WithSyntaxRoot(newRoot);
    }
}