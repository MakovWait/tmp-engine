using Raylib_cs;
using Tmp.Core;
using Tmp.Core.Redot;

namespace Tmp;

public delegate void Update(float dt);

public delegate void PreDraw();

public delegate void Draw();

public delegate void Input<in T>(T e);

public class Game
{
    private readonly Tree _tree = new();

    public void Init(Action<Tree> setup)
    {
        const int screenWidth = 800;
        const int screenHeight = 450;

        Raylib.SetConfigFlags(ConfigFlags.TopmostWindow);
        Raylib.InitWindow(screenWidth, screenHeight, "Hello World");
        Raylib.SetTargetFPS(60);
        
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
        _tree.QueueCall<Update>(update => update(Raylib.GetFrameTime()));
        _tree.QueueCall<PreDraw>(preDraw => preDraw());
        _tree.QueueCall<Draw>(draw => draw());
        _tree.Update();
    }
}
