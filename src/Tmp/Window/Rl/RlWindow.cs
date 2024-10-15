using System.Diagnostics;
using Raylib_cs;

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
    
    public void Close()
    {
       Raylib.CloseWindow();
    }
}

public class WindowsRl : IWindows
{
    private WindowRl? _current;
    
    public IWindow Create(WindowSettings settings)
    {
        Debug.Assert(_current == null);
        _current = new WindowRl(settings);
        return _current;
    }
}