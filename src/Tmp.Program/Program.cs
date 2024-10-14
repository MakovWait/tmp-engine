using Tmp;
using Tmp.Project;

var app = new App(Project.GetPlugins());
await app.Run();
