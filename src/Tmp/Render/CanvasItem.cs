using System.Diagnostics;
using Raylib_cs;
using Tmp.Math;
using Tmp.Render.Util;

namespace Tmp.Render;

public class CanvasItem : ICanvasItemContainer, IDrawContext
{
    private CanvasItem? _parent;
    private Transform2D _transform = Transform2D.Identity;

    private readonly List<CanvasItem> _children = [];
    private Action<IDrawContext>? _onDraw;

    public void AddChild(CanvasItem child)
    {
        Debug.Assert(child._parent == null);
        _children.Add(child);
        child._parent = this;
    }

    public void RemoveChild(CanvasItem child)
    {
        Debug.Assert(child._parent == this);
        _children.Remove(child);
        child._parent = null;
    }

    public void Draw()
    {
        _onDraw?.Invoke(this);
        foreach (var canvasItem in _children)
        {
            canvasItem.Draw();
        }
    }

    public void DrawRect(Rect2I rect, Color color)
    {
        var transform = GetGlobalTransform();
        transform = transform.TranslatedLocal(rect.Position);
        Raylib.DrawRectanglePro(rect, transform.Origin, transform.Rotation.ToDeg(), color);
    }

    public void DrawTexture(_Texture2D texture, Rect2 source, Rect2 dest, Color color)
    {
        var transform = GetGlobalTransform();
        Raylib.DrawTexturePro(texture, source, dest, transform.Origin, transform.Rotation.ToDeg(), color);
    }

    public void DrawFps()
    {
        var transform = GetGlobalTransform();
        Raylib.DrawFPS(transform.Origin.X.ToInt(), transform.Origin.Y.ToInt());
    }

    public void SetGlobalTransform(Transform2D transform)
    {
        _transform = transform;
    }

    public void OnDraw(Action<IDrawContext> onDraw)
    {
        _onDraw = onDraw;
    }
    
    private Transform2D GetGlobalTransform()
    {
        return _transform;
        // if (_parent == null)
        // {
        //     return _transform;
        // }
        // else
        // {
        //     return _parent.GetGlobalTransform() * _transform;
        // }
    }
}

public interface IDrawContext
{
    void DrawRect(Rect2I rect, Color color);

    void DrawTexture(_Texture2D texture, Rect2 source, Rect2 dest, Color color);
    
    void DrawFps();
}