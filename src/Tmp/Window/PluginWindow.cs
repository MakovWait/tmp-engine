using Tmp.Core;
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
) : PluginWrap<App>(new PluginAnonymous<App>("window")
{
    OnFinish = app =>
    {
        var windows = app.Val<IWindows>().GetOr(() => new WindowsRl());

        app.PreStart += () =>
        {
            var input = new Input(true);
            windows.Start(settings, input);
            app.SetSingleton(input);
        };

        app.PostClose += () =>
        {
            windows.Close();
        };
        
        app.DecorateRootUp(new Component(self =>
        {
            var window = windows.Main;
            window.BindTo(self);
            
            // TODO 🥶
            self.On<Draw>(() => window.Draw());
        }));
    }
});