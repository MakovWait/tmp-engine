using Tmp.Core.Plugins;
using Tmp.Core.Shelf;

namespace Tmp;

public sealed class App : IRunnableApp, IShelf
{
    public event Action? PreStart;
    public event Action? OnStart;
    
    public event Action? PreUpdate;
    public event Action? OnUpdate;
    public event Action? PostUpdate;
    
    public event Action? OnClose;
    public event Action? PostClose;

    private readonly IPluginSource<App> _pluginsToInstall;
    private readonly InstalledPluginsFor<App> _installedPlugins;
    private readonly IShelf _shelf = new Shelf();
    private IAppRunner _runner = new IAppRunner.Default();

    public IShelf Shelf => _shelf;
    
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
        OnStart?.Invoke();
    }

    public void Update()
    {
        PreUpdate?.Invoke();
        OnUpdate?.Invoke();
        PostUpdate?.Invoke();
    }

    public void Close()
    {
       OnClose?.Invoke();
       PostClose?.Invoke();
    }

    async private Task InstallPlugins()
    {
        await _pluginsToInstall.AddTo(_installedPlugins);
        await _installedPlugins.Finish();
    }

    void IShelf.Set<T>(T value)
    {
        _shelf.Set(value);
    }

    T IShelf.Get<T>()
    {
        return _shelf.Get<T>();
    }

    bool IShelf.Has<T>()
    {
        return _shelf.Has<T>();
    }

    void IShelf.Del<T>()
    {
        _shelf.Del<T>();
    }

    IDisposable IShelf.OnDel<T>(Action<T> callback)
    {
        return _shelf.OnDel(callback);
    }

    IDisposable IShelf.OnSet<T>(Action<T> callback)
    {
        return _shelf.OnSet(callback);
    }
}

public interface IRunnableApp
{
    IShelf Shelf { get; }
    
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
