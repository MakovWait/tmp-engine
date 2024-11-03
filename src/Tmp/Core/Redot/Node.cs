using System.Diagnostics;

namespace Tmp.Core.Redot;

internal enum NodeState
{
    Building,
    Ready,
    Built,
    Freed,
    QueuedToDeletion
}

public class Node
{
    private readonly Tree _tree;
    private readonly Context _ctx;
    private readonly Callbacks _callbacksOn = new();
    private readonly Callbacks _callbacksAfter = new();
    private readonly ContextValues _contextValues = new();
    private readonly LifecycleEffects _lifecycleEffectsOn = new();
    private readonly LifecycleEffects _lifecycleEffectsAfter = new();
    private readonly Children _children = new();
    private readonly ExportedVars _exportedVars = new();
    
    private Node? _parent;
    private IId _id = new IdEmpty();
    private NodeState _state = NodeState.Building;
    private bool _firstTimeEntersTree = true;

    internal Node(Tree tree)
    {
        _ctx = new Context(this);
        this._tree = tree;
    }
    
    public void Call<T>() where T : new()
    {
        Call(new T());
    }
    
    public void Call<T>(T state)
    {
        _callbacksOn.Call(state);
        _children.Call(state);
        _callbacksAfter.Call(state);
    }

    public ICreatedContextValue<T> CreateContext<T>(T val)
    {
        _ctx.Create(val);
        return new CreatedContextValue<T>(_ctx);
    }
    
    public IContextValue<T> Use<T>()
    {
        var val = new ContextValue<T>(this);
        _contextValues.Add(val);
        return val;
    }

    public ExportedVar<T> Export<T>(T initial)
    {
        return _exportedVars.Create(initial);
    }
    
    public ExportedVarOptional<T> ExportOpt<T>()
    {
        return _exportedVars.CreateOpt<T>();
    }

    public void SetSingleton<T>(T singleton)
    {
        _tree.SetSingleton(singleton);
    }
    
    public T UseSingleton<T>()
    {
        return _tree.UseSingleton<T>();
    }
    
    public void On<T>(Action<T> callback)
    {
        _callbacksOn.Add(callback);
    }
    
    public void After<T>(Action<T> callback)
    {
        _callbacksAfter.Add(callback);
    }

    public void UseEffect(Effect effect)
    {
        _lifecycleEffectsOn.Add(effect);
    }
    
    public void UseAfterEffect(Effect effect)
    {
        _lifecycleEffectsAfter.Add(effect);
    }

    public void SetId(string someId)
    {
        _id = new Id(someId, this);
    }

    public RuntimeNodeRef GetNodeById(string nodeId)
    {
        return new RuntimeNodeRef(_tree.IdNodeDict.Get(nodeId));
    }
    
    public void UseEffect(Func<Action> effect, IEffectDependency deps)
    {
        UseEffectWithDeps(effect, deps, _lifecycleEffectsOn);
    }
    
    public void UseAfterEffect(Func<Action> effect, IEffectDependency deps)
    {
        UseEffectWithDeps(effect, deps, _lifecycleEffectsAfter);
    }

    private void UseEffectWithDeps(Func<Action> effect, IEffectDependency deps, LifecycleEffects effects)
    {
        Action? cleanup = null;
        effects.Add(new Effect(() =>
        {
            deps.AboutToChange += Cleanup;
            deps.Changed += Trigger;

            if (_firstTimeEntersTree)
            {
                cleanup = effect();
            }
                
            return () =>
            {
                deps.AboutToChange -= Cleanup;
                deps.Changed -= Trigger;
                if (StateIs(NodeState.Freed))
                {
                    Cleanup();
                }
            };

            void Cleanup()
            {
                cleanup?.Invoke();
                cleanup = null;
            }
                
            void Trigger()
            {
                cleanup = effect();
            }
        }));
    }
    
    public void AddChild(Node node)
    {
        AddChildInternal(node);
        node.OnEnterTree();
    }

    public void RemoveChild(Node node)
    {
        node.PropagateOnExitTree();
        RemoveChildInternal(node);
    }

    public IReadOnlyList<Node> GetChildren()
    {
        return _children.List;
    }

    public Node? GetParent()
    {
        return _parent;
    }
    
    public T FindInContext<T>()
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

    public void CallDeferred(Action action)
    {
        _tree.CallDeferred(action);
    }
    
    public void CallDeferred(Action<Node> action)
    {
        _tree.CallDeferred(() => action(this));
    }
    
    internal bool StateIs(NodeState s)
    {
        return _state == s;
    }
    
    private void StateSet(NodeState s)
    {
        _state = s;
    }
    
    private void ReparentInternal(Node newParent)
    {
        Debug.Assert(newParent != _parent);
        _parent?.RemoveChildInternal(this);
        newParent.AddChildInternal(this);
    }
    
    private void AddChildInternal(Node child)
    {
        Debug.Assert(child._parent is null);
        child._parent = this;
        _children.Add(child);
    }
    
    private void RemoveChildInternal(Node child)
    {
        Debug.Assert(child._parent == this);
        child._parent = null;
        _children.Remove(child);
    }
    
    public void CreateChild(IComponent component)
    {
        var node = CreateSubChild(component);
        node.OnEnterTree();
    }
    
    public void DecorateUp(IComponent component)
    {
        PropagateOnExitTree();
        
        var prevParent = _parent;
        prevParent!.RemoveChildInternal(this);

        var newParent = prevParent.CreateSubChild(component);
        newParent.AddChildInternal(this);
        
        newParent.OnEnterTree();
    }
    
    public void DecorateDown(IComponent component)
    {
        var curChildren = GetChildren().ToArray();
        foreach (var curChild in curChildren)
        {
            RemoveChild(curChild);
        }
        
        var newParent = CreateSubChild(component);
        foreach (var curChild in curChildren)
        {
            newParent.AddChildInternal(curChild);
        }
        newParent.OnEnterTree();
    }

    public void Replace(IComponent component)
    {
        var curParent = _parent!;
        curParent.RemoveChild(this);
        
        var node = curParent.CreateSubChild(component);
        foreach (var child in GetChildren().ToArray())
        {
            child.ReparentInternal(node);
        }
        node.OnEnterTree();
        QueueFree();
    }

    public void Undecorate()
    {
        var curParent = _parent!;
        curParent.RemoveChild(this);
        
        foreach (var child in GetChildren().ToArray())
        {
            child.ReparentInternal(curParent);
            child.OnEnterTree();
        }
        
        QueueFree();
    }

    private Node CreateSubChild(IComponent component)
    {
        var node = new Node(_tree);
        var childComponents = component.Build(node);
        node.StateSet(NodeState.Built);
        foreach (var childComponent in childComponents)
        {
            node.CreateSubChild(childComponent);
        }
        AddChildInternal(node);
        node.StateSet(NodeState.Ready);
        return node;
    }
    
    internal void OnEnterTree()
    {
        if (_tree.IsBuilding()) return;
        _id.SetTo(_tree.IdNodeDict);
        _contextValues.Init();
        _lifecycleEffectsOn.Invoke();
        _firstTimeEntersTree = false;
        _children.OnEnterTree();
        _lifecycleEffectsAfter.Invoke();
    }

    private void PropagateOnExitTree()
    {
        if (_tree.IsBuilding()) return;
        OnExitTreeThisOnly();
        _children.OnExitTree();
        _lifecycleEffectsAfter.Cleanup();
    }
    
    private void OnExitTreeThisOnly()
    {
        _lifecycleEffectsOn.Cleanup();
        _contextValues.Uninitialize();
        _id.RemoveFrom(_tree.IdNodeDict);
    }

    public void SafeFree()
    {
        if (!StateIs(NodeState.Freed))
        {
            Free();
        }
    }

    private void Free()
    {
        Debug.Assert(!StateIs(NodeState.Freed));
        StateSet(NodeState.Freed);
        OnExitTreeThisOnly();
        _parent?.RemoveChildInternal(this);
        _children.Free();
        _lifecycleEffectsAfter.Cleanup();
    }
    
    public void QueueFree()
    {
        StateSet(NodeState.QueuedToDeletion);
        _tree.QueueFree(this);
        // TODO ???????
        // children.QueueFree();
    }

    private void PropagateContextChanged<T>(T val)
    {
        _contextValues.HandleContextChanged(val);
        if (_ctx.Has<T>()) return;
        _children.PropagateContextChanged(val);
    }

    private class ExportedVars
    {
        private readonly List<IExportedVar> _vars = [];
        
        public ExportedVar<T> Create<T>(T initial)
        {
            var exported = new ExportedVar<T>(initial);
            _vars.Add(exported);
            return exported;
        }

        public ExportedVarOptional<T> CreateOpt<T>()
        {
            var exported = new ExportedVarOptional<T>();
            _vars.Add(exported);
            return exported;
        }
    }

    private class ContextValues
    {
        private readonly List<IContextValue> values = [];
        
        public void Add<T>(ContextValue<T> val)
        {
            values.Add(val);
        }

        public void HandleContextChanged<T>(T val)
        {
            foreach (var cxtVal in values)
            {
                cxtVal.HandleContextChanged(val);
            }
        }

        public void Uninitialize()
        {
            foreach (var cxtVal in values)
            {
                cxtVal.Uninitialize();
            }
        }

        public void Init()
        {
            foreach (var cxtVal in values)
            {
                cxtVal.Init();
            }
        }
    }
    
    private class LifecycleEffects
    {
        private readonly List<Effect> effects = [];

        public void Add(Effect effect)
        {
            effects.Add(effect);
        }
        
        public void Invoke()
        {
            foreach (var effect in effects)
            {
                effect.Invoke();
            }
        }

        public void Cleanup()
        {
            foreach (var effect in effects)
            {
                effect.Cleanup();
            }
        }
    }
    
    public class Effect(Func<Action> effect)
    {
        private Action? cleanup;

        public void Invoke()
        {
            cleanup = effect();
        }

        public void Cleanup()
        {
            cleanup?.Invoke();
            cleanup = null;
        }
    }

    private class Children
    {
        public readonly List<Node> List = [];

        public void Add(Node child)
        {
            List.Add(child);
        }

        public void Remove(Node child)
        {
            List.Remove(child);
        }
        
        public void OnEnterTree()
        {
            foreach (var child in List)
            {
                child.OnEnterTree();
            }
        }

        public void OnExitTree()
        {
            foreach (var child in List)
            {
                child.PropagateOnExitTree();
            }
        }
    
        public void Free()
        {
            foreach (var child in List.ToArray())
            {
                child.Free();
            }
        }
    
        public void QueueFree()
        {
            foreach (var child in List)
            {
                child.QueueFree();
            }
        }

        public void Call<T>(T state)
        {
            foreach (var child in List)
            {
                child.Call(state);
            }
        }

        public void PropagateContextChanged<T>(T val)
        {
            foreach (var child in List)
            {
                child.PropagateContextChanged(val);
            }
        }
    }

    private class Context(Node node)
    {
        private readonly Dictionary<Type, object> map = new();

        public T Get<T>()
        {
            return (T)map[typeof(T)];
        }

        public bool Has<T>()
        {
            return map.ContainsKey(typeof(T));
        }
    
        public void Create<T>(T val)
        {
            if (val == null)
            {
                throw new ArgumentNullException(nameof(val));
            }
            
            map[typeof(T)] = val;
        }

        public void Replace<T>(T val)
        {
            Create(val);
            node._children.PropagateContextChanged(val);
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

    private interface IId
    {
        void SetTo(IIdNodeDict dict);
        
        void RemoveFrom(IIdNodeDict dict);
    }

    private class IdEmpty : IId
    {
        public void SetTo(IIdNodeDict dict)
        {
        }

        public void RemoveFrom(IIdNodeDict dict)
        {
        }
    }
    
    private class Id(string id, Node node) : IId
    {
        public void SetTo(IIdNodeDict dict)
        {
            dict.Put(id, node);
        }

        public void RemoveFrom(IIdNodeDict dict)
        {
            dict.Remove(id);
        }
    }

    private class CreatedContextValue<T>(Context ctx) : ICreatedContextValue<T>
    {
        public T Get()
        {
            return ctx.Get<T>();
        }

        public void Replace(T val)
        {
            ctx.Replace(val);
        }
    }
}

public interface IEffectDependency
{
    event Action AboutToChange;
    event Action Changed;
}

internal class EffectDependencies : IEffectDependency
{
    public event Action? AboutToChange;
    public event Action? Changed;
    
    public EffectDependencies(IEnumerable<IEffectDependency> dependencies)
    {
        foreach (var dependency in dependencies)
        {
            dependency.AboutToChange += () => AboutToChange?.Invoke();
            dependency.Changed += () => Changed?.Invoke();
        }
    }
}