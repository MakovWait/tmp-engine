using Tmp.Core.Plugins;
using Tmp.HotReload.Components;
using Tmp.Asset;
using Tmp.Asset.Components;

namespace Tmp.Core.Redot;

public class PluginTree(Action<App> initTree) : PluginWrap<App>(new PluginAnonymous<App>("tree")
{
    OnBuild = app =>
    {
        app.DecorateRootUp(new CAssets(new Assets()));
        app.DecorateRootUp(new CHotReloadSource());
        initTree(app);
    },
});