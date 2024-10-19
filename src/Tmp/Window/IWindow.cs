using Tmp.Render;

namespace Tmp.Window;

public interface IWindow
{
    SubViewport Viewport { get; }

    void Update();
}