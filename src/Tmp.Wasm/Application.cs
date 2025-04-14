using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Tmp.Window.Components;

namespace Tmp.Wasm;

public partial class Application
{
    private static App? _app;

    public static async Task Main()
    {
        _app = new App(new CWindowsRl
        {
            Project.Project.GetRoot()
        });
        _app.SetRunner(new AppRunnerBrowser());
        await _app.Run();
    }

    [JSExport]
    public static void UpdateFrame()
    {
        _app!.Update();
    }
}

internal class AppRunnerBrowser : IAppRunner
{
    public void Run(IRunnableApp app)
    {
        app.Start();
    }
}