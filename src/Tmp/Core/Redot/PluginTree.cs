using Tmp.Core.Plugins;
using Tmp.HotReload.Components;
using Tmp.Resource;
using Tmp.Resource.Components;

namespace Tmp.Core.Redot;

public class PluginTree(Action<App> initTree) : PluginWrap<App>(new PluginAnonymous<App>("tree")
{
    OnBuild = app =>
    {
        app.DecorateRootUp(new CResources(new Resources()));
        app.DecorateRootUp(new CHotReloadSource());
        initTree(app);
    },
});