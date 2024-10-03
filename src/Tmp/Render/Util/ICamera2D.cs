namespace Tmp.Render.Util;

public interface ICamera2D
{
    /// <summary>Camera rotation in degrees</summary>
    float Rotation { get; set; }
    
    /// <summary>Camera zoom (scaling), should be 1.0f by default</summary>
    float Zoom { get; set; }
    
    /// <summary>Camera offset (displacement from target)</summary>
    Vector2 Offset { get; set; }
    
    /// <summary>Camera target (rotation and zoom origin)</summary>
    Vector2 Target { get; set; }
}