namespace Tmp.Window;

public interface IWindows
{
    IWindow MainWindow { get; }

    void Close();
    
    IWindow Create(WindowSettings settings);
}