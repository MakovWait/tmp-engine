using System.Diagnostics.CodeAnalysis;
using Tmp.Core;

namespace Tmp.Math;

/// <summary>
/// 2D axis-aligned bounding box. Rect2 consists of a position, a size, and
/// several utility functions. It is typically used for fast overlap tests.
/// </summary>
[Serializable]
public struct Rect2 : IEquatable<Rect2>
{
    private Vector2 _position;
    private Vector2 _size;

    /// <summary>
    /// Beginning corner. Typically has values lower than <see cref="P:Godot.Rect2.End" />.
    /// </summary>
    /// <value>Directly uses a private field.</value>
    public Vector2 Position
    {
        readonly get => this._position;
        set => this._position = value;
    }

    /// <summary>
    /// Size from <see cref="P:Godot.Rect2.Position" /> to <see cref="P:Godot.Rect2.End" />. Typically all components are positive.
    /// If the size is negative, you can use <see cref="M:Godot.Rect2.Abs" /> to fix it.
    /// </summary>
    /// <value>Directly uses a private field.</value>
    public Vector2 Size
    {
        readonly get => this._size;
        set => this._size = value;
    }

    /// <summary>
    /// Ending corner. This is calculated as <see cref="P:Godot.Rect2.Position" /> plus <see cref="P:Godot.Rect2.Size" />.
    /// Setting this value will change the size.
    /// </summary>
    /// <value>
    /// Getting is equivalent to <paramref name="value" /> = <see cref="P:Godot.Rect2.Position" /> + <see cref="P:Godot.Rect2.Size" />,
    /// setting is equivalent to <see cref="P:Godot.Rect2.Size" /> = <paramref name="value" /> - <see cref="P:Godot.Rect2.Position" />
    /// </value>
    public Vector2 End
    {
        readonly get => this._position + this._size;
        set => this._size = value - this._position;
    }

    /// <summary>
    /// The area of this <see cref="T:Godot.Rect2" />.
    /// See also <see cref="M:Godot.Rect2.HasArea" />.
    /// </summary>
    public readonly float Area => this._size.X * this._size.Y;

    /// <summary>
    /// Returns a <see cref="T:Godot.Rect2" /> with equivalent position and size, modified so that
    /// the top-left corner is the origin and width and height are positive.
    /// </summary>
    /// <returns>The modified <see cref="T:Godot.Rect2" />.</returns>
    public readonly Rect2 Abs() => new Rect2(this.End.Min(this._position), this._size.Abs());

    /// <summary>
    /// Returns the intersection of this <see cref="T:Godot.Rect2" /> and <paramref name="b" />.
    /// If the rectangles do not intersect, an empty <see cref="T:Godot.Rect2" /> is returned.
    /// </summary>
    /// <param name="b">The other <see cref="T:Godot.Rect2" />.</param>
    /// <returns>
    /// The intersection of this <see cref="T:Godot.Rect2" /> and <paramref name="b" />,
    /// or an empty <see cref="T:Godot.Rect2" /> if they do not intersect.
    /// </returns>
    public readonly Rect2 Intersection(Rect2 b)
    {
        Rect2 b1 = b;
        if (!this.Intersects(b1))
            return new Rect2();
        b1._position = b._position.Max(this._position);
        Vector2 vector2 = b._position + b._size;
        Vector2 with = this._position + this._size;
        b1._size = vector2.Min(with) - b1._position;
        return b1;
    }

    /// <summary>
    /// Returns <see langword="true" /> if this <see cref="T:Godot.Rect2" /> is finite, by calling
    /// <see cref="M:Godot.Mathf.IsFinite(System.Single)" /> on each component.
    /// </summary>
    /// <returns>Whether this vector is finite or not.</returns>
    public bool IsFinite() => this._position.IsFinite() && this._size.IsFinite();

    /// <summary>
    /// Returns <see langword="true" /> if this <see cref="T:Godot.Rect2" /> completely encloses another one.
    /// </summary>
    /// <param name="b">The other <see cref="T:Godot.Rect2" /> that may be enclosed.</param>
    /// <returns>
    /// A <see langword="bool" /> for whether or not this <see cref="T:Godot.Rect2" /> encloses <paramref name="b" />.
    /// </returns>
    public readonly bool Encloses(Rect2 b)
    {
        return (double)b._position.X >= (double)this._position.X && (double)b._position.Y >= (double)this._position.Y &&
               (double)b._position.X + (double)b._size.X <= (double)this._position.X + (double)this._size.X &&
               (double)b._position.Y + (double)b._size.Y <= (double)this._position.Y + (double)this._size.Y;
    }

    /// <summary>
    /// Returns this <see cref="T:Godot.Rect2" /> expanded to include a given point.
    /// </summary>
    /// <param name="to">The point to include.</param>
    /// <returns>The expanded <see cref="T:Godot.Rect2" />.</returns>
    public readonly Rect2 Expand(Vector2 to)
    {
        Rect2 rect2 = this;
        Vector2 position = rect2._position;
        Vector2 vector2 = rect2._position + rect2._size;
        if ((double)to.X < (double)position.X)
            position.X = to.X;
        if ((double)to.Y < (double)position.Y)
            position.Y = to.Y;
        if ((double)to.X > (double)vector2.X)
            vector2.X = to.X;
        if ((double)to.Y > (double)vector2.Y)
            vector2.Y = to.Y;
        rect2._position = position;
        rect2._size = vector2 - position;
        return rect2;
    }

    /// <summary>
    /// Returns the center of the <see cref="T:Godot.Rect2" />, which is equal
    /// to <see cref="P:Godot.Rect2.Position" /> + (<see cref="P:Godot.Rect2.Size" /> / 2).
    /// </summary>
    /// <returns>The center.</returns>
    public readonly Vector2 GetCenter() => this._position + this._size * 0.5f;

    /// <summary>
    /// Returns a copy of the <see cref="T:Godot.Rect2" /> grown by the specified amount
    /// on all sides.
    /// </summary>
    /// <seealso cref="M:Godot.Rect2.GrowIndividual(System.Single,System.Single,System.Single,System.Single)" />
    /// <seealso cref="M:Godot.Rect2.GrowSide(Godot.Side,System.Single)" />
    /// <param name="by">The amount to grow by.</param>
    /// <returns>The grown <see cref="T:Godot.Rect2" />.</returns>
    public readonly Rect2 Grow(float by)
    {
        Rect2 rect2 = this;
        rect2._position.X -= by;
        rect2._position.Y -= by;
        rect2._size.X += by * 2f;
        rect2._size.Y += by * 2f;
        return rect2;
    }

    /// <summary>
    /// Returns a copy of the <see cref="T:Godot.Rect2" /> grown by the specified amount
    /// on each side individually.
    /// </summary>
    /// <seealso cref="M:Godot.Rect2.Grow(System.Single)" />
    /// <seealso cref="M:Godot.Rect2.GrowSide(Godot.Side,System.Single)" />
    /// <param name="left">The amount to grow by on the left side.</param>
    /// <param name="top">The amount to grow by on the top side.</param>
    /// <param name="right">The amount to grow by on the right side.</param>
    /// <param name="bottom">The amount to grow by on the bottom side.</param>
    /// <returns>The grown <see cref="T:Godot.Rect2" />.</returns>
    public readonly Rect2 GrowIndividual(float left, float top, float right, float bottom)
    {
        Rect2 rect2 = this;
        rect2._position.X -= left;
        rect2._position.Y -= top;
        rect2._size.X += left + right;
        rect2._size.Y += top + bottom;
        return rect2;
    }

    /// <summary>
    /// Returns a copy of the <see cref="T:Godot.Rect2" /> grown by the specified amount
    /// on the specified <see cref="T:Godot.Side" />.
    /// </summary>
    /// <seealso cref="M:Godot.Rect2.Grow(System.Single)" />
    /// <seealso cref="M:Godot.Rect2.GrowIndividual(System.Single,System.Single,System.Single,System.Single)" />
    /// <param name="side">The side to grow.</param>
    /// <param name="by">The amount to grow by.</param>
    /// <returns>The grown <see cref="T:Godot.Rect2" />.</returns>
    public readonly Rect2 GrowSide(Side side, float by)
    {
        Rect2 rect2 = this;
        rect2 = rect2.GrowIndividual(side == Side.Left ? by : 0.0f, Side.Top == side ? by : 0.0f, Side.Right == side ? by : 0.0f, Side.Bottom == side ? by : 0.0f);
        return rect2;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the <see cref="T:Godot.Rect2" /> has
    /// area, and <see langword="false" /> if the <see cref="T:Godot.Rect2" />
    /// is linear, empty, or has a negative <see cref="P:Godot.Rect2.Size" />.
    /// See also <see cref="P:Godot.Rect2.Area" />.
    /// </summary>
    /// <returns>
    /// A <see langword="bool" /> for whether or not the <see cref="T:Godot.Rect2" /> has area.
    /// </returns>
    public readonly bool HasArea() => (double)this._size.X > 0.0 && (double)this._size.Y > 0.0;

    /// <summary>
    /// Returns <see langword="true" /> if the <see cref="T:Godot.Rect2" /> contains a point,
    /// or <see langword="false" /> otherwise.
    /// </summary>
    /// <param name="point">The point to check.</param>
    /// <returns>
    /// A <see langword="bool" /> for whether or not the <see cref="T:Godot.Rect2" /> contains <paramref name="point" />.
    /// </returns>
    public readonly bool HasPoint(Vector2 point)
    {
        return (double)point.X >= (double)this._position.X && (double)point.Y >= (double)this._position.Y && (double)point.X < (double)this._position.X + (double)this._size.X &&
               (double)point.Y < (double)this._position.Y + (double)this._size.Y;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the <see cref="T:Godot.Rect2" /> overlaps with <paramref name="b" />
    /// (i.e. they have at least one point in common).
    /// 
    /// If <paramref name="includeBorders" /> is <see langword="true" />,
    /// they will also be considered overlapping if their borders touch,
    /// even without intersection.
    /// </summary>
    /// <param name="b">The other <see cref="T:Godot.Rect2" /> to check for intersections with.</param>
    /// <param name="includeBorders">Whether or not to consider borders.</param>
    /// <returns>A <see langword="bool" /> for whether or not they are intersecting.</returns>
    public readonly bool Intersects(Rect2 b, bool includeBorders = false)
    {
        if (includeBorders)
        {
            if ((double)this._position.X > (double)b._position.X + (double)b._size.X || (double)this._position.X + (double)this._size.X < (double)b._position.X ||
                (double)this._position.Y > (double)b._position.Y + (double)b._size.Y || (double)this._position.Y + (double)this._size.Y < (double)b._position.Y)
                return false;
        }
        else if ((double)this._position.X >= (double)b._position.X + (double)b._size.X || (double)this._position.X + (double)this._size.X <= (double)b._position.X ||
                 (double)this._position.Y >= (double)b._position.Y + (double)b._size.Y || (double)this._position.Y + (double)this._size.Y <= (double)b._position.Y)
            return false;
        return true;
    }

    /// <summary>
    /// Returns a larger <see cref="T:Godot.Rect2" /> that contains this <see cref="T:Godot.Rect2" /> and <paramref name="b" />.
    /// </summary>
    /// <param name="b">The other <see cref="T:Godot.Rect2" />.</param>
    /// <returns>The merged <see cref="T:Godot.Rect2" />.</returns>
    public readonly Rect2 Merge(Rect2 b)
    {
        Rect2 rect2;
        rect2._position = b._position.Min(this._position);
        rect2._size = (b._position + b._size).Max(this._position + this._size);
        rect2._size -= rect2._position;
        return rect2;
    }

    /// <summary>
    /// Constructs a <see cref="T:Godot.Rect2" /> from a position and size.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="size">The size.</param>
    public Rect2(Vector2 position, Vector2 size)
    {
        this._position = position;
        this._size = size;
    }

    /// <summary>
    /// Constructs a <see cref="T:Godot.Rect2" /> from a position, width, and height.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public Rect2(Vector2 position, float width, float height)
    {
        this._position = position;
        this._size = new Vector2(width, height);
    }

    /// <summary>
    /// Constructs a <see cref="T:Godot.Rect2" /> from x, y, and size.
    /// </summary>
    /// <param name="x">The position's X coordinate.</param>
    /// <param name="y">The position's Y coordinate.</param>
    /// <param name="size">The size.</param>
    public Rect2(float x, float y, Vector2 size)
    {
        this._position = new Vector2(x, y);
        this._size = size;
    }

    /// <summary>
    /// Constructs a <see cref="T:Godot.Rect2" /> from x, y, width, and height.
    /// </summary>
    /// <param name="x">The position's X coordinate.</param>
    /// <param name="y">The position's Y coordinate.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public Rect2(float x, float y, float width, float height)
    {
        this._position = new Vector2(x, y);
        this._size = new Vector2(width, height);
    }

    /// <summary>
    /// Returns <see langword="true" /> if the
    /// <see cref="T:Godot.Rect2" />s are exactly equal.
    /// Note: Due to floating-point precision errors, consider using
    /// <see cref="M:Godot.Rect2.IsEqualApprox(Godot.Rect2)" /> instead, which is more reliable.
    /// </summary>
    /// <param name="left">The left rect.</param>
    /// <param name="right">The right rect.</param>
    /// <returns>Whether or not the rects are exactly equal.</returns>
    public static bool operator ==(Rect2 left, Rect2 right) => left.Equals(right);

    /// <summary>
    /// Returns <see langword="true" /> if the
    /// <see cref="T:Godot.Rect2" />s are not equal.
    /// Note: Due to floating-point precision errors, consider using
    /// <see cref="M:Godot.Rect2.IsEqualApprox(Godot.Rect2)" /> instead, which is more reliable.
    /// </summary>
    /// <param name="left">The left rect.</param>
    /// <param name="right">The right rect.</param>
    /// <returns>Whether or not the rects are not equal.</returns>
    public static bool operator !=(Rect2 left, Rect2 right) => !left.Equals(right);

    /// <summary>
    /// Returns <see langword="true" /> if this rect and <paramref name="obj" /> are equal.
    /// </summary>
    /// <param name="obj">The other object to compare.</param>
    /// <returns>Whether or not the rect and the other object are exactly equal.</returns>
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Rect2 other && this.Equals(other);
    }

    /// <summary>
    /// Returns <see langword="true" /> if this rect and <paramref name="other" /> are equal.
    /// </summary>
    /// <param name="other">The other rect to compare.</param>
    /// <returns>Whether or not the rects are exactly equal.</returns>
    public readonly bool Equals(Rect2 other)
    {
        return this._position.Equals(other._position) && this._size.Equals(other._size);
    }

    /// <summary>
    /// Returns <see langword="true" /> if this rect and <paramref name="other" /> are approximately equal,
    /// by running <see cref="M:Godot.Vector2.IsEqualApprox(Godot.Vector2)" /> on each component.
    /// </summary>
    /// <param name="other">The other rect to compare.</param>
    /// <returns>Whether or not the rects are approximately equal.</returns>
    public readonly bool IsEqualApprox(Rect2 other)
    {
        return this._position.IsEqualApprox(other._position) && this._size.IsEqualApprox(other.Size);
    }

    public static implicit operator _Rectangle(Rect2 self) => new(self.Position.X, self.Position.Y, self.Size.X, self.Size.Y);
    
    /// <summary>
    /// Serves as the hash function for <see cref="T:Godot.Rect2" />.
    /// </summary>
    /// <returns>A hash code for this rect.</returns>
    public override readonly int GetHashCode()
    {
        return HashCode.Combine<Vector2, Vector2>(this._position, this._size);
    }

    /// <summary>
    /// Converts this <see cref="T:Godot.Rect2" /> to a string.
    /// </summary>
    /// <returns>A string representation of this rect.</returns>
    public override readonly string ToString() => this.ToString((string)null);

    /// <summary>
    /// Converts this <see cref="T:Godot.Rect2" /> to a string with the given <paramref name="format" />.
    /// </summary>
    /// <returns>A string representation of this rect.</returns>
    public readonly string ToString(string? format)
    {
        return this._position.ToString(format) + ", " + this._size.ToString(format);
    }
}