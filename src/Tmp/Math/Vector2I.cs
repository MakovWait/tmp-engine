using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Tmp.Math;

/// <summary>
/// 2-element structure that can be used to represent 2D grid coordinates or pairs of integers.
/// </summary>
[Serializable]
public struct Vector2I : IEquatable<Vector2I>
{
    /// <summary>
    /// The vector's X component. Also accessible by using the index position <c>[0]</c>.
    /// </summary>
    public int X;

    /// <summary>
    /// The vector's Y component. Also accessible by using the index position <c>[1]</c>.
    /// </summary>
    public int Y;

    private static readonly Vector2I _minValue = new Vector2I(int.MinValue, int.MinValue);
    private static readonly Vector2I _maxValue = new Vector2I(int.MaxValue, int.MaxValue);
    private static readonly Vector2I _zero = new Vector2I(0, 0);
    private static readonly Vector2I _one = new Vector2I(1, 1);
    private static readonly Vector2I _up = new Vector2I(0, -1);
    private static readonly Vector2I _down = new Vector2I(0, 1);
    private static readonly Vector2I _right = new Vector2I(1, 0);
    private static readonly Vector2I _left = new Vector2I(-1, 0);

    /// <summary>Access vector components using their index.</summary>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="index" /> is not 0 or 1.
    /// </exception>
    /// <value>
    /// <c>[0]</c> is equivalent to <see cref="F:Godot.Vector2I.X" />,
    /// <c>[1]</c> is equivalent to <see cref="F:Godot.Vector2I.Y" />.
    /// </value>
    public int this[int index]
    {
        readonly get
        {
            if (index == 0)
                return this.X;
            if (index == 1)
                return this.Y;
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        set
        {
            if (index != 0)
            {
                if (index != 1)
                    throw new ArgumentOutOfRangeException(nameof(index));
                this.Y = value;
            }
            else
                this.X = value;
        }
    }

    /// <summary>Helper method for deconstruction into a tuple.</summary>
    public readonly void Deconstruct(out int x, out int y)
    {
        x = this.X;
        y = this.Y;
    }

    /// <summary>
    /// Returns a new vector with all components in absolute values (i.e. positive).
    /// </summary>
    /// <returns>A vector with <see cref="M:Godot.Mathf.Abs(System.Int32)" /> called on each component.</returns>
    public readonly Vector2I Abs() => new Vector2I(Mathf.Abs(this.X), Mathf.Abs(this.Y));

    /// <summary>
    /// Returns the aspect ratio of this vector, the ratio of <see cref="F:Godot.Vector2I.X" /> to <see cref="F:Godot.Vector2I.Y" />.
    /// </summary>
    /// <returns>The <see cref="F:Godot.Vector2I.X" /> component divided by the <see cref="F:Godot.Vector2I.Y" /> component.</returns>
    public readonly float Aspect() => (float)this.X / (float)this.Y;

    /// <summary>
    /// Returns a new vector with all components clamped between the
    /// components of <paramref name="min" /> and <paramref name="max" /> using
    /// <see cref="M:Godot.Mathf.Clamp(System.Int32,System.Int32,System.Int32)" />.
    /// </summary>
    /// <param name="min">The vector with minimum allowed values.</param>
    /// <param name="max">The vector with maximum allowed values.</param>
    /// <returns>The vector with all components clamped.</returns>
    public readonly Vector2I Clamp(Vector2I min, Vector2I max)
    {
        return new Vector2I(Mathf.Clamp(this.X, min.X, max.X), Mathf.Clamp(this.Y, min.Y, max.Y));
    }

    /// <summary>
    /// Returns a new vector with all components clamped between the
    /// <paramref name="min" /> and <paramref name="max" /> using
    /// <see cref="M:Godot.Mathf.Clamp(System.Int32,System.Int32,System.Int32)" />.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <returns>The vector with all components clamped.</returns>
    public readonly Vector2I Clamp(int min, int max)
    {
        return new Vector2I(Mathf.Clamp(this.X, min, max), Mathf.Clamp(this.Y, min, max));
    }

    /// <summary>
    /// Returns the squared distance between this vector and <paramref name="to" />.
    /// This method runs faster than <see cref="M:Godot.Vector2I.DistanceTo(Godot.Vector2I)" />, so prefer it if
    /// you need to compare vectors or need the squared distance for some formula.
    /// </summary>
    /// <param name="to">The other vector to use.</param>
    /// <returns>The squared distance between the two vectors.</returns>
    public readonly int DistanceSquaredTo(Vector2I to) => (to - this).LengthSquared();

    /// <summary>
    /// Returns the distance between this vector and <paramref name="to" />.
    /// </summary>
    /// <seealso cref="M:Godot.Vector2I.DistanceSquaredTo(Godot.Vector2I)" />
    /// <param name="to">The other vector to use.</param>
    /// <returns>The distance between the two vectors.</returns>
    public readonly float DistanceTo(Vector2I to) => (to - this).Length();

    /// <summary>Returns the length (magnitude) of this vector.</summary>
    /// <seealso cref="M:Godot.Vector2I.LengthSquared" />
    /// <returns>The length of this vector.</returns>
    public readonly float Length() => Mathf.Sqrt((float)(this.X * this.X + this.Y * this.Y));

    /// <summary>
    /// Returns the squared length (squared magnitude) of this vector.
    /// This method runs faster than <see cref="M:Godot.Vector2I.Length" />, so prefer it if
    /// you need to compare vectors or need the squared length for some formula.
    /// </summary>
    /// <returns>The squared length of this vector.</returns>
    public readonly int LengthSquared() => this.X * this.X + this.Y * this.Y;

    /// <summary>
    /// Returns the result of the component-wise maximum between
    /// this vector and <paramref name="with" />.
    /// Equivalent to <c>new Vector2I(Mathf.Max(X, with.X), Mathf.Max(Y, with.Y))</c>.
    /// </summary>
    /// <param name="with">The other vector to use.</param>
    /// <returns>The resulting maximum vector.</returns>
    public readonly Vector2I Max(Vector2I with)
    {
        return new Vector2I(Mathf.Max(this.X, with.X), Mathf.Max(this.Y, with.Y));
    }

    /// <summary>
    /// Returns the result of the component-wise maximum between
    /// this vector and <paramref name="with" />.
    /// Equivalent to <c>new Vector2I(Mathf.Max(X, with), Mathf.Max(Y, with))</c>.
    /// </summary>
    /// <param name="with">The other value to use.</param>
    /// <returns>The resulting maximum vector.</returns>
    public readonly Vector2I Max(int with)
    {
        return new Vector2I(Mathf.Max(this.X, with), Mathf.Max(this.Y, with));
    }

    /// <summary>
    /// Returns the result of the component-wise minimum between
    /// this vector and <paramref name="with" />.
    /// Equivalent to <c>new Vector2I(Mathf.Min(X, with.X), Mathf.Min(Y, with.Y))</c>.
    /// </summary>
    /// <param name="with">The other vector to use.</param>
    /// <returns>The resulting minimum vector.</returns>
    public readonly Vector2I Min(Vector2I with)
    {
        return new Vector2I(Mathf.Min(this.X, with.X), Mathf.Min(this.Y, with.Y));
    }

    /// <summary>
    /// Returns the result of the component-wise minimum between
    /// this vector and <paramref name="with" />.
    /// Equivalent to <c>new Vector2I(Mathf.Min(X, with), Mathf.Min(Y, with))</c>.
    /// </summary>
    /// <param name="with">The other value to use.</param>
    /// <returns>The resulting minimum vector.</returns>
    public readonly Vector2I Min(int with)
    {
        return new Vector2I(Mathf.Min(this.X, with), Mathf.Min(this.Y, with));
    }

    /// <summary>
    /// Returns the axis of the vector's highest value. See <see cref="T:Godot.Vector2I.Axis" />.
    /// If both components are equal, this method returns <see cref="F:Godot.Vector2I.Axis.X" />.
    /// </summary>
    /// <returns>The index of the highest axis.</returns>
    public readonly Vector2I.Axis MaxAxisIndex()
    {
        return this.X >= this.Y ? Vector2I.Axis.X : Vector2I.Axis.Y;
    }

    /// <summary>
    /// Returns the axis of the vector's lowest value. See <see cref="T:Godot.Vector2I.Axis" />.
    /// If both components are equal, this method returns <see cref="F:Godot.Vector2I.Axis.Y" />.
    /// </summary>
    /// <returns>The index of the lowest axis.</returns>
    public readonly Vector2I.Axis MinAxisIndex()
    {
        return this.X >= this.Y ? Vector2I.Axis.Y : Vector2I.Axis.X;
    }

    /// <summary>
    /// Returns a vector with each component set to one or negative one, depending
    /// on the signs of this vector's components, or zero if the component is zero,
    /// by calling <see cref="M:Godot.Mathf.Sign(System.Int32)" /> on each component.
    /// </summary>
    /// <returns>A vector with all components as either <c>1</c>, <c>-1</c>, or <c>0</c>.</returns>
    public readonly Vector2I Sign()
    {
        Vector2I vector2I = this;
        vector2I.X = Mathf.Sign(vector2I.X);
        vector2I.Y = Mathf.Sign(vector2I.Y);
        return vector2I;
    }

    /// <summary>
    /// Returns a new vector with each component snapped to the closest multiple of the corresponding component in <paramref name="step" />.
    /// </summary>
    /// <param name="step">A vector value representing the step size to snap to.</param>
    /// <returns>The snapped vector.</returns>
    public readonly Vector2I Snapped(Vector2I step)
    {
        return new Vector2I((int)Mathf.Snapped((double)this.X, (double)step.X), (int)Mathf.Snapped((double)this.Y, (double)step.Y));
    }

    /// <summary>
    /// Returns a new vector with each component snapped to the closest multiple of <paramref name="step" />.
    /// </summary>
    /// <param name="step">The step size to snap to.</param>
    /// <returns>The snapped vector.</returns>
    public readonly Vector2I Snapped(int step)
    {
        return new Vector2I((int)Mathf.Snapped((double)this.X, (double)step), (int)Mathf.Snapped((double)this.Y, (double)step));
    }

    /// <summary>
    /// Min vector, a vector with all components equal to <see cref="F:System.Int32.MinValue" />. Can be used as a negative integer equivalent of <see cref="P:Godot.Vector2.Inf" />.
    /// </summary>
    /// <value>Equivalent to <c>new Vector2I(int.MinValue, int.MinValue)</c>.</value>
    public static Vector2I MinValue => Vector2I._minValue;

    /// <summary>
    /// Max vector, a vector with all components equal to <see cref="F:System.Int32.MaxValue" />. Can be used as an integer equivalent of <see cref="P:Godot.Vector2.Inf" />.
    /// </summary>
    /// <value>Equivalent to <c>new Vector2I(int.MaxValue, int.MaxValue)</c>.</value>
    public static Vector2I MaxValue => Vector2I._maxValue;

    /// <summary>
    /// Zero vector, a vector with all components set to <c>0</c>.
    /// </summary>
    /// <value>Equivalent to <c>new Vector2I(0, 0)</c>.</value>
    public static Vector2I Zero => Vector2I._zero;

    /// <summary>
    /// One vector, a vector with all components set to <c>1</c>.
    /// </summary>
    /// <value>Equivalent to <c>new Vector2I(1, 1)</c>.</value>
    public static Vector2I One => Vector2I._one;

    /// <summary>
    /// Up unit vector. Y is down in 2D, so this vector points -Y.
    /// </summary>
    /// <value>Equivalent to <c>new Vector2I(0, -1)</c>.</value>
    public static Vector2I Up => Vector2I._up;

    /// <summary>
    /// Down unit vector. Y is down in 2D, so this vector points +Y.
    /// </summary>
    /// <value>Equivalent to <c>new Vector2I(0, 1)</c>.</value>
    public static Vector2I Down => Vector2I._down;

    /// <summary>Right unit vector. Represents the direction of right.</summary>
    /// <value>Equivalent to <c>new Vector2I(1, 0)</c>.</value>
    public static Vector2I Right => Vector2I._right;

    /// <summary>Left unit vector. Represents the direction of left.</summary>
    /// <value>Equivalent to <c>new Vector2I(-1, 0)</c>.</value>
    public static Vector2I Left => Vector2I._left;

    /// <summary>
    /// Constructs a new <see cref="T:Godot.Vector2I" /> with the given components.
    /// </summary>
    /// <param name="x">The vector's X component.</param>
    /// <param name="y">The vector's Y component.</param>
    public Vector2I(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    /// <summary>
    /// Adds each component of the <see cref="T:Godot.Vector2I" />
    /// with the components of the given <see cref="T:Godot.Vector2I" />.
    /// </summary>
    /// <param name="left">The left vector.</param>
    /// <param name="right">The right vector.</param>
    /// <returns>The added vector.</returns>
    public static Vector2I operator +(Vector2I left, Vector2I right)
    {
        left.X += right.X;
        left.Y += right.Y;
        return left;
    }

    /// <summary>
    /// Subtracts each component of the <see cref="T:Godot.Vector2I" />
    /// by the components of the given <see cref="T:Godot.Vector2I" />.
    /// </summary>
    /// <param name="left">The left vector.</param>
    /// <param name="right">The right vector.</param>
    /// <returns>The subtracted vector.</returns>
    public static Vector2I operator -(Vector2I left, Vector2I right)
    {
        left.X -= right.X;
        left.Y -= right.Y;
        return left;
    }

    /// <summary>
    /// Returns the negative value of the <see cref="T:Godot.Vector2I" />.
    /// This is the same as writing <c>new Vector2I(-v.X, -v.Y)</c>.
    /// This operation flips the direction of the vector while
    /// keeping the same magnitude.
    /// </summary>
    /// <param name="vec">The vector to negate/flip.</param>
    /// <returns>The negated/flipped vector.</returns>
    public static Vector2I operator -(Vector2I vec)
    {
        vec.X = -vec.X;
        vec.Y = -vec.Y;
        return vec;
    }

    /// <summary>
    /// Multiplies each component of the <see cref="T:Godot.Vector2I" />
    /// by the given <see langword="int" />.
    /// </summary>
    /// <param name="vec">The vector to multiply.</param>
    /// <param name="scale">The scale to multiply by.</param>
    /// <returns>The multiplied vector.</returns>
    public static Vector2I operator *(Vector2I vec, int scale)
    {
        vec.X *= scale;
        vec.Y *= scale;
        return vec;
    }

    /// <summary>
    /// Multiplies each component of the <see cref="T:Godot.Vector2I" />
    /// by the given <see langword="int" />.
    /// </summary>
    /// <param name="scale">The scale to multiply by.</param>
    /// <param name="vec">The vector to multiply.</param>
    /// <returns>The multiplied vector.</returns>
    public static Vector2I operator *(int scale, Vector2I vec)
    {
        vec.X *= scale;
        vec.Y *= scale;
        return vec;
    }

    /// <summary>
    /// Multiplies each component of the <see cref="T:Godot.Vector2I" />
    /// by the components of the given <see cref="T:Godot.Vector2I" />.
    /// </summary>
    /// <param name="left">The left vector.</param>
    /// <param name="right">The right vector.</param>
    /// <returns>The multiplied vector.</returns>
    public static Vector2I operator *(Vector2I left, Vector2I right)
    {
        left.X *= right.X;
        left.Y *= right.Y;
        return left;
    }

    /// <summary>
    /// Divides each component of the <see cref="T:Godot.Vector2I" />
    /// by the given <see langword="int" />.
    /// </summary>
    /// <param name="vec">The dividend vector.</param>
    /// <param name="divisor">The divisor value.</param>
    /// <returns>The divided vector.</returns>
    public static Vector2I operator /(Vector2I vec, int divisor)
    {
        vec.X /= divisor;
        vec.Y /= divisor;
        return vec;
    }

    /// <summary>
    /// Divides each component of the <see cref="T:Godot.Vector2I" />
    /// by the components of the given <see cref="T:Godot.Vector2I" />.
    /// </summary>
    /// <param name="vec">The dividend vector.</param>
    /// <param name="divisorv">The divisor vector.</param>
    /// <returns>The divided vector.</returns>
    public static Vector2I operator /(Vector2I vec, Vector2I divisorv)
    {
        vec.X /= divisorv.X;
        vec.Y /= divisorv.Y;
        return vec;
    }

    /// <summary>
    /// Gets the remainder of each component of the <see cref="T:Godot.Vector2I" />
    /// with the components of the given <see langword="int" />.
    /// This operation uses truncated division, which is often not desired
    /// as it does not work well with negative numbers.
    /// Consider using <see cref="M:Godot.Mathf.PosMod(System.Int32,System.Int32)" /> instead
    /// if you want to handle negative numbers.
    /// </summary>
    /// <example>
    /// <code>
    /// GD.Print(new Vector2I(10, -20) % 7); // Prints "(3, -6)"
    /// </code>
    /// </example>
    /// <param name="vec">The dividend vector.</param>
    /// <param name="divisor">The divisor value.</param>
    /// <returns>The remainder vector.</returns>
    public static Vector2I operator %(Vector2I vec, int divisor)
    {
        vec.X %= divisor;
        vec.Y %= divisor;
        return vec;
    }

    /// <summary>
    /// Gets the remainder of each component of the <see cref="T:Godot.Vector2I" />
    /// with the components of the given <see cref="T:Godot.Vector2I" />.
    /// This operation uses truncated division, which is often not desired
    /// as it does not work well with negative numbers.
    /// Consider using <see cref="M:Godot.Mathf.PosMod(System.Int32,System.Int32)" /> instead
    /// if you want to handle negative numbers.
    /// </summary>
    /// <example>
    /// <code>
    /// GD.Print(new Vector2I(10, -20) % new Vector2I(7, 8)); // Prints "(3, -4)"
    /// </code>
    /// </example>
    /// <param name="vec">The dividend vector.</param>
    /// <param name="divisorv">The divisor vector.</param>
    /// <returns>The remainder vector.</returns>
    public static Vector2I operator %(Vector2I vec, Vector2I divisorv)
    {
        vec.X %= divisorv.X;
        vec.Y %= divisorv.Y;
        return vec;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the vectors are equal.
    /// </summary>
    /// <param name="left">The left vector.</param>
    /// <param name="right">The right vector.</param>
    /// <returns>Whether or not the vectors are equal.</returns>
    public static bool operator ==(Vector2I left, Vector2I right) => left.Equals(right);

    /// <summary>
    /// Returns <see langword="true" /> if the vectors are not equal.
    /// </summary>
    /// <param name="left">The left vector.</param>
    /// <param name="right">The right vector.</param>
    /// <returns>Whether or not the vectors are not equal.</returns>
    public static bool operator !=(Vector2I left, Vector2I right) => !left.Equals(right);

    /// <summary>
    /// Compares two <see cref="T:Godot.Vector2I" /> vectors by first checking if
    /// the X value of the <paramref name="left" /> vector is less than
    /// the X value of the <paramref name="right" /> vector.
    /// If the X values are exactly equal, then it repeats this check
    /// with the Y values of the two vectors.
    /// This operator is useful for sorting vectors.
    /// </summary>
    /// <param name="left">The left vector.</param>
    /// <param name="right">The right vector.</param>
    /// <returns>Whether or not the left is less than the right.</returns>
    public static bool operator <(Vector2I left, Vector2I right)
    {
        return left.X == right.X ? left.Y < right.Y : left.X < right.X;
    }

    /// <summary>
    /// Compares two <see cref="T:Godot.Vector2I" /> vectors by first checking if
    /// the X value of the <paramref name="left" /> vector is greater than
    /// the X value of the <paramref name="right" /> vector.
    /// If the X values are exactly equal, then it repeats this check
    /// with the Y values of the two vectors.
    /// This operator is useful for sorting vectors.
    /// </summary>
    /// <param name="left">The left vector.</param>
    /// <param name="right">The right vector.</param>
    /// <returns>Whether or not the left is greater than the right.</returns>
    public static bool operator >(Vector2I left, Vector2I right)
    {
        return left.X == right.X ? left.Y > right.Y : left.X > right.X;
    }

    /// <summary>
    /// Compares two <see cref="T:Godot.Vector2I" /> vectors by first checking if
    /// the X value of the <paramref name="left" /> vector is less than
    /// or equal to the X value of the <paramref name="right" /> vector.
    /// If the X values are exactly equal, then it repeats this check
    /// with the Y values of the two vectors.
    /// This operator is useful for sorting vectors.
    /// </summary>
    /// <param name="left">The left vector.</param>
    /// <param name="right">The right vector.</param>
    /// <returns>Whether or not the left is less than or equal to the right.</returns>
    public static bool operator <=(Vector2I left, Vector2I right)
    {
        return left.X == right.X ? left.Y <= right.Y : left.X < right.X;
    }

    /// <summary>
    /// Compares two <see cref="T:Godot.Vector2I" /> vectors by first checking if
    /// the X value of the <paramref name="left" /> vector is greater than
    /// or equal to the X value of the <paramref name="right" /> vector.
    /// If the X values are exactly equal, then it repeats this check
    /// with the Y values of the two vectors.
    /// This operator is useful for sorting vectors.
    /// </summary>
    /// <param name="left">The left vector.</param>
    /// <param name="right">The right vector.</param>
    /// <returns>Whether or not the left is greater than or equal to the right.</returns>
    public static bool operator >=(Vector2I left, Vector2I right)
    {
        return left.X == right.X ? left.Y >= right.Y : left.X > right.X;
    }

    /// <summary>
    /// Converts this <see cref="T:Godot.Vector2I" /> to a <see cref="T:Godot.Vector2" />.
    /// </summary>
    /// <param name="value">The vector to convert.</param>
    public static implicit operator Vector2(Vector2I value)
    {
        return new Vector2((float)value.X, (float)value.Y);
    }

    /// <summary>
    /// Converts a <see cref="T:Godot.Vector2" /> to a <see cref="T:Godot.Vector2I" /> by truncating
    /// components' fractional parts (rounding towards zero). For a different
    /// behavior consider passing the result of <see cref="M:Godot.Vector2.Ceil" />,
    /// <see cref="M:Godot.Vector2.Floor" /> or <see cref="M:Godot.Vector2.Round" /> to this conversion operator instead.
    /// </summary>
    /// <param name="value">The vector to convert.</param>
    public static explicit operator Vector2I(Vector2 value)
    {
        return new Vector2I((int)value.X, (int)value.Y);
    }
    
    public static implicit operator System.Numerics.Vector2(Vector2I self) => new(self.X, self.Y);

    /// <summary>
    /// Returns <see langword="true" /> if the vector is equal
    /// to the given object (<paramref name="obj" />).
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns>Whether or not the vector and the object are equal.</returns>
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Vector2I other && this.Equals(other);
    }

    /// <summary>
    /// Returns <see langword="true" /> if the vectors are equal.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>Whether or not the vectors are equal.</returns>
    public readonly bool Equals(Vector2I other) => this.X == other.X && this.Y == other.Y;

    /// <summary>
    /// Serves as the hash function for <see cref="T:Godot.Vector2I" />.
    /// </summary>
    /// <returns>A hash code for this vector.</returns>
    public override readonly int GetHashCode() => HashCode.Combine<int, int>(this.X, this.Y);

    /// <summary>
    /// Converts this <see cref="T:Godot.Vector2I" /> to a string.
    /// </summary>
    /// <returns>A string representation of this vector.</returns>
    public override readonly string ToString() => this.ToString((string)null);

    /// <summary>
    /// Converts this <see cref="T:Godot.Vector2I" /> to a string with the given <paramref name="format" />.
    /// </summary>
    /// <returns>A string representation of this vector.</returns>
    public readonly string ToString(string? format)
    {
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
        interpolatedStringHandler.AppendLiteral("(");
        interpolatedStringHandler.AppendFormatted(this.X.ToString(format, (IFormatProvider)CultureInfo.InvariantCulture));
        interpolatedStringHandler.AppendLiteral(", ");
        interpolatedStringHandler.AppendFormatted(this.Y.ToString(format, (IFormatProvider)CultureInfo.InvariantCulture));
        interpolatedStringHandler.AppendLiteral(")");
        return interpolatedStringHandler.ToStringAndClear();
    }

    /// <summary>
    /// Enumerated index values for the axes.
    /// Returned by <see cref="M:Godot.Vector2I.MaxAxisIndex" /> and <see cref="M:Godot.Vector2I.MinAxisIndex" />.
    /// </summary>
    public enum Axis
    {
        /// <summary>The vector's X axis.</summary>
        X,

        /// <summary>The vector's Y axis.</summary>
        Y,
    }
}