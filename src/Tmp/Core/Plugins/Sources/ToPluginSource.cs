using Tmp.Core.Plugins;

namespace Modules.Sources;

public class ToPluginSource<T>(IPlugin<T> module) : IPluginSource<T>
{
	public Task AddTo(IPluginTarget<T> target)
	{
		return target.Add(module);
	}
}

public static class ToPluginSourceEx
{
	public static ToPluginSource<T> ToPluginSource<T>(this IPlugin<T> self)
	{
		return new ToPluginSource<T>(self);
	}
}