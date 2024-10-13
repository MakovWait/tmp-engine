using Raylib_cs;
using Tmp.Core;
using Tmp.Core.Redot;
using Tmp.HotReload.Components;
using Tmp.Resource;
using Tmp.Resource.Components;

namespace Tmp;

public readonly record struct Update(float Dt)
{
    public static implicit operator float(Update self) => self.Dt;
}

public readonly struct PreDraw;

public readonly struct Draw;

public class Game
{
    private readonly Tree _tree = new();

    public void Init(Action<Tree> setup)
    {
        const int screenWidth = 800;
        const int screenHeight = 450;

        Raylib.SetConfigFlags(ConfigFlags.TopmostWindow | ConfigFlags.ResizableWindow);
        Raylib.InitWindow(screenWidth, screenHeight, "Hello World");
        Raylib.SetTargetFPS(60);
        
        _tree.DecorateRootUp(new CResources(new Resources()));
        _tree.DecorateRootUp(new CHotReloadSource());
        
        setup.Invoke(_tree);
    }

    public void Run()
    {
        while (!Raylib.WindowShouldClose())
        {
            Render();
        }
        
        _tree.Free();
        Raylib.CloseWindow();
    }
    
    public void Render()
    {
        Input.Propagate(_tree);
        _tree.Call(new Update(Raylib.GetFrameTime()));
        _tree.Call<PreDraw>();
        _tree.Call<Draw>();
        _tree.Update();
    }
}
