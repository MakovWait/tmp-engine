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

    public void DrawLine(Vector2 from, Vector2 to, Color color, float width=-1)
    {
        var transform = GetFinalTransform();
        if (width < 0)
        {
            transform = new Transform2D(transform.Rotation, new Vector2(1, 1), transform.Skew, transform.Origin);
            from = transform.BasisXform(from);
            to = transform.BasisXform(to);
            width = 1;
        }
        else
        {
            from = transform.BasisXform(from);
            to = transform.BasisXform(to);  
        }
        Raylib.DrawLineEx(transform.Origin + from, transform.Origin + to, width, color);
    }

    public void DrawRect(Rect2I rect, Color color)
    {
        var transform = GetFinalTransform();
        
        Vector2 topLeft = rect.Position;
        var topRight = new Vector2(rect.Position.X + rect.Size.X, rect.Position.Y);
        var bottomRight = new Vector2(rect.Position.X + rect.Size.X, rect.Position.Y + rect.Size.Y);
        var bottomLeft = new Vector2(rect.Position.X, rect.Position.Y + rect.Size.Y);
        
        topLeft = transform.Origin + transform.BasisXform(topLeft);
        topRight = transform.Origin + transform.BasisXform(topRight);
        bottomRight = transform.Origin + transform.BasisXform(bottomRight);
        bottomLeft = transform.Origin + transform.BasisXform(bottomLeft);
        
        Raylib.DrawTriangle(topLeft, bottomLeft, bottomRight, color);
        Raylib.DrawTriangle(topLeft, bottomRight, topRight, color);
    }

    public void DrawTexture(_Texture2D texture, Rect2 source, Rect2 dest, Color color)
    {
        var transform = GetFinalTransform();
        Raylib.DrawTexturePro(texture, source, dest, transform.Origin, transform.Rotation.ToDeg(), color);
    }

    public void DrawFps()
    {
        var transform = GetFinalTransform();
        Raylib.DrawFPS(transform.Origin.X.ToInt(), transform.Origin.Y.ToInt());
    }

    public void SetFinalTransform(Transform2D transform)
    {
        _transform = transform;
    }

    public void OnDraw(Action<IDrawContext> onDraw)
    {
        _onDraw = onDraw;
    }
    
    private Transform2D GetFinalTransform()
    {
        return _transform;
    }
}

public interface IDrawContext
{
    /// <summary>
    /// <para>Draws a line from a 2D point to another, with a given color and width.</para>
    /// <para>If <paramref name="width"/> is negative, this means that when the CanvasItem is scaled, the line will remain thin. If this behavior is not desired, then pass a positive <paramref name="width"/> like <c>1.0</c>.</para>
    /// </summary>
    void DrawLine(Vector2 from, Vector2 to, Color color, float width=-1);
    
    void DrawRect(Rect2I rect, Color color);

    void DrawTexture(_Texture2D texture, Rect2 source, Rect2 dest, Color color);
    
    void DrawFps();
}