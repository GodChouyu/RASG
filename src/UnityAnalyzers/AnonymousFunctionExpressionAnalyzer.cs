using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnityAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AnonymousFunctionExpressionAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.RuleU0004);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeLambdaExpressionSyntaxNode,
            SyntaxKind.AnonymousMethodExpression,
            SyntaxKind.SimpleLambdaExpression,
            SyntaxKind.ParenthesizedLambdaExpression);
    }

    private void AnalyzeLambdaExpressionSyntaxNode(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not AnonymousFunctionExpressionSyntax anonymousFunctionExpressionSyntax) return;
        if (anonymousFunctionExpressionSyntax.AsyncKeyword == default) return;
        var typeInfo =
            context.SemanticModel.GetTypeInfo(anonymousFunctionExpressionSyntax, context.CancellationToken);
        var returnType = (typeInfo.ConvertedType as INamedTypeSymbol)?.DelegateInvokeMethod?.ReturnType;
        if (returnType == null) return;

        if (returnType.SpecialType == SpecialType.System_Void)
        {
            var u0004Diagnostic = Diagnostic.Create(DiagnosticDescriptors.RuleU0004,
                anonymousFunctionExpressionSyntax.GetLocation()
            );
            context.ReportDiagnostic(u0004Diagnostic);
        }
        else
        {
            var taskSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");
            if (!SymbolEqualityComparer.Default.Equals(returnType, taskSymbol)) return;
            var u0004Diagnostic = Diagnostic.Create(DiagnosticDescriptors.RuleU0004,
                anonymousFunctionExpressionSyntax.GetLocation()
            );
            context.ReportDiagnostic(u0004Diagnostic);
        }
    }
}