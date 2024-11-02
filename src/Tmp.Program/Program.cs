using Tmp;
using Tmp.Project;
using Tmp.Window.Components;

var app = new App(new CWindowsRl()
{
    Project.GetRoot()
});
await app.Run();
