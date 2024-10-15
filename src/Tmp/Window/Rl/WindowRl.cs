using System.Diagnostics;
using Raylib_cs;
using Tmp.Math;

namespace Tmp.Window.Rl;

internal class WindowRl : IWindow
{
    public WindowRl(WindowSettings settings)
    {
        Raylib.SetConfigFlags(ConfigFlags.TopmostWindow | ConfigFlags.ResizableWindow);
        Raylib.InitWindow(
            settings.Size?.X ?? 800,
            settings.Size?.Y ?? 450,
            settings.Title ?? "Game"
        );
        Raylib.SetTargetFPS(settings.TargetFps ?? 60);      
    }

    public Vector2I Size => new(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
    
    public void BeginDraw()
    {
        Raylib.ClearBackground(Color.White);
        Raylib.BeginDrawing();
    }

    public void EndDraw()
    {
        Raylib.EndDrawing();
    }

    public void Close()
    {
       Raylib.CloseWindow();
    }
}

public class WindowsRl : IWindows
{
    private WindowRl? _current;

    public IWindow MainWindow => _current!;

    public void Close()
    {
        _current?.Close();
    }

    public IWindow Create(WindowSettings settings)
    {
        Debug.Assert(_current == null);
        _current = new WindowRl(settings);
        return _current;
    }
}