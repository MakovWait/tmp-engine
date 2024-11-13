using Tmp.Core.Comp;

namespace Tmp.Window;

public interface IWindow
{
    void Draw();

    void BindTo(INodeInit self);
}