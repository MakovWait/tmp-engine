using System.Diagnostics;
using Raylib_cs;
using Tmp.Asset.BuiltIn.Texture;
using Tmp.Core.Comp;
using Tmp.Math;
using Tmp.Render.Util;

namespace Tmp.Render;

public class SubViewport : IViewport
{
    private readonly SubViewports _subViewports;
    private readonly CanvasLayers _canvasLayers = new();
    private readonly Canvas _canvas = new();
    private readonly Camera2D _camera = new();
    private readonly ContainerItem _containerItem;
    private readonly SubViewportTexture _texture;
    
    private Vector2I _size;
    
    public Vector2I Size => _size;
    public _RenderTexture2D RenderTarget => _texture.Target;
    public ITexture2D Texture => _texture;
    
    public SubViewport(Vector2I size)
    {
        _size = size;
        
        _texture = new SubViewportTexture(this);
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
        _texture.Load(_size);
    }

    public void Unload()
    {
        _texture.Unload();
    }

    public void Resize(Vector2I size)
    {
        _size = size;
        _texture.Reload(_size);
    }

    public void Draw()
    {
        _subViewports.Draw();

        Raylib.BeginTextureMode(RenderTarget);
        Raylib.ClearBackground(Color.Black);

        Raylib.BeginMode2D(_camera);
        _canvas.Draw();
        Raylib.EndMode2D();

        _canvasLayers.Draw();

        Raylib.EndTextureMode();
    }

    internal void BindTo(INodeInit self)
    {
        self.CreateContext<ICanvasItemContainer>(_canvas);
        self.CreateContext<ICanvasLayerContainer>(_canvasLayers);
        self.CreateContext<ISubViewportContainer>(_subViewports);
        self.CreateContext<ICamera2D>(_camera);
        self.CreateContext<IViewport>(this);
        
        Load();
        self.OnLateCleanup(Unload);
    }

    public class SubViewportTexture(SubViewport subViewport) : ITexture2D
    {
        private _RenderTexture2D? _target;
        public _RenderTexture2D Target => _target!.Value;
        
        public void Reload(Vector2I size)
        {
            Unload();
            Load(size);
        }

        public void Unload()
        {
            if (_target != null)
            {
                Raylib.UnloadRenderTexture(_target.Value);
            }
        }

        public void Load(Vector2I size)
        {
            _target = Raylib.LoadRenderTexture(size.X, size.Y);
        }
        
        public void Draw(IDrawContext ctx, Vector2 position, Color modulate)
        {
            ctx.DrawTextureRectRegion(TargetTexture, Dest, Source, modulate);
        }

        public void DrawTextureRect(IDrawContext ctx, Rect2 rect, Color modulate)
        {
            ctx.DrawTextureRectRegion(TargetTexture, rect, Source, modulate);
        }

        public void DrawTextureRectRegion(IDrawContext ctx, Rect2 rect, Rect2 srcRect, Color modulate)
        {
            ctx.DrawTextureRectRegion(TargetTexture, rect, Source, modulate);
        }

        private Rect2 Source => new(0, 0, Target.Texture.Width, -Target.Texture.Height);
        private Rect2 Dest => new(0, 0, Target.Texture.Width, Target.Texture.Height);
        private _Texture2D TargetTexture => Target.Texture;
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