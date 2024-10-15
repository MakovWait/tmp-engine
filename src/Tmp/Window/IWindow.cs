using Tmp.Render;

namespace Tmp.Window;

public interface IWindow : IRenderTarget
{
    void Close();
}