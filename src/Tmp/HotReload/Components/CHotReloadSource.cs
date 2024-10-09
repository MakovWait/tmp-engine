using Tmp.Core.Redot;

namespace Tmp.HotReload.Components;

public class CHotReloadSource() : Component(self =>
{
    var hotReloadSource = new HotReloadSource();
    self.SetSingleton<IHotReloadSource>(hotReloadSource);

    var target = new HotReloadTarget(self);
    self.UseEffect(() =>
    {
        hotReloadSource.Add(target);
        return () =>
        {
            hotReloadSource.Remove(target);
        };
    }, []);
})
{
    private class HotReloadTarget(Self self) : IHotReloadTarget
    {
        public void ClearCache(Type[]? types)
        {
            self.Call<HotReloadClearCache>();
        }

        public void UpdateApplication(Type[]? types)
        {
            self.Call<HotReloadUpdateApplication>();
        }
    }
}

public readonly record struct HotReloadClearCache(Type[]? Types);

public readonly record struct HotReloadUpdateApplication(Type[]? Types);
