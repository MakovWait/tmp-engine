using System.Diagnostics;

namespace Tmp.Core.Redot;

public class Tree
{
    private readonly Node _root;
    private readonly Node _hiddenRoot;
    private readonly FreeQueue _freeQueue = new();
    private readonly Queue<Action> _callQueue = new();
    private readonly Queue<Action> _callDeferredQueue = new();
    private readonly IdNodeDict _idNodeDict = new();
    private readonly Dictionary<Type, object> _singletons = new();
    private readonly Queue<Action<Tree>> _builders = new();

    internal IIdNodeDict IdNodeDict => _idNodeDict;
    private bool _isReady;
    
    public Tree(bool isReady=true)
    {
        _isReady = isReady;
        _hiddenRoot = new Node(this);
        _root = new Node(this);
        _hiddenRoot.AddChild(_root);
        _idNodeDict.Put("root", _root);
    }

    public void Build()
    {
        while (_builders.Count > 0)
        {
            _builders.Dequeue()(this);
        }
        _isReady = true;
        _hiddenRoot.OnEnterTree();
    }
    
    public void Update()
    {
        while (_callQueue.Count > 0)
        {
            _callQueue.Dequeue()();
        }

        while (_callDeferredQueue.Count > 0)
        {
            _callDeferredQueue.Dequeue()();
        }

        _freeQueue.DequeueAll();
    }
    
    public void AttachToRoot(IComponent component)
    {
        _root.CreateChild(component);
    }
    
    public void DecorateRootUp(IComponent component)
    {
        _root.DecorateUp(component);
    }

    public void Free()
    {
        _hiddenRoot.SafeFree();
    }
    
    public void Call<T>() where T : new()
    {
        _hiddenRoot.Call(new T());
    }
    
    public void Call<T>(T state)
    {
        _hiddenRoot.Call(state);
    }

    internal void CallDeferred(Action action)
    {
        _callDeferredQueue.Enqueue(action);
    }

    internal void QueueBuild(Action<Tree> build)
    {
        _builders.Enqueue(build);
    }
    
    internal void QueueFree(Node node)
    {
        _freeQueue.Add(node);
    }

    public void SetSingleton<T>(T singleton)
    {
        Debug.Assert(!_singletons.ContainsKey(typeof(T)));
        _singletons[typeof(T)] = singleton ?? throw new ArgumentNullException(nameof(singleton));   
    }
    
    public T UseSingleton<T>()
    {
        return (T)_singletons[typeof(T)];
    }

    public bool IsBuilding()
    {
        return !_isReady;
    }
}

public interface ITreeBuilder
{
    void SetSingleton<T>(T singleton);
    
    void DecorateRootUp(IComponent component);

    void AttachToRoot(IComponent component);
}

internal class FreeQueue
{
    private readonly Queue<Node> nodes = new();
    
    public void Add(Node node)
    {
        nodes.Enqueue(node);
    }

    public void DequeueAll()
    {
        while (nodes.Count > 0)
        {
            var node = nodes.Dequeue();
            node.SafeFree();
        }
    }
}

internal interface IIdNodeDict
{
    Node Get(string id);
    
    void Put(string id, Node node);
    
    void Remove(string id);
}

internal class IdNodeDict : IIdNodeDict
{
    private readonly Dictionary<string, Node> dict = new();

    public Node Get(string id)
    {
        return dict[id];
    }

    public void Put(string id, Node node)
    {
        if (!dict.TryAdd(id, node))
        {
            throw new Exception($"Node with id {id} is already registered!!!");
        }
    }
    
    public void Remove(string id)
    {
        dict.Remove(id);
    }
}