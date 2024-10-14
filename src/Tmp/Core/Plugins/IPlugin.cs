namespace Tmp.Core.Plugins;

public interface IPlugin<in T>
{
	string Name { get; }

	Task Build(T target);

	Task Finish(T target);

	Task Cleanup(T target);
}

public class PluginAnonymous<T>(string name) : IPlugin<T>, IPluginSource<T>
{
	public Func<T, Task>? OnBuild { get; init; }
	public Func<T, Task>? OnFinish { get; init; }
	public Func<T, Task>? OnCleanup { get; init; }

	public string Name { get; } = name;

	public Task Build(T target)
	{
		return OnBuild?.Invoke(target) ?? Task.CompletedTask;
	}

	public Task Finish(T target)
	{
		return OnFinish?.Invoke(target) ?? Task.CompletedTask;
	}

	public Task Cleanup(T target)
	{
		return OnCleanup?.Invoke(target) ?? Task.CompletedTask;
	}

	public Task AddTo(IPluginTarget<T> target)
	{
		return target.Add(this);
	}
}

public abstract class PluginBase<T> : IPlugin<T>, IPluginSource<T>
{
	public virtual string Name => GetType().FullName!;

	public virtual Task Build(T target)
	{
		return Task.CompletedTask;
	}

	public virtual Task Finish(T target)
	{
		return Task.CompletedTask;
	}

	public virtual Task Cleanup(T target)
	{
		return Task.CompletedTask;
	}

	public Task AddTo(IPluginTarget<T> target)
	{
		return target.Add(this);
	}
}

public class PluginWrap<T>(IPlugin<T> origin) : IPlugin<T>, IPluginSource<T>
{
	public string Name => origin.Name;
	
	public Task Build(T target)
	{
		return origin.Build(target);
	}

	public Task Finish(T target)
	{
		return origin.Finish(target);
	}

	public Task Cleanup(T target)
	{
		return origin.Cleanup(target);
	}

	public Task AddTo(IPluginTarget<T> target)
	{
		return target.Add(origin);
	}
}