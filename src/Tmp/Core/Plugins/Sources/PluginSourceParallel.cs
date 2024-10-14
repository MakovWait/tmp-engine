namespace Tmp.Core.Plugins.Sources;

public class PluginSourceParallel<T>(IEnumerable<IPluginSource<T>> sources) : IPluginSource<T>
{
    public async Task AddTo(IPluginTarget<T> target)
    {
        await Task.WhenAll(sources.Select(x => x.AddTo(target)));
    }
}