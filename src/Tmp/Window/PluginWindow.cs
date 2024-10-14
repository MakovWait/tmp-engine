using Raylib_cs;
using Tmp.Core.Plugins;
using Tmp.Math;

namespace Tmp.Window;

public readonly struct WindowSettings
{
    public string? Title { get; init; }
    public Vector2I? Size { get; init; }
    public int? TargetFps { get; init; }
}

public class PluginWindow(
    WindowSettings settings
) : PluginWrap<App>(new PluginAnonymous<App>("raylib-window")
{
    OnBuild = async app =>
    {
        app.PreStart += () =>
        {
            Raylib.SetConfigFlags(ConfigFlags.TopmostWindow | ConfigFlags.ResizableWindow);
            Raylib.InitWindow(
                settings.Size?.X ?? 800,
                settings.Size?.Y ?? 450,
                settings.Title ?? "Game"
            );
            Raylib.SetTargetFPS(settings.TargetFps ?? 60);            
        };
        app.PostClose += Raylib.CloseWindow;
    }
});