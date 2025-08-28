using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AsyncMethodAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
    [
        DiagnosticDescriptors.RuleU0001, DiagnosticDescriptors.RuleU0002, DiagnosticDescriptors.RuleU0003
    ];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.MethodDeclaration);
    }

    private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
    {
        var methodDeclarationSyntax = (MethodDeclarationSyntax)context.Node;
        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax, context.CancellationToken);

        if (methodSymbol == null) return;

        if (methodSymbol is { IsAsync: true, ReturnsVoid: true })
        {
            var asyncLocations = methodDeclarationSyntax.Modifiers.Where(m => m.IsKind(SyntaxKind.AsyncKeyword))
                .Select(m => m.GetLocation()).ToList();
            var u0001Diagnostic = Diagnostic.Create(DiagnosticDescriptors.RuleU0001,
                methodDeclarationSyntax.ReturnType.GetLocation(),
                asyncLocations,
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