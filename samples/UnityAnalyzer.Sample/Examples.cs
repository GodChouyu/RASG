using System.Threading.Tasks;

namespace UnityAnalyzer.Sample;

public class Examples
{
    public async void AsyncVoidMethod()
    {
    }

    public async Task TaskMethod()
    {
    }

    public async Task<int> GenericTaskMethod()
    {
        return 42;
    }
}