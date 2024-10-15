using Tmp.Core.Redot;
using Tmp.Window;

namespace Tmp.Render.Components;

public class CViewport(IWindow window) : Component(self =>
{
    var viewport = new Viewport(window);
    viewport.CreateContext(self);
    self.On<Draw>(() => viewport.Draw());
});