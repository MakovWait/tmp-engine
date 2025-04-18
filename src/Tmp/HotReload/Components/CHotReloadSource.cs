using Tmp.Core.Comp;

namespace Tmp.HotReload.Components;

public class CHotReloadSource() : ComponentFunc((self, children) =>
{
    var hotReloadSource = new HotReloadSource();
    self.CreateContext<IHotReloadSource>(hotReloadSource);

    var target = new HotReloadTarget(self);
    hotReloadSource.Add(target);
    self.OnLateCleanup(() => hotReloadSource.Remove(target));

    return children;
})
{
    private class HotReloadTarget(INodeInit self) : IHotReloadTarget
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
