namespace Tmp.Window;

public interface IWindows
{
    IWindow Main { get; }

    void Start(WindowSettings settings);
    
    void Close();
}