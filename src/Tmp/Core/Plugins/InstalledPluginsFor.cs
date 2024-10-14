using System.Diagnostics;

namespace Tmp.Core.Plugins;

public enum State
{
	Installing,
	Finishing,
	Installed
}

public sealed class InstalledPluginsFor<T>(T target) : IPluginTarget<T>
{
	private readonly List<IPlugin<T>> _plugins = [];
	private State state = State.Installing;

	public bool Sealed => state != State.Installing;

	public async Task Add(IPlugin<T> plugin)
	{
		Debug.Assert(!Sealed);
		_plugins.Add(plugin);
		await plugin.Build(target);
	}

	public async Task Finish()
	{
		Debug.Assert(!Sealed);
		state = State.Finishing;

		await Task.WhenAll(_plugins.Select(x => x.Finish(target)));
		await Task.WhenAll(_plugins.Select(x => x.Cleanup(target)));

		state = State.Installed;
	}
}
