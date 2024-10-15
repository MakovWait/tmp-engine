using Tmp.Core.Plugins;
using Tmp.Core.Shelf;
using Tmp.Math;
using Tmp.Window.Rl;

namespace Tmp.Window;

public readonly struct WindowSettings
{
    public string? Title { get; init; }
    public Vector2I? Size { get; init; }
    public int? TargetFps { get; init; }
}

public class PluginWindow(
    WindowSettings settings
) : PluginWrap<App>(new PluginAnonymous<App>("window")
{
    OnFinish = app =>
    {
        var windows = app.Shelf.Get<IWindows>(() => new WindowsRl());
        app.PreStart += () =>
        {
            var window = windows.Create(settings);  
            app.PostClose += window.Close;
        };
    }
});