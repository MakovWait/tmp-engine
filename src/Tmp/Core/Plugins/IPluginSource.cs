namespace Tmp.Core.Plugins;

public interface IPluginSource<in T>
{
	Task AddTo(IPluginTarget<T> target);
}