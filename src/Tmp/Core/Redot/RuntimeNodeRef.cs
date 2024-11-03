namespace Tmp.Core.Redot;

public readonly struct RuntimeNodeRef(Node node)
{
    private readonly Node node = node;
    
    [Obsolete("Not really obsolete but wanna make an accent that using it is not welcome")]
    public Node Unchecked { get; } = node;

    public void CreateChild(IComponent component)
    {
        node.CallDeferred(t => t.CreateChild(component));
    }
    
    public void DecorateUp(IComponent component)
    {
        node.CallDeferred(t => t.DecorateUp(component));
    }
    
    public void DecorateDown(IComponent component)
    {
        node.CallDeferred(t => t.DecorateDown(component));
    }
    
    public void ReplaceWith(IComponent component)
    {
        node.CallDeferred(t => t.Replace(component));
    }
    
    public void Undecorate()
    {
        node.CallDeferred(t => t.Undecorate());
    }
    
    public void QueueFree()
    {
        node.QueueFree();
    }
}