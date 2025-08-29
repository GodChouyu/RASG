using System;
using System.Threading.Tasks;

namespace UnityAnalyzer.Sample;

public class Examples
{
    public async void AsyncVoidMethod()
    {
        await Task.CompletedTask;
    }

    public Task TaskMethod()
    {
        return Task.CompletedTask;
    }

    public Task<int> GenericTaskMethod()
    {
        return Task.FromResult(42);
    }

    public void AnonymousFunctionExpressions()
    {
        var f1 = async () => { }; // returns Task
        Action f2 = async () => { }; // returns void but convert to task because of await
        Func<Task> func = async delegate { };
    }
}