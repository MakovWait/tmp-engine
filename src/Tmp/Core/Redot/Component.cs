using System.Collections;
using System.Diagnostics;

namespace Tmp.Core.Redot;

public class Component(Func<Component.Self, IEnumerable<IComponent>> build) 
    : IComponent, IEnumerable<IFakeEnumerable>, IFakeEnumerable
{
    private readonly List<IComponent> outerChildren = [];

    public Component() : this(self => self.Children)
    {
    }
    
    public Component(Action<Self> build) : this(node =>
    {
        build(node);
        return node.Children;
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
    
    public class Self(Node @unchecked, IEnumerable<IComponent> outerChildren)
    {
        public IEnumerable<IComponent> Children
        {
            get
            {
                AssertNodeIsBuilding();
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

        public void On<T>(T callback) where T : Delegate
        {
            AssertNodeIsBuilding();
            @unchecked.On(callback);
        }
        
        public void After<T>(T callback) where T : Delegate
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
