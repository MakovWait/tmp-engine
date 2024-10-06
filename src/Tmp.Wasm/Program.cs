using System.Runtime.InteropServices.JavaScript;

namespace Tmp.Wasm;

public partial class Application
{
    private static Game _game;

    public static void Main()
    {
        _game = new Game();
        Project.Project.Init(_game);
    }

    [JSExport]
    public static void UpdateFrame()
    {
        _game.Render();
    }
}