using Tmp.Core;
using Tmp.Core.Comp;
using Tmp.Math;
using Tmp.Window.Rl;

namespace Tmp.Window.Components;

public readonly struct WindowSettings
{
    public string? Title { get; init; }
    public Vector2I? Size { get; init; }
    public int? TargetFps { get; init; }
}

public class CWindow(WindowSettings settings) : ComponentFunc((self, children) =>
{
    var windows = self.UseContext<IWindows>();

    var input = new Input(true);
    windows.Start(settings, input);
    var window = windows.Main;
    
    window.BindTo(self);

    self.CreateContext(input);
    
    self.On<Draw>(_ => window.Draw());

    return children;
});

public class CWindowsRl() : ComponentFunc((self, children) =>
{
    var windows = new WindowsRl();
    self.CreateContext<IWindows>(windows);
    
    self.OnLateCleanup(() => windows.Close());

    return children;
});
