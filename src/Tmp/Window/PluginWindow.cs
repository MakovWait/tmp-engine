using Tmp.Core.Plugins;
using Tmp.Core.Redot;
using Tmp.Core.Shelf;
using Tmp.Math;
using Tmp.Render.Components;
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
            windows.Create(settings);
        };
        
        app.PostClose += windows.Close;

        app.Shelf.Inspect<Tree>(tree =>
        {
            tree.OnInit += _ =>
            {
                tree.DecorateRootUp(new CViewport(windows.MainWindow));
            };
        });
    }
});