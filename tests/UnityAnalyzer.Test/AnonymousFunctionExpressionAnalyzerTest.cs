using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using UnityAnalyzers;
using Xunit;

namespace UnityAnalyzer.Test;

[TestSubject(typeof(AnonymousFunctionExpressionAnalyzer))]
public class AnonymousFunctionExpressionAnalyzerTest
{
    [Fact]
    public async Task ShouldReportU0004_When_AnonymousFunctionExpressions1()
    {
        const string code = @"
using System;
using System.Threading.Tasks;

namespace UnityAnalyzer.Sample;

public class Examples
{
    public void AnonymousFunctionExpressions()
    {
        var f1 = async () => { };
    }
}";
        var expected =
            CSharpAnalyzerVerifier<AnonymousFunctionExpressionAnalyzer, DefaultVerifier>.Diagnostic("U0004")
                .WithSpan(11, 18, 11, 33);
        await CSharpAnalyzerVerifier<AnonymousFunctionExpressionAnalyzer, DefaultVerifier>
            .VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task ShouldReportU0004_When_AnonymousFunctionExpressions2()
    {
        const string code = @"
using System;
using System.Threading.Tasks;

namespace UnityAnalyzer.Sample;

public class Examples
{
    public void AnonymousFunctionExpressions()
    {
        Action f2 = async () => { };
    }
}";
        var expected =
            CSharpAnalyzerVerifier<AnonymousFunctionExpressionAnalyzer, DefaultVerifier>.Diagnostic("U0004")
                .WithSpan(11, 21, 11, 36);
        await CSharpAnalyzerVerifier<AnonymousFunctionExpressionAnalyzer, DefaultVerifier>
            .VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task ShouldReportU0004_When_AnonymousFunctionExpressions3()
    {
        const string code = @"
using System;
using System.Threading.Tasks;

namespace UnityAnalyzer.Sample;

public class Examples
{
    public void AnonymousFunctionExpressions()
    {
        Func<Task> func = async delegate { };
    }
}";
        var expected =
            CSharpAnalyzerVerifier<AnonymousFunctionExpressionAnalyzer, DefaultVerifier>.Diagnostic("U0004")
                .WithSpan(11, 27, 11, 45);
        await CSharpAnalyzerVerifier<AnonymousFunctionExpressionAnalyzer, DefaultVerifier>
            .VerifyAnalyzerAsync(code, expected);
    }
}