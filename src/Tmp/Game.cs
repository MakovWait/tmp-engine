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
    public readonly Tree Tree = new();

    public void Run(Action<Tree> setup)
    {
        const int screenWidth = 800;
        const int screenHeight = 450;

        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
        Raylib.InitWindow(screenWidth, screenHeight, "Hello World");
        Raylib.SetTargetFPS(60);
        
        setup.Invoke(Tree);

        while (!Raylib.WindowShouldClose())
        {
            Input.Propagate(Tree);
            Tree.QueueCall<Update>(update => update(Raylib.GetFrameTime()));
            Tree.QueueCall<PreDraw>(preDraw => preDraw());
            Tree.QueueCall<Draw>(draw => draw());
            Tree.Update();
        }

        Tree.Free();
        Raylib.CloseWindow();
    }
}
