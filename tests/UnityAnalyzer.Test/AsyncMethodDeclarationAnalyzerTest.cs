using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using UnityAnalyzers;
using Xunit;

namespace UnityAnalyzer.Test;

[TestSubject(typeof(AsyncMethodDeclarationAnalyzer))]
public class AsyncMethodDeclarationAnalyzerTest
{
    [Fact]
    public async Task ShouldReportU0001_When_AsyncVoidMethod()
    {
        const string code = @"
using System.Threading.Tasks;

namespace UnityAnalyzer.Sample;

public class Examples
{
    public async void AsyncVoidMethod()
    {
    }
}";
        var u0001Expected = CSharpAnalyzerVerifier<AsyncMethodDeclarationAnalyzer, DefaultVerifier>.Diagnostic("U0001")
            .WithSpan(8, 18, 8, 22).WithArguments("AsyncVoidMethod");

        await CSharpAnalyzerVerifier<AsyncMethodDeclarationAnalyzer, DefaultVerifier>
            .VerifyAnalyzerAsync(code, u0001Expected)
            .ConfigureAwait(true);
    }

    [Fact]
    public async Task ShouldReportU0002_When_AsyncTaskMethod()
    {
        const string code = @"
using System.Threading.Tasks;

namespace UnityAnalyzer.Sample;

public class Examples
{
    public async Task AsyncTaskMethod()
    {
        await Task.Delay(100);
    }
}";
        var expected = CSharpAnalyzerVerifier<AsyncMethodDeclarationAnalyzer, DefaultVerifier>
            .Diagnostic("U0002")
            .WithSpan(8, 18, 8, 22)
            .WithArguments("AsyncTaskMethod");

        await CSharpAnalyzerVerifier<AsyncMethodDeclarationAnalyzer, DefaultVerifier>
            .VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task ShouldReportU0003_When_AsyncGenericTaskMethod()
    {
        const string code = @"
using System.Threading.Tasks;

namespace UnityAnalyzer.Sample;

public class Examples
{
    public async Task<int> AsyncGenericTaskMethod()
    {
        await Task.Delay(100);
        return 42;
    }
}";
        var expected = CSharpAnalyzerVerifier<AsyncMethodDeclarationAnalyzer, DefaultVerifier>
            .Diagnostic("U0003")
            .WithSpan(8, 18, 8, 27)
            .WithArguments("AsyncGenericTaskMethod");

        await CSharpAnalyzerVerifier<AsyncMethodDeclarationAnalyzer, DefaultVerifier>
            .VerifyAnalyzerAsync(code, expected);
    }
}