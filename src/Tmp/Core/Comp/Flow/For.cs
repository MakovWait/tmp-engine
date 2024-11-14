using System.Collections;
using System.Diagnostics;

namespace Tmp.Core.Comp.Flow;

public class For<T> : Component where T : For<T>.IItem
{
    public required ReactiveList<T> In { get; init; }

    public required Func<T, int, IComponent> Render { get; init; }
    
    protected override Components Init(INodeInit self)
    {
        var sIn = In.ToSignalFrom(self);
        
        Dictionary<string, Node> nodes = new();
        Dictionary<string, Node> nodesCopy = new();
        
        self.UseEffect(() =>
        {
            var items = sIn.Value;
            
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
        });

        return [];
        
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
    private event Action? Changed;

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
    
    public ISignal<ReactiveList<T>> ToSignalFrom(INodeInit node)
    {
        var signal = node.CreateSignal(set =>
        {
            var trigger = () => set(this);
            Changed += trigger;
            return () => Changed -= trigger;
        }, this);
        return signal;
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
