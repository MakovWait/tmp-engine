using Tmp.Core.Redot;

namespace Tmp.Render.Components;

public class CViewport() : Component(self =>
{
    var viewport = new Viewport();
    viewport.CreateContext(self);
    self.On<Draw>(() => viewport.Draw());
});