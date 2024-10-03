namespace Tmp.Render.Util;

public class CanvasLayers : ICanvasLayerContainer
{
    private readonly List<CanvasLayer> _layers = [];

    private bool _isDirty;

    public void Add(CanvasLayer layer)
    {
        _layers.Add(layer);
        QueueSort();
    }
    
    public void Remove(CanvasLayer layer)
    {
        _layers.Remove(layer);
        QueueSort();
    }
    
    public void Draw()
    {
        SortLayers();
        foreach (var layer in _layers)
        {
            layer.Draw();
        }
    }

    private void QueueSort()
    {
        _isDirty = true;
    }

    private void SortLayers()
    {
        if (_isDirty)
        {
            _layers.Sort((l1, l2) => l1.Order - l2.Order);
            _isDirty = false;
        }
    }
}