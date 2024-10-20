using Tmp.Core.Plugins;
using Tmp.Core.Redot;
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
) : PluginWrap<App>(new PluginAnonymous<App>("raylib-window")
{
    OnFinish = app =>
    {
        var windows = app.Val<IWindows>().GetOr(() => new WindowsRl());

        app.PreStart += () =>
        {
            windows.Start(settings);
        };

        app.Val<Tree>().Inspect(tree =>
        {
            tree.OnInit += _ =>
            {
                var window = windows.Main;
                var viewport = window.Viewport;
                
                tree.DecorateRootUp(new Component(self =>
                {
                    viewport.BindTo(self);
                
                    // TODO 🥶
                    self.UseEffect(() =>
                    {
                        viewport.Load();
                        return () => viewport.Unload();
                    }, []);
                
                    self.On<Draw>(() => window.Draw());
                }));  
            };
        });
        
        app.PostClose += () =>
        {
            windows.Close();
        };
    }
});