using Raylib_cs;
using Tmp.Core.Plugins;
using Tmp.Core.Shelf;
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
    OnBuild = app =>
    {
        var tree = new Tree();
        app.Shelf.Set(tree);
        tree.OnInit += _ =>
        {
            tree.DecorateRootUp(new CResources(new Resources()));
            tree.DecorateRootUp(new CHotReloadSource());
        };
        app.OnStart += () =>
        {
            tree.Init();
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
    },
    
    OnCleanup = app =>
    {
        app.Shelf.Inspect<Tree>(tree =>
        {
            tree.OnInit += _ =>
            {
                initTree(tree);  
            };
        });
    }
});