using Tmp.HotReload;

[assembly: System.Reflection.Metadata.MetadataUpdateHandler(typeof(HotReloadManager))]

namespace Tmp.HotReload;

internal static class HotReloadManager
{
    private static readonly List<IHotReloadTarget> _targets = [];

    public static void Add(IHotReloadTarget target)
    {
        _targets.Add(target);
    }

    public static void Remove(IHotReloadTarget target)
    {
        _targets.Remove(target);
    }
    
    public static void ClearCache(Type[]? types)
    {
        foreach (var hotReloadTarget in _targets)
        {
            hotReloadTarget.ClearCache(types);
        }
    }

    public static void UpdateApplication(Type[]? types)
    {
        foreach (var hotReloadTarget in _targets)
        {
            hotReloadTarget.UpdateApplication(types);
        }
    }
}