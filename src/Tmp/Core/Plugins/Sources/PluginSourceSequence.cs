namespace Tmp.Core.Plugins.Sources;

public class PluginSourceSequence<T>(IEnumerable<IPluginSource<T>> sources) : IPluginSource<T>
{
    public async Task AddTo(IPluginTarget<T> target)
    {
        foreach (var source in sources)
        {
            await source.AddTo(target);
        }
    }
}