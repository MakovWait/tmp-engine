using System.Diagnostics;

namespace Tmp.Core.Comp;

[DebuggerDisplay("{NodePath}")]
public class Node : INodeInit
{
    private Node? _parent;
    private readonly Context _ctx = new();
    private readonly Children _children = new();
    private readonly Callbacks _callbacks = new();
    private readonly Callbacks _lateCallbacks = new();
    private readonly LifecycleCallbacks _onCleanups = new();
    private readonly LifecycleCallbacks _onLateCleanups = new();
    private readonly LifecycleCallbacks _onMounts = new();
    private readonly LifecycleCallbacks _onLateMounts = new();
    
    private NodeState _state = NodeState.Building;
    private readonly Tree _tree;

    public string Name { get; set; }

    public void OnMount(Action mount)
    {
        _onMounts.Add(mount);
    }

    public void OnLateMount(Action mount)
    {
        _onLateMounts.Add(mount);
    }

    public Node(string initialName, Tree tree)
    {
        _tree = tree;
        Name = initialName;
    }

    enum NodeState
    {
        Building,
        Mounted,
        QueuedForDeletion,
        Freed,
    }
    
    public void Mount()
    {
        _state = NodeState.Mounted;
        _onMounts.InvokeOneShot();
        _children.Mount();
        _onLateMounts.InvokeOneShot();
    }

    public void Free()
    {
        _onCleanups.InvokeOneShot();
        _children.Free();
        _onLateCleanups.InvokeOneShot();
        _state = NodeState.Freed;
        _onCleanups.Clear();
        _onLateCleanups.Clear();
    }

    public Node? GetNode(NodePath nodePath)
    {
        Node? current = null;
        if (!nodePath.IsAbsolute())
        {
            current = this;
        }
        
        foreach (var name in nodePath.Names())
        {
            Node? next = null;
            if (name == ".")
            {
                next = current;
            }
            else if (name == "..")
            {
                if (current?._parent is null)
                {
                    return null;
                }

                next = current._parent;
            }
            else if (current == null)
            {
                if (name.Equals(_tree.Root.Name))
                {
                    next = _tree.Root;
                }
            }
            else
            {
                next = null;
                var node = current._children.GetByName(name);
                if (node != null)
                {
                    next = node;
                }
                else
                {
                    return null;
                }
            }
            current = next;
        }

        return current;
    }
    
    public void AddChild(Node child)
    {
        Debug.Assert(child._parent is null);
        child._parent = this;
        _children.Add(child);
        ValidateChildName(child);
    }
    
    public void RemoveChild(Node child)
    {
        Debug.Assert(child._parent == this);
        child._parent = null;
        _children.Remove(child);
    }

    public IReadOnlyList<Node> GetChildren()
    {
        return _children.List;
    }
    
    public void ClearChildren()
    {
        for (var i = _children.List.Count - 1; i >= 0; i--)
        {
            var child = _children.List[i];
            RemoveChild(child);
            child.Free();
        }
    }
    
    public void Init(Action<Node> init)
    {
        init(this);
    }

    private void ValidateChildName(Node child)
    {
        var name = child.Name;
        var unique = _children.NameIsUnique(child);
        if (unique) return;
        child.Name = $"@{name}@{_children.IndexOf(child)}";
    }

    public void OnLateCleanup(Action cleanup)
    {
        _onLateCleanups.Add(cleanup);
    }

    public void OnCleanup(Action cleanup)
    {
        _onCleanups.Add(cleanup);
    }

    public T CreateContext<T>(T value)
    {
        _ctx.Create(value);
        return value;
    }

    public T UseContext<T>()
    {
        return FindInContext<T>();
    }

    public void Call<T>(T args)
    {
        _callbacks.Call(args);
        _children.Call(args);
        _lateCallbacks.Call(args);
    }

    public void On<T>(Action<T> action)
    {
        _callbacks.Add(action);
    }

    public void OnLate<T>(Action<T> action)
    {
        _lateCallbacks.Add(action);
    }

    public void SetName(string value)
    {
        Name = value;
        _parent?.ValidateChildName(this);
    }

    public void CallDeferred<T>(Action<T> action, T args)
    {
        _tree.CallDeferred(action, args);
    }

    private T FindInContext<T>()
    {
        if (_parent == null)
        {
            throw new Exception($"Unable to find a context value for {typeof(T)}");
        }

        if (_parent._ctx.Has<T>())
        {
            return _parent._ctx.Get<T>();
        }
        
        return _parent!.FindInContext<T>();
    }
    
    private string NodePath => string.Join("/", _parent?.NodePath ?? "", Name);
    
    private class Children
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly List<Node> List = [];

        public void Add(Node child)
        {
            List.Add(child);
        }

        public void Remove(Node child)
        {
            List.Remove(child);
        }
        
        public void Call<T>(T state)
        {
            foreach (var child in List)
            {
                child.Call(state);
            }
        }

        public void Free()
        {
            foreach (var child in List)
            {
                child.Free();
            }
        }

        public void Mount()
        {
            foreach (var child in List)
            {
                child.Mount();
            }
        }

        public int IndexOf(Node node)
        {
            return List.IndexOf(node);
        }

        public bool NameIsUnique(Node child)
        {
            foreach (var node in List)
            {
                if (node == child) continue;
                if (node.Name.Equals(child.Name))
                {
                    return false;
                }
            }
            return true;
        }

        public Node? GetByName(string name)
        {
            foreach (var node in List)
            {
                if (node.Name.Equals(name))
                {
                    return node;
                }
            }
            return null;
        }
    }
    
    private class Context
    {
        private readonly Dictionary<Type, object> _map = new();

        public T Get<T>()
        {
            return (T)_map[typeof(T)];
        }

        public bool Has<T>()
        {
            return _map.ContainsKey(typeof(T));
        }
    
        public void Create<T>(T val)
        {
            if (val == null)
            {
                throw new ArgumentNullException(nameof(val));
            }
            
            _map[typeof(T)] = val;
        }
    }

    internal class LifecycleCallbacks
    {
        private readonly List<Action> _callbacks = [];

        public void Clear()
        {
            _callbacks.Clear();
        }
        
        public void InvokeOneShot()
        {
            Invoke();
            Clear();
        }
        
        public void Invoke()
        {
            foreach (var callback in _callbacks)
            {
                callback();
            }
        }
        
        public void Add(Action callback)
        {
            _callbacks.Add(callback);
        }
    }
    
    internal class Callbacks
    {
        private readonly List<ICallback> _callbacks = [];
    
        public void Call<T>(T state)
        {
            foreach (var callback in _callbacks)
            {
                callback.Handle(state);
            }
        }

        public void Add<T>(Action<T> callback)
        {
            _callbacks.Add(new Callback<T>(callback));
        }

        private interface ICallback
        {
            void Handle<T>(T state);
        }
        
        private class Callback<Y>(Action<Y> callback) : ICallback
        {
            public void Handle<T>(T state)
            {
                if (state is Y casted)
                {
                    callback(casted);
                }
            }
        }
    }
}

[DebuggerDisplay("{AsString}")]
public sealed class NodePath(string path) : IEquatable<string>
{
    public bool IsAbsolute()
    {
        return path.StartsWith('/');
    }

    public IReadOnlyList<string> Names()
    {
        return path.Split('/').Where(x => !x.Equals("")).ToList();
    }

    public bool Equals(string? other)
    {
        return path.Equals(other);
    }
    
    private string AsString => path;
}

public interface IDeferredAction
{
    void Invoke();
}

public class DeferredActionWithArgs<T>(Action<T> action, T args) : IDeferredAction
{
    public void Invoke()
    {
        action(args);
    }
}

public class DeferredQueue : IDeferredQueue
{
    private readonly List<IDeferredAction> _actions = [];
    
    public void Enqueue(IDeferredAction action)
    {
        if (_actions.Contains(action)) return;
        _actions.Add(action);
    }

    public void Flush()
    {
        foreach (var action in _actions)
        {
            action.Invoke();
        }
        _actions.Clear();
    }
}

public interface IDeferredQueue
{
    public void Enqueue(IDeferredAction deferredAction);
}

public class Tree : IDeferredQueue
{
    private readonly TreeDeferredQueue _deferredQueue = new();
    private Node? _root;

    internal Node Root => _root!;

    public void Call<T>(T args)
    {
        _root!.Call(args);
    }
    
    public void Call<T>() where T : new()
    {
        _root!.Call(new T());
    }

    public void Free()
    {
        _root!.Free();
    }
    
    public void Build(IComponent component)
    {
        _root = component.Build(this, null);
        _root.Mount();
    }

    public void Build(Node root)
    {
        _root = root;
        _root.Mount();
    }

    public void FlushDeferredQueue()
    {
        _deferredQueue.Flush();
    }

    public void Enqueue(IDeferredAction action)
    {
        _deferredQueue.Enqueue(action);
    }
    
    public void CallDeferred<T>(Action<T> action, T args)
    {
        // TODO pool
        Enqueue(new DeferredActionWithArgs<T>(action, args));
    }

    internal Node CreateNode(string name)
    {
        return new Node(name, this);
    }

    private class TreeDeferredQueue
    {
        private readonly Queue<DeferredQueue> _buffer = new();
        private DeferredQueue _current;

        public TreeDeferredQueue()
        {
            _current = new DeferredQueue();
            _buffer.Enqueue(new DeferredQueue());
            _buffer.Enqueue(_current);
        }

        public void Enqueue(IDeferredAction action)
        {
            _current.Enqueue(action);
        }

        private void ChangeBuffer()
        {
            _current = _buffer.Dequeue();
            _buffer.Enqueue(_current);
        }
        
        public void Flush()
        {
            var bufferToFlush = _current;
            ChangeBuffer();
            bufferToFlush.Flush();
        }
    }
}

public delegate Action CreateSignalFactory<out T>(Action<T> set);

public interface INodeLocator
{
    Node? Get(INodeLocation location);
}

public static class NodeLocatorEx
{
    public static INodeLocator AsNodePathLocator(this string self)
    {
        return new LocatorNodePath(new NodePath(self));
    }
    
    public static INodeLocator AsNodePathLocator(this NodePath self)
    {
        return new LocatorNodePath(self);
    }
}

[DebuggerDisplay("{_path}")]
public class LocatorNodePath(NodePath path) : INodeLocator
{
    private readonly NodePath _path = path;
    
    public Node? Get(INodeLocation location)
    {
        return location.GetNode(_path);
    }
}

public interface INodeLocation
{
    Node? GetNode(NodePath nodePath);
}

public interface INodeInit : INodeLocation
{
    string Name { get; }

    void OnMount(Action mount);
    
    void OnLateMount(Action mount);
    
    void OnLateCleanup(Action cleanup);
    
    void OnCleanup(Action cleanup);

    T CreateContext<T>(T value);
    
    T UseContext<T>();

    void Call<T>(T args);
    
    void On<T>(Action<T> action);
    
    void OnLate<T>(Action<T> action);

    void SetName(string value);

    void CallDeferred<T>(Action<T> action, T args);
}

public static class NodeInitEx
{
    public static void Call<T>(this INodeInit self) where T : new()
    {
        self.Call<T>(new T());
    }
    
    public static void CallDeferred(this INodeInit self, Action action)
    {
        self.CallDeferred(act => act(), action);
    }
}
