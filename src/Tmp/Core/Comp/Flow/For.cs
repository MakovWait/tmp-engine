using System.Collections;
using System.Diagnostics;

namespace Tmp.Core.Comp.Flow;

public class For<T> : Component where T : For<T>.IItem
{
    public required ReactiveList<T> In { get; init; }

    public required Func<T, int, IComponent> Render { get; init; }

    private bool _queuedToUpdate;
    
    protected override Components Init(INodeInit self)
    {
        Dictionary<string, Node> nodes = new();
        Dictionary<string, Node> nodesCopy = new();

        In.Changed += UpdateDeferred;
        self.OnCleanup(() => In.Changed -= UpdateDeferred);

        return [];
    
        void UpdateDeferred()
        {
            if (_queuedToUpdate) return;
            _queuedToUpdate = true;
            self.CallDeferred(Update);
        }
        
        void Update()
        {
            var items = In;
            
            foreach (var node in nodes)
            {
                nodesCopy.Add(node.Key, node.Value);
                Self.RemoveChild(node.Value);
            }
            nodes.Clear();
        
            var idx = 0;
            foreach (var item in items)
            {
                if (nodesCopy.TryGetValue(item.Key, out var n))
                {
                    RegisterNode(item.Key, n);
                    nodesCopy.Remove(item.Key);
                    Self.AddChild(n);
                }
                else
                {
                    var node = CreateChildAndMount(Render(item, idx));
                    RegisterNode(item.Key, node);
                }
                idx++;
            }
        
            foreach (var node in nodesCopy.Values)
            {
                node.Free();
            }
            nodesCopy.Clear();
            
            _queuedToUpdate = false;
        }
        
        void RegisterNode(string key, Node node)
        {
            Debug.Assert(!nodes.ContainsKey(key));
            nodes.Add(key, node);
        }
    }

    public interface IItem
    {
        string Key { get; }
    }
}

public class ReactiveList<T> : IEnumerable<T>
{
    public event Action? Changed;

    private readonly List<T> _items = [];

    public int Count => _items.Count;
    
    public void Add(T item)
    {
        _items.Add(item);
        Changed?.Invoke();
    }
        
    public void Remove(T item)
    {
        _items.Remove(item);
        Changed?.Invoke();
    }
    
    public void Clear()
    {
        _items.Clear();
        Changed?.Invoke();
    }
    
    public List<T>.Enumerator GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
