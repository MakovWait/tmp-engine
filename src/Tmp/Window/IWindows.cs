using Tmp.Core;

namespace Tmp.Window;

public interface IWindows
{
    IWindow Main { get; }

    void Start(WindowSettings settings, Input input);
    
    void Close();
}