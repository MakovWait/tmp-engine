namespace Tmp.HotReload;

public interface IHotReloadSource
{
    public void Add(IHotReloadTarget target);

    public void Remove(IHotReloadTarget target);
}

public class HotReloadSource : IHotReloadSource
{
    public void Add(IHotReloadTarget target)
    {
        HotReloadManager.Add(target);
    }

    public void Remove(IHotReloadTarget target)
    {
        HotReloadManager.Remove(target);
    }
}