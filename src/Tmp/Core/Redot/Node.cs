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
    private readonly Tree tree;
    private readonly Context ctx;
    private readonly Callbacks callbacksOn = new();
    private readonly Callbacks callbacksAfter = new();
    private readonly ContextValues contextValues = new();
    private readonly LifecycleEffects lifecycleEffects = new();
    private readonly Children children = new();
    
    private Node? parent;
    private IId id = new IdEmpty();
    private NodeState state = NodeState.Ready;
    private bool firstTimeEntersTree = true;

    internal Node(Tree tree)
    {
        ctx = new Context(this);
        this.tree = tree;
    }
    
    public void Call<T>(Action<T> call) where T : Delegate
    {
        callbacksOn.Call(call);
        children.Call(call);
        callbacksAfter.Call(call);
    }

    public ICreatedContextValue<T> CreateContext<T>(T val)
    {
        ctx.Create(val);
        return new CreatedContextValue<T>(ctx);
    }
    
    public IContextValue<T> Use<T>()
    {
        var val = new ContextValue<T>(this);
        contextValues.Add(val);
        return val;
    }
    
    public void On<T>(T callback) where T : Delegate
    {
        callbacksOn.Add(callback);
    }
    
    public void After<T>(T callback) where T : Delegate
    {
        callbacksAfter.Add(callback);
    }

    public void UseEffect(Effect effect)
    {
        lifecycleEffects.Add(effect);
    }

    public void SetId(string someId)
    {
        id = new Id(someId, this);
    }

    public RuntimeNodeRef GetNodeById(string nodeId)
    {
        return new RuntimeNodeRef(tree.IdNodeDict.Get(nodeId));
    }
    
    public void UseEffect(Func<Action> effect, IEffectDependency deps)
    {
        Action? cleanup = null;
        UseEffect(new Effect(() =>
        {
            deps.AboutToChange += Cleanup;
            deps.Changed += Trigger;

            if (firstTimeEntersTree)
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
        return children.List;
    }

    public Node? GetParent()
    {
        return parent;
    }
    
    public T FindInContext<T>()
    {
        if (parent == null)
        {
            throw new Exception($"Unable to find a context value for {typeof(T)}");
        }

        if (parent.ctx.Has<T>())
        {
            return parent.ctx.Get<T>();
        }
        
        return parent!.FindInContext<T>();
    }

    public void CallDeferred(Action action)
    {
        tree.CallDeferred(action);
    }
    
    public void CallDeferred(Action<Node> action)
    {
        tree.CallDeferred(() => action(this));
    }
    
    internal bool StateIs(NodeState s)
    {
        return state == s;
    }
    
    private void StateSet(NodeState s)
    {
        state = s;
    }
    
    private void ReparentInternal(Node newParent)
    {
        Debug.Assert(newParent != parent);
        parent?.RemoveChildInternal(this);
        newParent.AddChildInternal(this);
    }
    
    private void AddChildInternal(Node child)
    {
        Debug.Assert(child.parent is null);
        child.parent = this;
        children.Add(child);
    }
    
    private void RemoveChildInternal(Node child)
    {
        Debug.Assert(child.parent == this);
        child.parent = null;
        children.Remove(child);
    }
    
    public void CreateChild(IComponent component)
    {
        var node = CreateSubChild(component);
        node.OnEnterTree();
    }
    
    public void DecorateUp(IComponent component)
    {
        PropagateOnExitTree();
        
        var prevParent = parent;
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

    public void Undecorate()
    {
        var curParent = parent!;
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
        var node = new Node(tree);
        node.StateSet(NodeState.Building);
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
    
    private void OnEnterTree()
    {
        id.SetTo(tree.IdNodeDict);
        contextValues.Init();
        lifecycleEffects.Invoke();
        firstTimeEntersTree = false;
        children.OnEnterTree();
    }

    private void PropagateOnExitTree()
    {
        OnExitTreeThisOnly();
        children.OnExitTree();
    }
    
    private void OnExitTreeThisOnly()
    {
        lifecycleEffects.Cleanup();
        contextValues.Uninitialize();
        id.RemoveFrom(tree.IdNodeDict);
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
        parent?.RemoveChildInternal(this);
        children.Free();
    }
    
    public void QueueFree()
    {
        StateSet(NodeState.QueuedToDeletion);
        tree.QueueFree(this);
        // TODO ???????
        // children.QueueFree();
    }

    private void PropagateContextChanged<T>(T val)
    {
        contextValues.HandleContextChanged(val);
        if (ctx.Has<T>()) return;
        children.PropagateContextChanged(val);
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

        public void Call<T>(Action<T> call) where T : Delegate
        {
            foreach (var child in List)
            {
                child.Call(call);
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
            node.children.PropagateContextChanged(val);
        }
    }

    private class Callbacks
    {
        private readonly Dictionary<Type, List<object>> map = new();
    
        public void Call<T>(Action<T> call)
        {
            foreach (T callback in Bucket<T>())
            {
                call(callback);
            }
        }

        public void Add<T>(T callback) where T : Delegate
        {
            Bucket<T>().Add(callback);
        }

        private List<object> Bucket<T>()
        {
            if (!map.ContainsKey(typeof(T)))
            {
                map[typeof(T)] = [];
            }

            return map[typeof(T)];
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