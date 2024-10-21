using Raylib_cs;
using Tmp.Core.Plugins;
using Tmp.Core.Redot;
using Tmp.Core.Resource;

namespace Tmp;

public readonly record struct PreUpdate(float Dt)
{
    public static implicit operator float(PreUpdate self) => self.Dt;
}

public readonly record struct Update(float Dt)
{
    public static implicit operator float(Update self) => self.Dt;
}

public readonly record struct PostUpdate(float Dt)
{
    public static implicit operator float(PostUpdate self) => self.Dt;
}

public readonly record struct PreDraw;
public readonly record struct Draw;
public readonly record struct PostDraw;

public sealed class App : IRunnableApp, IResources, ITreeBuilder
{
    public event Action? PreStart; 
    public event Action? PostClose; 
    
    private readonly IPluginSource<App> _pluginsToInstall;
    private readonly InstalledPluginsFor<App> _installedPlugins;
    private readonly Resources _resources = new();
    private readonly Tree _tree = new(isReady: false);
    private IAppRunner _runner = new IAppRunner.Default();
    
    public bool ShouldClose { get; set; }

    public App(IPluginSource<App> pluginsToInstall)
    {
        _pluginsToInstall = pluginsToInstall;
        _installedPlugins = new InstalledPluginsFor<App>(this);
    }

    public void SetRunner(IAppRunner runner) 
    {
        _runner = runner;
    }
    
    public async Task Run()
    {
        await InstallPlugins();
        _runner.Run(this);
    }

    public void Start()
    {
        PreStart?.Invoke();
        _tree.Build();
    }

    public void Update()
    {
        _tree.Call(new PreUpdate(Raylib.GetFrameTime()));
        _tree.Call(new Update(Raylib.GetFrameTime()));
        _tree.Call(new PostUpdate(Raylib.GetFrameTime()));

        _tree.Call<PreDraw>();
        _tree.Call<Draw>();
        _tree.Call<PostDraw>();
        
        _tree.Update();
    }

    public void Close()
    {
        _tree.Free();
        PostClose?.Invoke();
    }

    private async Task InstallPlugins()
    {
        await _pluginsToInstall.AddTo(_installedPlugins);
        await _installedPlugins.Finish();
    }

    void IResources.Set<T>(T value)
    {
        _resources.Set(value);
    }

    T IResources.Get<T>()
    {
        return _resources.Get<T>();
    }

    bool IResources.Has<T>()
    {
        return _resources.Has<T>();
    }

    void IResources.Del<T>()
    {
        _resources.Del<T>();
    }

    IDisposable IResources.OnDel<T>(Action<T> callback)
    {
        return _resources.OnDel(callback);
    }

    IDisposable IResources.OnSet<T>(Action<T> callback)
    {
        return _resources.OnSet(callback);
    }

    public void SetSingleton<T>(T singleton)
    {
        _tree.SetSingleton(singleton);
    }

    public void DecorateRootUp(IComponent component)
    {
        _tree.QueueBuild(tree => tree.DecorateRootUp(component));
    }

    public void AttachToRoot(IComponent component)
    {
        _tree.QueueBuild(tree => tree.AttachToRoot(component));
    }
}

public interface IRunnableApp
{
    bool ShouldClose { get; set; }
    
    void Start();
    
    void Update();

    void Close();
}

public interface IAppRunner
{
    public void Run(IRunnableApp app);
    
    public class Default : IAppRunner 
    {
        public void Run(IRunnableApp app)
        {
            app.Start();

            while (!app.ShouldClose)
            {
                app.Update();
            }

            app.Close();
        }
    }
}
