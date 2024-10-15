using System.Diagnostics;
using Raylib_cs;
using Tmp.Core.Redot;
using Tmp.Math;
using Tmp.Render.Util;
using Tmp.Resource.BuiltIn.Texture;

namespace Tmp.Render;

public class SubViewport : IViewport
{
    private readonly SubViewports _subViewports;
    private readonly CanvasLayers _canvasLayers = new();
    private readonly Canvas _canvas = new();
    private readonly Camera2D _camera = new();
    private readonly ContainerItem _containerItem;
    private readonly Vector2I _size;
    private readonly IDeferredValueMut<Texture> _texture;

    private _RenderTexture2D _target;

    public Vector2I Size => _size;
    private Vector2I ParentViewportSize => _containerItem.Parent!.Size;

    public SubViewport(Vector2I size, IDeferredValueMut<Texture> texture)
    {
        _size = size;
        _texture = texture;
        
        _containerItem = new ContainerItem(this);
        _subViewports = new SubViewports(this);
    }
    
    public void AddTo(ISubViewportContainer container)
    {
        container.Add(_containerItem);
    }

    public void RemoveFrom(ISubViewportContainer container)
    {
        container.Remove(_containerItem);
    }

    public void Load()
    {
        _target = Raylib.LoadRenderTexture(_size.X, _size.Y);
        _texture.Set(new Texture(this));
    }

    public void Unload()
    {
        Raylib.UnloadRenderTexture(_target);
    }

    public void Draw()
    {
        _subViewports.Draw();

        Raylib.BeginTextureMode(_target);
        Raylib.ClearBackground(Color.Black);

        Raylib.BeginMode2D(_camera);
        _canvas.Draw();
        Raylib.EndMode2D();

        _canvasLayers.Draw();

        Raylib.EndTextureMode();
    }

    internal void CreateContext(Component.Self self)
    {
        self.CreateContext<ICanvasItemContainer>(_canvas);
        self.CreateContext<ICanvasLayerContainer>(_canvasLayers);
        self.CreateContext<ISubViewportContainer>(_subViewports);
        self.CreateContext<ICamera2D>(_camera);
        self.CreateContext<IViewport>(this);
    }

    public class Texture(SubViewport subViewport) : ITexture2D
    {
        public void Draw(IDrawContext ctx, Vector2 position, Color modulate)
        {
            ctx.DrawTextureRectRegion(TargetTexture, new Rect2(position, subViewport.ParentViewportSize), Source, modulate);
        }

        public void DrawTextureRect(IDrawContext ctx, Rect2 rect, Color modulate)
        {
            ctx.DrawTextureRectRegion(TargetTexture, rect, Source, modulate);
        }

        public void DrawTextureRectRegion(IDrawContext ctx, Rect2 rect, Rect2 srcRect, Color modulate)
        {
            ctx.DrawTextureRectRegion(TargetTexture, rect, Source, modulate);
        }

        private Rect2 Source => new(0, 0, subViewport._target.Texture.Width, -subViewport._target.Texture.Height);
        public _Texture2D TargetTexture => subViewport._target.Texture;
    }

    private class ContainerItem(SubViewport self) : ISubViewportContainer.IItem
    {
        public IViewport? Parent { get; private set; }
        
        public void AttachToParent(IViewport parent)
        {
            Debug.Assert(Parent is null);
            Parent = parent;
        }

        public void ClearParent()
        {
            Debug.Assert(Parent is not null);
            Parent = null;
        }

        public void Draw()
        {
            self.Draw();
        }
    }
}