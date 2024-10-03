namespace Tmp.Core.Redot;

public readonly struct RuntimeNodeRef(Node node)
{
    private readonly Node node = node;
    
    [Obsolete("Not really obsolete but wanna make an accent that using it is not welcome")]
    public Node Unchecked { get; } = node;

    public void CreateChild(Component component)
    {
        node.CallDeferred(t => t.CreateChild(component));
    }
    
    public void DecorateUp(Component component)
    {
        node.CallDeferred(t => t.DecorateUp(component));
    }
    
    public void DecorateDown(Component component)
    {
        node.CallDeferred(t => t.DecorateDown(component));
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