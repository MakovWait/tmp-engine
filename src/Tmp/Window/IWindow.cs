using Tmp.Core.Redot;

namespace Tmp.Window;

public interface IWindow
{
    void Draw();

    void BindTo(Component.Self self);
}