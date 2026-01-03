using System.Diagnostics;

namespace Lofi2D.Core.Comp;

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
    private readonly ITraits _traits = new Traits();

    private readonly Tree _tree;

    public string Name { get; set; }
    public NodeState State { get; private set; } = NodeState.Building;

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

    public enum NodeState
    {
        Building,
        Mounted,
        QueuedForDeletion,
        Freed,
    }
    
    public void Mount()
    {
        State = NodeState.Mounted;
        _onMounts.InvokeOneShot();
        _children.Mount();
        _onLateMounts.InvokeOneShot();
    }

    public void Free()
    {
        _onCleanups.InvokeOneShot();
        _children.Free();
        _onLateCleanups.InvokeOneShot();
        State = NodeState.Freed;
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

    public T? UseNullableContext<T>()
    {
        return FindInContextOrNull<T>();
    }

    public void Call<T>(T args)
    {
        _callbacks.Call(args);
        _children.Call(args);
        _lateCallbacks.Call(args);
    }

    public ICallbackDispose On<T>(Action<T> action)
    {
        return _callbacks.Add(action);
    }

    public ICallbackDispose OnLate<T>(Action<T> action)
    {
        return _lateCallbacks.Add(action);
    }

    public void SetName(string value)
    {
        Name = value;
        _parent?.ValidateChildName(this);
    }

    public T AddTrait<T>(T trait) where T : Trait
    {
        _traits.AddTrait(trait);
        return trait;
    }

    public T GetTrait<T>() where T : Trait
    {
        return _traits.GetTrait<T>();
    }

    public T? TryGetTrait<T>() where T : Trait
    {
        return _traits.TryGetTrait<T>();
    }

    public bool HasTrait<T>() where T : Trait
    {
        return _traits.HasTrait<T>();
    }

    public bool RemoveTrait<T>() where T : Trait
    {
        return _traits.RemoveTrait<T>();
    }

    public void CallDeferred<T>(Action<T> action, T args)
    {
        _tree.CallDeferred(action, args);
    }

    private T FindInContext<T>()
    {
        var value = FindInContextOrNull<T>();
        
        if (value == null)
        {
            throw new Exception($"Unable to find a context value for {typeof(T)}");
        }

        return value;
    }
    
    private T? FindInContextOrNull<T>()
    {
        if (_parent == null)
        {
            return default;
        }

        if (_parent._ctx.Has<T>())
        {
            return _parent._ctx.Get<T>();
        }
        
        return _parent!.FindInContextOrNull<T>();
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

        public ICallbackDispose Add<T>(Action<T> callback)
        {
            var callbackWrapper = new Callback<T>(callback, this);
            _callbacks.Add(callbackWrapper);
            return callbackWrapper;
        }

        private void Remove(ICallback callback)
        {
            _callbacks.Remove(callback);
        }

        private interface ICallback : ICallbackDispose
        {
            void Handle<T>(T state);
        }
        
        private class Callback<Y>(Action<Y> callback, Callbacks callbacks) : ICallback
        {
            private bool _disposed;
            
            public void Handle<T>(T state)
            {
                Debug.Assert(!_disposed);;
                if (state is Y casted)
                {
                    callback(casted);
                }
            }

            public void Dispose()
            {
                Debug.Assert(!_disposed);;
                GC.SuppressFinalize(this);
                callbacks.Remove(this);
                _disposed = true;
            }
        }
    }
}

public interface ICallbackDispose : IDisposable;

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

public interface ICallDeferredSource
{
    void CallDeferred<T>(Action<T> action, T args);
}

public interface INodeInit : INodeLocation, ICallDeferredSource
{
    string Name { get; }

    Node.NodeState State { get; }
    
    void OnMount(Action mount);
    
    void OnLateMount(Action mount);
    
    void OnLateCleanup(Action cleanup);
    
    void OnCleanup(Action cleanup);

    T CreateContext<T>(T value);
    
    T UseContext<T>();
    
    T? UseNullableContext<T>();

    void Call<T>(T args);
    
    ICallbackDispose On<T>(Action<T> action);
    
    ICallbackDispose OnLate<T>(Action<T> action);

    void SetName(string value);
    
    T AddTrait<T>(T trait) where T : Trait;
    
    T GetTrait<T>() where T : Trait;
    
    T? TryGetTrait<T>() where T : Trait;
    
    bool HasTrait<T>() where T : Trait;
    
    bool RemoveTrait<T>() where T : Trait;
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

    public static T AutoDispose<T>(this INodeInit self, T disposable) where T : IDisposable
    {
        self.OnCleanup(() => disposable.Dispose());
        return disposable;
    }
    
    public static T DisposeWith<T>(this T self, INodeInit node) where T : IDisposable
    {
        return node.AutoDispose(self);
    }
}

public abstract class Trait
{
    
}

public interface ITraits
{
    void AddTrait<T>(T trait) where T : Trait;
    
    T GetTrait<T>() where T : Trait;
    
    T? TryGetTrait<T>() where T : Trait;
    
    bool HasTrait<T>() where T : Trait;
    
    bool RemoveTrait<T>() where T : Trait;
}

public class Traits : ITraits
{
    private readonly Dictionary<Type, Trait> _traits = new();
    
    public void AddTrait<T>(T trait) where T : Trait
    {
        Debug.Assert(!HasTrait<T>());
        _traits[typeof(T)] = trait;
    }

    public T GetTrait<T>() where T : Trait
    {
        if (_traits.TryGetValue(typeof(T), out var trait))
        {
            return (T)trait;
        }
        throw new KeyNotFoundException($"Trait of type {typeof(T)} not found.");
    }

    public T? TryGetTrait<T>() where T : Trait
    {
        if (_traits.TryGetValue(typeof(T), out var trait))
        {
            return (T)trait;
        }
        return null;
    }

    public bool HasTrait<T>() where T : Trait
    {
        return _traits.ContainsKey(typeof(T));
    }

    public bool RemoveTrait<T>() where T : Trait
    {
        return _traits.Remove(typeof(T));
    }
}
