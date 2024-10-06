namespace Tmp.Core.Redot;

public class Tree
{
    private readonly Node root;
    private readonly Node hiddenRoot;
    private readonly FreeQueue freeQueue = new();
    private readonly Queue<Action> callQueue = new();
    private readonly Queue<Action> callDeferredQueue = new();
    private readonly IdNodeDict idNodeDict = new();

    internal IIdNodeDict IdNodeDict => idNodeDict;

    public Tree()
    {
        hiddenRoot = new Node(this);
        root = new Node(this);
        hiddenRoot.AddChild(root);
        idNodeDict.Put("root", root);
    }
    
    public void Update()
    {
        while (callQueue.Count > 0)
        {
            callQueue.Dequeue()();
        }

        while (callDeferredQueue.Count > 0)
        {
            callDeferredQueue.Dequeue()();
        }

        freeQueue.DequeueAll();
    }
    
    public void AttachToRoot(IComponent component)
    {
        root.CreateChild(component);
    }
    
    public void DecorateRootUp(IComponent component)
    {
        root.DecorateUp(component);
    }

    public void Free()
    {
        hiddenRoot.SafeFree();
    }
    
    public void Call<T>() where T : new()
    {
        hiddenRoot.Call(new T());
    }
    
    public void Call<T>(T state)
    {
        hiddenRoot.Call(state);
    }

    internal void CallDeferred(Action action)
    {
        callDeferredQueue.Enqueue(action);
    }
    
    internal void QueueFree(Node node)
    {
        freeQueue.Add(node);
    }
}

internal class FreeQueue
{
    private readonly Queue<Node> nodes = new();
    
    public void Add(Node node)
    {
        nodes.Enqueue(node);
    }

    public void DequeueAll()
    {
        while (nodes.Count > 0)
        {
            var node = nodes.Dequeue();
            node.SafeFree();
        }
    }
}

internal interface IIdNodeDict
{
    Node Get(string id);
    
    void Put(string id, Node node);
    
    void Remove(string id);
}

internal class IdNodeDict : IIdNodeDict
{
    private readonly Dictionary<string, Node> dict = new();

    public Node Get(string id)
    {
        return dict[id];
    }

    public void Put(string id, Node node)
    {
        if (!dict.TryAdd(id, node))
        {
            throw new Exception($"Node with id {id} is already registered!!!");
        }
    }
    
    public void Remove(string id)
    {
        dict.Remove(id);
    }
}