using System.Collections;
using Tmp.Core.Comp.Flow;

namespace Tmp.Core.Comp;

public class Component : IComponent, IEnumerable<IComponent>
{
    public Components Children { get; init; } = [];
    public string? Name { get; init; } = null;

    private Node? _self;
    private Tree? _tree;

    protected Tree Tree => _tree!;
    protected Node Self => _self!;

    public Node Build(Tree tree, Node? parent)
    {
        _tree = tree;
        _self = tree.CreateNode(Name ?? "Node");
        parent?.AddChild(_self);
        
        _self.Init(_ =>
        {
            CreateChildren(Init(_self));
        });
        
        return _self;
    }

    protected virtual Components Init(INodeInit self)
    {
        return Children;
    }
    
    protected void CreateChildren(Components components)
    {
        foreach (var component in components)
        {
            component.Build(Tree, Self);
        }
    }
    
    protected void CreateChildrenAndMount(Components components)
    {
        foreach (var component in components)
        {
            var child = component.Build(Tree, Self);
            child.Mount();
        }
    }

    protected void ClearChildren()
    {
        Self.ClearChildren();
    }
    
    protected void ReplaceChildren(Components components)
    {
        ClearChildren();
        CreateChildren(components);
    }
    
    public void Add(IComponent component)
    {
        Children.Add(component);
    }

    IEnumerator<IComponent> IEnumerable<IComponent>.GetEnumerator()
    {
        return Children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Children.GetEnumerator();
    }
}

public class ComponentFunc(Func<INodeInit, Components, Components> init) : Component
{
    public ComponentFunc(Func<INodeInit, Components> init): this((self, _) => init(self))
    {
        
    }
    
    protected override Components Init(INodeInit self)
    {
        return init(self, Children);
    }
}

public class Components : IEnumerable<IComponent>
{
    private List<IComponent> Children { get; init; } = [];
    
    public static implicit operator Components(Component component) => component.AsChildren();
    
    public void Add(IComponent component)
    {
        Children.Add(component);
    }

    public List<IComponent>.Enumerator GetEnumerator()
    {
        return Children.GetEnumerator();
    }
    
    IEnumerator<IComponent> IEnumerable<IComponent>.GetEnumerator()
    {
        return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public static class ComponentEx
{
    public static Components AsChildren(this IComponent self)
    {
        return [self];
    }
    
    public static IComponent If(this IComponent self, ISignal<bool> condition)
    {
        return new Conditional
        {
            When = condition,
            Children = self.AsChildren()
        };
    }
}

public interface IComponent
{
    Node Build(Tree tree, Node? parent);
}

