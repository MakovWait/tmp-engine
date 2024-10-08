using System.Collections;
using System.Diagnostics;
using Tmp.HotReload.Components;

namespace Tmp.Core.Redot;

public readonly record struct NoProps {}

public class Component : Component<NoProps>
{
    public Component() : base()
    {
        
    }
    
    public Component(Action<Self> build) : base(build)
    {
    }

    public Component(Func<Self, IComponent> build) : base(build)
    {
        
    }
    
    public Component(Func<Self, IEnumerable<IComponent>> build) : base(build)
    {
        
    }
}

public class Component<T>(Func<Component<T>.Self, IEnumerable<IComponent>> build) 
    : IComponent, IEnumerable<IFakeEnumerable>, IFakeEnumerable
{
    public readonly List<IComponent> Children = [];
    public Func<T> Props { get; init; } = () => default!;
    private PropsCache _propsCache;

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
        Children.Add(outerChild);
    }

    IEnumerable<IComponent> IComponent.Build(Node node)
    {
        _propsCache = new PropsCache(this);
        _propsCache.Update();
        
        var self = new Self(this, node);
        var children = build(self);
        self.On<HotReloadUpdateApplication>(() =>
        {
            _propsCache.Update();
        });
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
    
    public class PropsCache(Component<T> component)
    {
        private T _value;

        public void Update()
        {
            _value = component.Props();
        }
        
        public T Value()
        {
            return _value;
        }
    }
    
    public class Self(Component<T> component, Node @unchecked)
    {
        public IEnumerable<IComponent> Children
        {
            get
            {
                AssertNodeIsBuilding();
                return component.Children;
            }
        }

        public T Props => component._propsCache.Value();

        private readonly Node @unchecked = @unchecked;
        
        [Obsolete("Not really obsolete but wanna make an accent that using it is not welcome")]
        public Node Unchecked { get; } = @unchecked;

        public void SetId(string id)
        {
            AssertNodeIsBuilding();
            @unchecked.SetId(id);
        }

        public RuntimeNodeRef GetNodeById(string id)
        {
            AssertNodeIsReady();
            return @unchecked.GetNodeById(id);
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
        
        public void UseEffect(Func<Action> effect, IEffectDependency deps)
        {
            AssertNodeIsBuilding();
            @unchecked.UseEffect(effect, deps);
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
