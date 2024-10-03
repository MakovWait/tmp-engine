namespace Tmp.Core;

public enum Side : long
{
    /// <summary>
    /// <para>Left side, usually used for <see cref="Godot.Control"/> or <see cref="Godot.StyleBox"/>-derived classes.</para>
    /// </summary>
    Left = 0,
    /// <summary>
    /// <para>Top side, usually used for <see cref="Godot.Control"/> or <see cref="Godot.StyleBox"/>-derived classes.</para>
    /// </summary>
    Top = 1,
    /// <summary>
    /// <para>Right side, usually used for <see cref="Godot.Control"/> or <see cref="Godot.StyleBox"/>-derived classes.</para>
    /// </summary>
    Right = 2,
    /// <summary>
    /// <para>Bottom side, usually used for <see cref="Godot.Control"/> or <see cref="Godot.StyleBox"/>-derived classes.</para>
    /// </summary>
    Bottom = 3,
}
