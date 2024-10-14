using Raylib_cs;
using Tmp.Core.Plugins;
using Tmp.HotReload.Components;
using Tmp.Resource;
using Tmp.Resource.Components;

namespace Tmp.Core.Redot;

public readonly record struct Update(float Dt)
{
    public static implicit operator float(Update self) => self.Dt;
}

public readonly struct PreDraw;

public readonly struct Draw;

public class PluginTree(Action<Tree> initTree) : PluginWrap<App>(new PluginAnonymous<App>("tree")
{
    OnBuild = async app =>
    {
        var tree = new Tree();
        app.Shelf.Set(tree);
        tree.OnInit += _ =>
        {
            tree.DecorateRootUp(new CResources(new Resources()));
            tree.DecorateRootUp(new CHotReloadSource());
            initTree(tree);
        };
        app.OnStart += () =>
        {
            tree.Init();
        };
        app.PreUpdate += () =>
        {
            Input.Propagate(tree);
        };
        app.OnUpdate += () =>
        {
            tree.Call(new Update(Raylib.GetFrameTime()));
            tree.Call<PreDraw>();
            tree.Call<Draw>();
        };
        app.PostUpdate += () =>
        {
            tree.Update();
        };
        app.OnClose += () =>
        {
            tree.Free();
        };
    }
});