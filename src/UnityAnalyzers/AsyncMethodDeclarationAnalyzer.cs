using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnityAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AsyncMethodDeclarationAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.RuleU0001, DiagnosticDescriptors.RuleU0002,
            DiagnosticDescriptors.RuleU0003);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeMethodDeclarationSyntaxNode, SyntaxKind.MethodDeclaration);
    }

    private void AnalyzeMethodDeclarationSyntaxNode(SyntaxNodeAnalysisContext context)
    {
        var methodDeclarationSyntax = (MethodDeclarationSyntax)context.Node;
        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax, context.CancellationToken);

        if (methodSymbol == null) return;

        if (methodSymbol is { IsAsync: true, ReturnsVoid: true })
        {
            var u0001Diagnostic = Diagnostic.Create(DiagnosticDescriptors.RuleU0001,
                methodDeclarationSyntax.ReturnType.GetLocation(),
                methodDeclarationSyntax.Identifier);
            context.ReportDiagnostic(u0001Diagnostic);
            return;
        }

        var returnType = methodSymbol.ReturnType;

        var taskSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");
        if (SymbolEqualityComparer.Default.Equals(returnType, taskSymbol))
        {
            var u0002Diagnostic = Diagnostic.Create(DiagnosticDescriptors.RuleU0002,
                methodDeclarationSyntax.ReturnType.GetLocation(),
                methodDeclarationSyntax.Identifier
            );
            context.ReportDiagnostic(u0002Diagnostic);
            return;
        }

        var originalDefine = returnType.OriginalDefinition;
        var genericTaskSymbol =
            context.SemanticModel.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");
        if (SymbolEqualityComparer.Default.Equals(originalDefine, genericTaskSymbol))
        {
            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.RuleU0003,
                methodDeclarationSyntax.ReturnType.GetLocation(),
                methodDeclarationSyntax.Identifier
            );
            context.ReportDiagnostic(diagnostic);
        }
    }
}