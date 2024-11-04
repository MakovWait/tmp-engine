using System.Collections;
using System.Diagnostics;

namespace Tmp.Core.Redot;

public class Component(Func<Component.Self, IEnumerable<IComponent>> build) 
    : IComponent, IEnumerable<IFakeEnumerable>, IFakeEnumerable
{
    private readonly List<IComponent> outerChildren = [];
    
    public List<IComponent> Children
    {
        set => outerChildren.AddRange(value);
    }

    public Component() : this(self => self.Children.ToArray())
    {
    }
    
    public Component(Action<Self> build) : this(node =>
    {
        // node.Children throws error after node building
        var children = node.Children.ToArray();
        build(node);
        return children;
    })
    {
    }

    public Component(Func<Self, IComponent> build) : this(node => [build(node)])
    {
        
    }

    public void Add(IComponent outerChild)
    {
        outerChildren.Add(outerChild);
    }

    IEnumerable<IComponent> IComponent.Build(Node node)
    {
        var self = new Self(node, outerChildren);
        var children = build(self);
        return children;
    }

    public IEnumerator<IFakeEnumerable> GetEnumerator()
    {
        yield break;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public class Self(Node @unchecked, List<IComponent> outerChildren)
    {
        public List<IComponent> Children
        {
            get
            {
                // TODO hmmmm
                // AssertNodeIsBuilding();
                return outerChildren;
            }
        }

        private readonly Node @unchecked = @unchecked;
        
        [Obsolete("Not really obsolete but wanna make an accent that using it is not welcome")]
        public Node Unchecked { get; } = @unchecked;

        public void SetId(string id)
        {
            AssertNodeIsBuilding();
            @unchecked.SetId(id);
        }

        public RuntimeNodeRef RuntimeRef => new(@unchecked);
        
        public RuntimeNodeRef GetNodeById(string id)
        {
            AssertNodeIsReady();
            return @unchecked.GetNodeById(id);
        }

        public void Call<TState>(TState state)
        {
            AssertNodeIsReady();
            @unchecked.Call(state);
        }
        
        public void Call<TState>() where TState : new()
        {
            AssertNodeIsReady();
            @unchecked.Call<TState>();
        }

        public IRefMut<T> UseRef<T>(T initial = default!)
        {
            return new Ref<T>(initial);
        } 
        
        public void UseEffect(Func<Action> effect)
        {
            AssertNodeIsBuilding();
            @unchecked.UseEffect(new Node.Effect(effect));
        }
    
        public void UseEffect(Action effect)
        {
            AssertNodeIsBuilding();
            @unchecked.UseEffect(new Node.Effect(() =>
            {
                effect();
                return () => { };
            }));
        }
        
        public void UseEffect(Action effect, IEffectDependency deps)
        {
            UseEffect(() =>
            {
                effect();
                return () => { };
            }, deps);
        }
        
        public void UseEffect(Action effect, IEnumerable<IEffectDependency> deps)
        {
            UseEffect(() =>
            {
                effect();
                return () => { };
            }, new EffectDependencies(deps));
        }
        
        public void UseEffect(Func<Action> effect, IEnumerable<IEffectDependency> deps)
        {
            UseEffect(effect, new EffectDependencies(deps));
        }

        public void UseTask(Func<CancellationToken, Task> taskCtor, Action<Task> callback, IEnumerable<IEffectDependency> deps)
        {
            AssertNodeIsBuilding();
            @unchecked.UseTask(taskCtor, callback, new EffectDependencies(deps));
        }
        
        public void UseTask(Func<CancellationToken, Task> taskCtor, Action<Task> callback, IEffectDependency deps)
        {
            AssertNodeIsBuilding();
            @unchecked.UseTask(taskCtor, callback, deps);
        }
        
        public void UseTask<T>(Func<CancellationToken, Task<T>> taskCtor, Action<Task<T>> callback, IEnumerable<IEffectDependency> deps)
        {
            AssertNodeIsBuilding();
            @unchecked.UseTask(taskCtor, callback, new EffectDependencies(deps));
        }
        
        public void UseTask<T>(Func<CancellationToken, Task<T>> taskCtor, Action<Task<T>> callback, IEffectDependency deps)
        {
            AssertNodeIsBuilding();
            @unchecked.UseTask(taskCtor, callback, deps);
        }
        
        public void UseCleanup(Action cleanup)
        {
            UseEffect(() => cleanup, []);
        }
        
        public void UseEffect(Func<Action> effect, IEffectDependency deps)
        {
            AssertNodeIsBuilding();
            @unchecked.UseEffect(effect, deps);
        }

        public void UseAfterEffect(Func<Action> effect)
        {
            AssertNodeIsBuilding();
            @unchecked.UseAfterEffect(new Node.Effect(effect));
        }
    
        public void UseAfterEffect(Action effect)
        {
            AssertNodeIsBuilding();
            @unchecked.UseAfterEffect(new Node.Effect(() =>
            {
                effect();
                return () => { };
            }));
        }
        
        public void UseAfterEffect(Action effect, IEffectDependency deps)
        {
            UseAfterEffect(() =>
            {
                effect();
                return () => { };
            }, deps);
        }
        
        public void UseAfterEffect(Action effect, IEnumerable<IEffectDependency> deps)
        {
            UseAfterEffect(() =>
            {
                effect();
                return () => { };
            }, new EffectDependencies(deps));
        }
        
        public void UseAfterEffect(Func<Action> effect, IEnumerable<IEffectDependency> deps)
        {
            UseAfterEffect(effect, new EffectDependencies(deps));
        }

        public void UseAfterCleanup(Action cleanup)
        {
            UseAfterEffect(() => cleanup, []);
        }
        
        public void UseAfterEffect(Func<Action> effect, IEffectDependency deps)
        {
            AssertNodeIsBuilding();
            @unchecked.UseAfterEffect(effect, deps);
        }
        
        public void QueueFree()
        {
            @unchecked.QueueFree();
        }
        
        public ICreatedContextValue<T> CreateContext<T>(T val)
        {
            AssertNodeIsBuilding();
            return @unchecked.CreateContext(val);
        }
        
        public T FindInContext<T>()
        {
            return @unchecked.FindInContext<T>();
        }

        public IContextValue<T> Use<T>()
        {
            AssertNodeIsBuilding();
            return @unchecked.Use<T>();
        }

        public ExportedVarOptional<T> Export<T>()
        {
            AssertNodeIsBuilding();
            return @unchecked.ExportOpt<T>();
        }
        
        public ExportedVar<T> Export<T>(T initial)
        {
            AssertNodeIsBuilding();
            return @unchecked.Export(initial);
        }

        public void On<T>(Action callback)
        {
            AssertNodeIsBuilding();
            @unchecked.On<T>(_ => callback());
        }
        
        public void On<T>(Action<T> callback)
        {
            AssertNodeIsBuilding();
            @unchecked.On(callback);
        }
        
        public void After<T>(Action callback)
        {
            AssertNodeIsBuilding();
            @unchecked.After<T>(_ => callback());
        }
        
        public void After<T>(Action<T> callback)
        {
            AssertNodeIsBuilding();
            @unchecked.After(callback);
        }
        
        public void SetSingleton<T>(T singleton)
        {
            AssertNodeIsBuilding();
            @unchecked.SetSingleton(singleton);
        }
    
        public T UseSingleton<T>()
        {
            AssertNodeIsBuilding();
            return @unchecked.UseSingleton<T>();
        }

        private void AssertNodeIsReady()
        {
            Debug.Assert(
                @unchecked.StateIs(NodeState.Ready), 
                "Unable to call this on the node that is not ready"
            );
        }
        
        private void AssertNodeIsBuilding()
        {
            Debug.Assert(
                @unchecked.StateIs(NodeState.Building), 
                "Unable to call this on the node that is already built"
            );
        }
    }
}

public interface IComponent
{
    internal IEnumerable<IComponent> Build(Node node);
}

public interface IFakeEnumerable;

public class ComponentWithRef(IComponent origin, IRefMut<RuntimeNodeRef?> nodeRef) : IComponent
{
    public IEnumerable<IComponent> Build(Node node)
    {
        var children = origin.Build(node); 
        nodeRef.Set(new RuntimeNodeRef(node));
        return children;
    }
}

public static class ComponentEx
{
    public static IComponent WithRef(this IComponent self, IRefMut<RuntimeNodeRef?> nodeRef)
    {
        return new ComponentWithRef(self, nodeRef);
    } 
    
    public static Component WithChildren(this Component self, List<IComponent> children)
    {
        self.Children = children;
        return self;
    } 
}