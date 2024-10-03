using Tmp.Render.Util;

namespace Tmp.Render;

internal class Camera2D : ICamera2D
{
    public float Rotation { get; set; }
    public float Zoom { get; set; } = 1f;
    public Vector2 Offset { get; set; }
    public Vector2 Target { get; set; }

    private _Camera2D Camera => new()
    {
        Rotation = Rotation,
        Zoom = Zoom,
        Offset = Offset,
        Target = Target
    };
    
    public static implicit operator _Camera2D(Camera2D self) => self.Camera;
}