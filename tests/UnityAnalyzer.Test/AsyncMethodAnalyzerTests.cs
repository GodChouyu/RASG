using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using UnityAnalyzers;
using Xunit;

namespace UnityAnalyzer.Test;

public class AsyncMethodAnalyzerTests
{
    private const string Text = @"
using System.Threading.Tasks;

namespace UnityAnalyzer.Sample;

public class Examples
{
    public async void AsyncVoidMethod()
    {
    }
}";

    [Fact]
    public async Task AsyncVoidMethod_AlertDiagnostic()
    {
        var expected = CSharpAnalyzerVerifier<AsyncMethodAnalyzer, DefaultVerifier>.Diagnostic("U0001")
            .WithLocation(8, 18);
        await CSharpAnalyzerVerifier<AsyncMethodAnalyzer, DefaultVerifier>.VerifyAnalyzerAsync(Text, expected)
            .ConfigureAwait(true);
    }
}