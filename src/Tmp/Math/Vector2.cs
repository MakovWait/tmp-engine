using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Tmp.Math;

/// <summary>
/// 2-element structure that can be used to represent positions in 2D space or any other pair of numeric values.
/// </summary>
[Serializable]
public struct Vector2 : IEquatable<Vector2>
{
    /// <summary>
    /// The vector's X component. Also accessible by using the index position <c>[0]</c>.
    /// </summary>
    public float X;

    /// <summary>
    /// The vector's Y component. Also accessible by using the index position <c>[1]</c>.
    /// </summary>
    public float Y;

    private static readonly Vector2 _zero = new Vector2(0.0f, 0.0f);
    private static readonly Vector2 _one = new Vector2(1f, 1f);
    private static readonly Vector2 _inf = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
    private static readonly Vector2 _up = new Vector2(0.0f, -1f);
    private static readonly Vector2 _down = new Vector2(0.0f, 1f);
    private static readonly Vector2 _right = new Vector2(1f, 0.0f);
    private static readonly Vector2 _left = new Vector2(-1f, 0.0f);

    /// <summary>Access vector components using their index.</summary>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="index" /> is not 0 or 1.
    /// </exception>
    /// <value>
    /// <c>[0]</c> is equivalent to <see cref="F:Godot.Vector2.X" />,
    /// <c>[1]</c> is equivalent to <see cref="F:Godot.Vector2.Y" />.
    /// </value>
    public float this[int index]
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
    public readonly void Deconstruct(out float x, out float y)
    {
        x = this.X;
        y = this.Y;
    }

    internal void Normalize()
    {
        float s = this.LengthSquared();
        if ((double)s == 0.0)
        {
            this.X = this.Y = 0.0f;
        }
        else
        {
            float num = Mathf.Sqrt(s);
            this.X /= num;
            this.Y /= num;
        }
    }

    /// <summary>
    /// Returns a new vector with all components in absolute values (i.e. positive).
    /// </summary>
    /// <returns>A vector with <see cref="M:Godot.Mathf.Abs(System.Single)" /> called on each component.</returns>
    public readonly Vector2 Abs() => new Vector2(Mathf.Abs(this.X), Mathf.Abs(this.Y));

    /// <summary>
    /// Returns this vector's angle with respect to the X axis, or (1, 0) vector, in radians.
    /// 
    /// Equivalent to the result of <see cref="M:Godot.Mathf.Atan2(System.Single,System.Single)" /> when
    /// called with the vector's <see cref="F:Godot.Vector2.Y" /> and <see cref="F:Godot.Vector2.X" /> as parameters: <c>Mathf.Atan2(v.Y, v.X)</c>.
    /// </summary>
    /// <returns>The angle of this vector, in radians.</returns>
    public readonly float Angle() => Mathf.Atan2(this.Y, this.X);

    /// <summary>Returns the angle to the given vector, in radians.</summary>
    /// <param name="to">The other vector to compare this vector to.</param>
    /// <returns>The angle between the two vectors, in radians.</returns>
    public readonly float AngleTo(Vector2 to) => Mathf.Atan2(this.Cross(to), this.Dot(to));

    /// <summary>
    /// Returns the angle between the line connecting the two points and the X axis, in radians.
    /// </summary>
    /// <param name="to">The other vector to compare this vector to.</param>
    /// <returns>The angle between the two vectors, in radians.</returns>
    public readonly float AngleToPoint(Vector2 to) => Mathf.Atan2(to.Y - this.Y, to.X - this.X);

    /// <summary>
    /// Returns the aspect ratio of this vector, the ratio of <see cref="F:Godot.Vector2.X" /> to <see cref="F:Godot.Vector2.Y" />.
    /// </summary>
    /// <returns>The <see cref="F:Godot.Vector2.X" /> component divided by the <see cref="F:Godot.Vector2.Y" /> component.</returns>
    public readonly float Aspect() => this.X / this.Y;

    /// <summary>
    /// Returns the vector "bounced off" from a plane defined by the given normal.
    /// </summary>
    /// <param name="normal">The normal vector defining the plane to bounce off. Must be normalized.</param>
    /// <returns>The bounced vector.</returns>
    public readonly Vector2 Bounce(Vector2 normal) => -this.Reflect(normal);

    /// <summary>
    /// Returns a new vector with all components rounded up (towards positive infinity).
    /// </summary>
    /// <returns>A vector with <see cref="M:Godot.Mathf.Ceil(System.Single)" /> called on each component.</returns>
    public readonly Vector2 Ceil() => new Vector2(Mathf.Ceil(this.X), Mathf.Ceil(this.Y));

    /// <summary>
    /// Returns a new vector with all components clamped between the
    /// components of <paramref name="min" /> and <paramref name="max" /> using
    /// <see cref="M:Godot.Mathf.Clamp(System.Single,System.Single,System.Single)" />.
    /// </summary>
    /// <param name="min">The vector with minimum allowed values.</param>
    /// <param name="max">The vector with maximum allowed values.</param>
    /// <returns>The vector with all components clamped.</returns>
    public readonly Vector2 Clamp(Vector2 min, Vector2 max)
    {
        return new Vector2(Mathf.Clamp(this.X, min.X, max.X), Mathf.Clamp(this.Y, min.Y, max.Y));
    }

    /// <summary>
    /// Returns a new vector with all components clamped between the
    /// <paramref name="min" /> and <paramref name="max" /> using
    /// <see cref="M:Godot.Mathf.Clamp(System.Single,System.Single,System.Single)" />.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <returns>The vector with all components clamped.</returns>
    public readonly Vector2 Clamp(float min, float max)
    {
        return new Vector2(Mathf.Clamp(this.X, min, max), Mathf.Clamp(this.Y, min, max));
    }

    /// <summary>
    /// Returns the cross product of this vector and <paramref name="with" />.
    /// </summary>
    /// <param name="with">The other vector.</param>
    /// <returns>The cross product value.</returns>
    public readonly float Cross(Vector2 with)
    {
        return (float)((double)this.X * (double)with.Y - (double)this.Y * (double)with.X);
    }

    /// <summary>
    /// Performs a cubic interpolation between vectors <paramref name="preA" />, this vector,
    /// <paramref name="b" />, and <paramref name="postB" />, by the given amount <paramref name="weight" />.
    /// </summary>
    /// <param name="b">The destination vector.</param>
    /// <param name="preA">A vector before this vector.</param>
    /// <param name="postB">A vector after <paramref name="b" />.</param>
    /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <returns>The interpolated vector.</returns>
    public readonly Vector2 CubicInterpolate(Vector2 b, Vector2 preA, Vector2 postB, float weight)
    {
        return new Vector2(Mathf.CubicInterpolate(this.X, b.X, preA.X, postB.X, weight), Mathf.CubicInterpolate(this.Y, b.Y, preA.Y, postB.Y, weight));
    }

    /// <summary>
    /// Performs a cubic interpolation between vectors <paramref name="preA" />, this vector,
    /// <paramref name="b" />, and <paramref name="postB" />, by the given amount <paramref name="weight" />.
    /// It can perform smoother interpolation than <see cref="M:Godot.Vector2.CubicInterpolate(Godot.Vector2,Godot.Vector2,Godot.Vector2,System.Single)" />
    /// by the time values.
    /// </summary>
    /// <param name="b">The destination vector.</param>
    /// <param name="preA">A vector before this vector.</param>
    /// <param name="postB">A vector after <paramref name="b" />.</param>
    /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <param name="t"></param>
    /// <param name="preAT"></param>
    /// <param name="postBT"></param>
    /// <returns>The interpolated vector.</returns>
    public readonly Vector2 CubicInterpolateInTime(
        Vector2 b,
        Vector2 preA,
        Vector2 postB,
        float weight,
        float t,
        float preAT,
        float postBT
    )
    {
        return new Vector2(Mathf.CubicInterpolateInTime(this.X, b.X, preA.X, postB.X, weight, t, preAT, postBT), Mathf.CubicInterpolateInTime(this.Y, b.Y, preA.Y, postB.Y, weight, t, preAT, postBT));
    }

    /// <summary>
    /// Returns the point at the given <paramref name="t" /> on a one-dimensional Bezier curve defined by this vector
    /// and the given <paramref name="control1" />, <paramref name="control2" />, and <paramref name="end" /> points.
    /// </summary>
    /// <param name="control1">Control point that defines the bezier curve.</param>
    /// <param name="control2">Control point that defines the bezier curve.</param>
    /// <param name="end">The destination vector.</param>
    /// <param name="t">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <returns>The interpolated vector.</returns>
    public readonly Vector2 BezierInterpolate(
        Vector2 control1,
        Vector2 control2,
        Vector2 end,
        float t
    )
    {
        return new Vector2(Mathf.BezierInterpolate(this.X, control1.X, control2.X, end.X, t), Mathf.BezierInterpolate(this.Y, control1.Y, control2.Y, end.Y, t));
    }

    /// <summary>
    /// Returns the derivative at the given <paramref name="t" /> on the Bezier curve defined by this vector
    /// and the given <paramref name="control1" />, <paramref name="control2" />, and <paramref name="end" /> points.
    /// </summary>
    /// <param name="control1">Control point that defines the bezier curve.</param>
    /// <param name="control2">Control point that defines the bezier curve.</param>
    /// <param name="end">The destination value for the interpolation.</param>
    /// <param name="t">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <returns>The resulting value of the interpolation.</returns>
    public readonly Vector2 BezierDerivative(
        Vector2 control1,
        Vector2 control2,
        Vector2 end,
        float t
    )
    {
        return new Vector2(Mathf.BezierDerivative(this.X, control1.X, control2.X, end.X, t), Mathf.BezierDerivative(this.Y, control1.Y, control2.Y, end.Y, t));
    }

    /// <summary>
    /// Returns the normalized vector pointing from this vector to <paramref name="to" />.
    /// </summary>
    /// <param name="to">The other vector to point towards.</param>
    /// <returns>The direction from this vector to <paramref name="to" />.</returns>
    public readonly Vector2 DirectionTo(Vector2 to)
    {
        return new Vector2(to.X - this.X, to.Y - this.Y).Normalized();
    }

    /// <summary>
    /// Returns the squared distance between this vector and <paramref name="to" />.
    /// This method runs faster than <see cref="M:Godot.Vector2.DistanceTo(Godot.Vector2)" />, so prefer it if
    /// you need to compare vectors or need the squared distance for some formula.
    /// </summary>
    /// <param name="to">The other vector to use.</param>
    /// <returns>The squared distance between the two vectors.</returns>
    public readonly float DistanceSquaredTo(Vector2 to)
    {
        return (float)(((double)this.X - (double)to.X) * ((double)this.X - (double)to.X) + ((double)this.Y - (double)to.Y) * ((double)this.Y - (double)to.Y));
    }

    /// <summary>
    /// Returns the distance between this vector and <paramref name="to" />.
    /// </summary>
    /// <param name="to">The other vector to use.</param>
    /// <returns>The distance between the two vectors.</returns>
    public readonly float DistanceTo(Vector2 to)
    {
        return Mathf.Sqrt((float)(((double)this.X - (double)to.X) * ((double)this.X - (double)to.X) + ((double)this.Y - (double)to.Y) * ((double)this.Y - (double)to.Y)));
    }

    /// <summary>
    /// Returns the dot product of this vector and <paramref name="with" />.
    /// </summary>
    /// <param name="with">The other vector to use.</param>
    /// <returns>The dot product of the two vectors.</returns>
    public readonly float Dot(Vector2 with)
    {
        return (float)((double)this.X * (double)with.X + (double)this.Y * (double)with.Y);
    }

    /// <summary>
    /// Returns a new vector with all components rounded down (towards negative infinity).
    /// </summary>
    /// <returns>A vector with <see cref="M:Godot.Mathf.Floor(System.Single)" /> called on each component.</returns>
    public readonly Vector2 Floor() => new Vector2(Mathf.Floor(this.X), Mathf.Floor(this.Y));

    /// <summary>
    /// Returns the inverse of this vector. This is the same as <c>new Vector2(1 / v.X, 1 / v.Y)</c>.
    /// </summary>
    /// <returns>The inverse of this vector.</returns>
    public readonly Vector2 Inverse() => new Vector2(1f / this.X, 1f / this.Y);

    /// <summary>
    /// Returns <see langword="true" /> if this vector is finite, by calling
    /// <see cref="M:Godot.Mathf.IsFinite(System.Single)" /> on each component.
    /// </summary>
    /// <returns>Whether this vector is finite or not.</returns>
    public readonly bool IsFinite() => Mathf.IsFinite(this.X) && Mathf.IsFinite(this.Y);

    /// <summary>
    /// Returns <see langword="true" /> if the vector is normalized, and <see langword="false" /> otherwise.
    /// </summary>
    /// <returns>A <see langword="bool" /> indicating whether or not the vector is normalized.</returns>
    public readonly bool IsNormalized()
    {
        return (double)Mathf.Abs(this.LengthSquared() - 1f) < 9.999999974752427E-07;
    }

    /// <summary>Returns the length (magnitude) of this vector.</summary>
    /// <seealso cref="M:Godot.Vector2.LengthSquared" />
    /// <returns>The length of this vector.</returns>
    public readonly float Length()
    {
        return Mathf.Sqrt((float)((double)this.X * (double)this.X + (double)this.Y * (double)this.Y));
    }

    /// <summary>
    /// Returns the squared length (squared magnitude) of this vector.
    /// This method runs faster than <see cref="M:Godot.Vector2.Length" />, so prefer it if
    /// you need to compare vectors or need the squared length for some formula.
    /// </summary>
    /// <returns>The squared length of this vector.</returns>
    public readonly float LengthSquared()
    {
        return (float)((double)this.X * (double)this.X + (double)this.Y * (double)this.Y);
    }

    /// <summary>
    /// Returns the result of the linear interpolation between
    /// this vector and <paramref name="to" /> by amount <paramref name="weight" />.
    /// </summary>
    /// <param name="to">The destination vector for interpolation.</param>
    /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <returns>The resulting vector of the interpolation.</returns>
    public readonly Vector2 Lerp(Vector2 to, float weight)
    {
        return new Vector2(Mathf.Lerp(this.X, to.X, weight), Mathf.Lerp(this.Y, to.Y, weight));
    }

    /// <summary>
    /// Returns the vector with a maximum length by limiting its length to <paramref name="length" />.
    /// </summary>
    /// <param name="length">The length to limit to.</param>
    /// <returns>The vector with its length limited.</returns>
    public readonly Vector2 LimitLength(float length = 1f)
    {
        Vector2 vector2 = this;
        float num = this.Length();
        if ((double)num > 0.0 && (double)length < (double)num)
            vector2 = vector2 / num * length;
        return vector2;
    }

    /// <summary>
    /// Returns the result of the component-wise maximum between
    /// this vector and <paramref name="with" />.
    /// Equivalent to <c>new Vector2(Mathf.Max(X, with.X), Mathf.Max(Y, with.Y))</c>.
    /// </summary>
    /// <param name="with">The other vector to use.</param>
    /// <returns>The resulting maximum vector.</returns>
    public readonly Vector2 Max(Vector2 with)
    {
        return new Vector2(Mathf.Max(this.X, with.X), Mathf.Max(this.Y, with.Y));
    }

    /// <summary>
    /// Returns the result of the component-wise maximum between
    /// this vector and <paramref name="with" />.
    /// Equivalent to <c>new Vector2(Mathf.Max(X, with), Mathf.Max(Y, with))</c>.
    /// </summary>
    /// <param name="with">The other value to use.</param>
    /// <returns>The resulting maximum vector.</returns>
    public readonly Vector2 Max(float with)
    {
        return new Vector2(Mathf.Max(this.X, with), Mathf.Max(this.Y, with));
    }

    /// <summary>
    /// Returns the result of the component-wise minimum between
    /// this vector and <paramref name="with" />.
    /// Equivalent to <c>new Vector2(Mathf.Min(X, with.X), Mathf.Min(Y, with.Y))</c>.
    /// </summary>
    /// <param name="with">The other vector to use.</param>
    /// <returns>The resulting minimum vector.</returns>
    public readonly Vector2 Min(Vector2 with)
    {
        return new Vector2(Mathf.Min(this.X, with.X), Mathf.Min(this.Y, with.Y));
    }

    /// <summary>
    /// Returns the result of the component-wise minimum between
    /// this vector and <paramref name="with" />.
    /// Equivalent to <c>new Vector2(Mathf.Min(X, with), Mathf.Min(Y, with))</c>.
    /// </summary>
    /// <param name="with">The other value to use.</param>
    /// <returns>The resulting minimum vector.</returns>
    public readonly Vector2 Min(float with)
    {
        return new Vector2(Mathf.Min(this.X, with), Mathf.Min(this.Y, with));
    }

    /// <summary>
    /// Returns the axis of the vector's highest value. See <see cref="T:Godot.Vector2.Axis" />.
    /// If both components are equal, this method returns <see cref="F:Godot.Vector2.Axis.X" />.
    /// </summary>
    /// <returns>The index of the highest axis.</returns>
    public readonly Vector2.Axis MaxAxisIndex()
    {
        return (double)this.X >= (double)this.Y ? Vector2.Axis.X : Vector2.Axis.Y;
    }

    /// <summary>
    /// Returns the axis of the vector's lowest value. See <see cref="T:Godot.Vector2.Axis" />.
    /// If both components are equal, this method returns <see cref="F:Godot.Vector2.Axis.Y" />.
    /// </summary>
    /// <returns>The index of the lowest axis.</returns>
    public readonly Vector2.Axis MinAxisIndex()
    {
        return (double)this.X >= (double)this.Y ? Vector2.Axis.Y : Vector2.Axis.X;
    }

    /// <summary>
    /// Moves this vector toward <paramref name="to" /> by the fixed <paramref name="delta" /> amount.
    /// </summary>
    /// <param name="to">The vector to move towards.</param>
    /// <param name="delta">The amount to move towards by.</param>
    /// <returns>The resulting vector.</returns>
    public readonly Vector2 MoveToward(Vector2 to, float delta)
    {
        Vector2 vector2_1 = this;
        Vector2 vector2_2 = to - vector2_1;
        float num = vector2_2.Length();
        return (double)num <= (double)delta || (double)num < 9.999999974752427E-07 ? to : vector2_1 + vector2_2 / num * delta;
    }

    /// <summary>
    /// Returns the vector scaled to unit length. Equivalent to <c>v / v.Length()</c>.
    /// </summary>
    /// <returns>A normalized version of the vector.</returns>
    public readonly Vector2 Normalized()
    {
        Vector2 vector2 = this;
        vector2.Normalize();
        return vector2;
    }

    /// <summary>
    /// Returns a vector composed of the <see cref="M:Godot.Mathf.PosMod(System.Single,System.Single)" /> of this vector's components
    /// and <paramref name="mod" />.
    /// </summary>
    /// <param name="mod">A value representing the divisor of the operation.</param>
    /// <returns>
    /// A vector with each component <see cref="M:Godot.Mathf.PosMod(System.Single,System.Single)" /> by <paramref name="mod" />.
    /// </returns>
    public readonly Vector2 PosMod(float mod)
    {
        Vector2 vector2;
        vector2.X = Mathf.PosMod(this.X, mod);
        vector2.Y = Mathf.PosMod(this.Y, mod);
        return vector2;
    }

    /// <summary>
    /// Returns a vector composed of the <see cref="M:Godot.Mathf.PosMod(System.Single,System.Single)" /> of this vector's components
    /// and <paramref name="modv" />'s components.
    /// </summary>
    /// <param name="modv">A vector representing the divisors of the operation.</param>
    /// <returns>
    /// A vector with each component <see cref="M:Godot.Mathf.PosMod(System.Single,System.Single)" /> by <paramref name="modv" />'s components.
    /// </returns>
    public readonly Vector2 PosMod(Vector2 modv)
    {
        Vector2 vector2;
        vector2.X = Mathf.PosMod(this.X, modv.X);
        vector2.Y = Mathf.PosMod(this.Y, modv.Y);
        return vector2;
    }

    /// <summary>
    /// Returns a new vector resulting from projecting this vector onto the given vector <paramref name="onNormal" />.
    /// The resulting new vector is parallel to <paramref name="onNormal" />.
    /// See also <see cref="M:Godot.Vector2.Slide(Godot.Vector2)" />.
    /// Note: If the vector <paramref name="onNormal" /> is a zero vector, the components of the resulting new vector will be <see cref="F:System.Single.NaN" />.
    /// </summary>
    /// <param name="onNormal">The vector to project onto.</param>
    /// <returns>The projected vector.</returns>
    public readonly Vector2 Project(Vector2 onNormal)
    {
        return onNormal * (this.Dot(onNormal) / onNormal.LengthSquared());
    }

    /// <summary>
    /// Returns this vector reflected from a plane defined by the given <paramref name="normal" />.
    /// </summary>
    /// <param name="normal">The normal vector defining the plane to reflect from. Must be normalized.</param>
    /// <returns>The reflected vector.</returns>
    public readonly Vector2 Reflect(Vector2 normal) => 2f * this.Dot(normal) * normal - this;

    /// <summary>
    /// Rotates this vector by <paramref name="angle" /> radians.
    /// </summary>
    /// <param name="angle">The angle to rotate by, in radians.</param>
    /// <returns>The rotated vector.</returns>
    public readonly Vector2 Rotated(float angle)
    {
        (float Sin, float Cos) = Mathf.SinCos(angle);
        return new Vector2((float)((double)this.X * (double)Cos - (double)this.Y * (double)Sin), (float)((double)this.X * (double)Sin + (double)this.Y * (double)Cos));
    }

    /// <summary>
    /// Returns this vector with all components rounded to the nearest integer,
    /// with halfway cases rounded towards the nearest multiple of two.
    /// </summary>
    /// <returns>The rounded vector.</returns>
    public readonly Vector2 Round() => new Vector2(Mathf.Round(this.X), Mathf.Round(this.Y));

    /// <summary>
    /// Returns a vector with each component set to one or negative one, depending
    /// on the signs of this vector's components, or zero if the component is zero,
    /// by calling <see cref="M:Godot.Mathf.Sign(System.Single)" /> on each component.
    /// </summary>
    /// <returns>A vector with all components as either <c>1</c>, <c>-1</c>, or <c>0</c>.</returns>
    public readonly Vector2 Sign()
    {
        Vector2 vector2;
        vector2.X = (float)Mathf.Sign(this.X);
        vector2.Y = (float)Mathf.Sign(this.Y);
        return vector2;
    }

    /// <summary>
    /// Returns the result of the spherical linear interpolation between
    /// this vector and <paramref name="to" /> by amount <paramref name="weight" />.
    /// 
    /// This method also handles interpolating the lengths if the input vectors
    /// have different lengths. For the special case of one or both input vectors
    /// having zero length, this method behaves like <see cref="M:Godot.Vector2.Lerp(Godot.Vector2,System.Single)" />.
    /// </summary>
    /// <param name="to">The destination vector for interpolation.</param>
    /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <returns>The resulting vector of the interpolation.</returns>
    public readonly Vector2 Slerp(Vector2 to, float weight)
    {
        float s1 = this.LengthSquared();
        float s2 = to.LengthSquared();
        if ((double)s1 == 0.0 || (double)s2 == 0.0)
            return this.Lerp(to, weight);
        float from = Mathf.Sqrt(s1);
        float num = Mathf.Lerp(from, Mathf.Sqrt(s2), weight);
        return this.Rotated(this.AngleTo(to) * weight) * (num / from);
    }

    /// <summary>
    /// Returns a new vector resulting from sliding this vector along a line with normal <paramref name="normal" />.
    /// The resulting new vector is perpendicular to <paramref name="normal" />, and is equivalent to this vector minus its projection on <paramref name="normal" />.
    /// See also <see cref="M:Godot.Vector2.Project(Godot.Vector2)" />.
    /// Note: The vector <paramref name="normal" /> must be normalized. See also <see cref="M:Godot.Vector2.Normalized" />.
    /// </summary>
    /// <param name="normal">The normal vector of the plane to slide on.</param>
    /// <returns>The slid vector.</returns>
    public readonly Vector2 Slide(Vector2 normal) => this - normal * this.Dot(normal);

    /// <summary>
    /// Returns a new vector with each component snapped to the nearest multiple of the corresponding component in <paramref name="step" />.
    /// This can also be used to round to an arbitrary number of decimals.
    /// </summary>
    /// <param name="step">A vector value representing the step size to snap to.</param>
    /// <returns>The snapped vector.</returns>
    public readonly Vector2 Snapped(Vector2 step)
    {
        return new Vector2(Mathf.Snapped(this.X, step.X), Mathf.Snapped(this.Y, step.Y));
    }

    /// <summary>
    /// Returns a new vector with each component snapped to the nearest multiple of <paramref name="step" />.
    /// This can also be used to round to an arbitrary number of decimals.
    /// </summary>
    /// <param name="step">The step size to snap to.</param>
    /// <returns>The snapped vector.</returns>
    public readonly Vector2 Snapped(float step)
    {
        return new Vector2(Mathf.Snapped(this.X, step), Mathf.Snapped(this.Y, step));
    }

    /// <summary>
    /// Returns a perpendicular vector rotated 90 degrees counter-clockwise
    /// compared to the original, with the same length.
    /// </summary>
    /// <returns>The perpendicular vector.</returns>
    public readonly Vector2 Orthogonal() => new Vector2(this.Y, -this.X);

    /// <summary>
    /// Zero vector, a vector with all components set to <c>0</c>.
    /// </summary>
    /// <value>Equivalent to <c>new Vector2(0, 0)</c>.</value>
    public static Vector2 Zero => Vector2._zero;

    /// <summary>
    /// One vector, a vector with all components set to <c>1</c>.
    /// </summary>
    /// <value>Equivalent to <c>new Vector2(1, 1)</c>.</value>
    public static Vector2 One => Vector2._one;

    /// <summary>
    /// Infinity vector, a vector with all components set to <see cref="F:Godot.Mathf.Inf" />.
    /// </summary>
    /// <value>Equivalent to <c>new Vector2(Mathf.Inf, Mathf.Inf)</c>.</value>
    public static Vector2 Inf => Vector2._inf;

    /// <summary>
    /// Up unit vector. Y is down in 2D, so this vector points -Y.
    /// </summary>
    /// <value>Equivalent to <c>new Vector2(0, -1)</c>.</value>
    public static Vector2 Up => Vector2._up;

    /// <summary>
    /// Down unit vector. Y is down in 2D, so this vector points +Y.
    /// </summary>
    /// <value>Equivalent to <c>new Vector2(0, 1)</c>.</value>
    public static Vector2 Down => Vector2._down;

    /// <summary>Right unit vector. Represents the direction of right.</summary>
    /// <value>Equivalent to <c>new Vector2(1, 0)</c>.</value>
    public static Vector2 Right => Vector2._right;

    /// <summary>Left unit vector. Represents the direction of left.</summary>
    /// <value>Equivalent to <c>new Vector2(-1, 0)</c>.</value>
    public static Vector2 Left => Vector2._left;

    /// <summary>
    /// Constructs a new <see cref="T:Godot.Vector2" /> with the given components.
    /// </summary>
    /// <param name="x">The vector's X component.</param>
    /// <param name="y">The vector's Y component.</param>
    public Vector2(float x, float y)
    {
        this.X = x;
        this.Y = y;
    }

    /// <summary>
    /// Creates a unit Vector2 rotated to the given angle. This is equivalent to doing
    /// <c>Vector2(Mathf.Cos(angle), Mathf.Sin(angle))</c> or <c>Vector2.Right.Rotated(angle)</c>.
    /// </summary>
    /// <param name="angle">Angle of the vector, in radians.</param>
    /// <returns>The resulting vector.</returns>
    public static Vector2 FromAngle(float angle)
    {
        (float Sin, float Cos) tuple = Mathf.SinCos(angle);
        return new Vector2(tuple.Cos, tuple.Sin);
    }

    /// <summary>
    /// Adds each component of the <see cref="T:Godot.Vector2" />
    /// with the components of the given <see cref="T:Godot.Vector2" />.
    /// </summary>
    /// <param name="left">The left vector.</param>
    /// <param name="right">The right vector.</param>
    /// <returns>The added vector.</returns>
    public static Vector2 operator +(Vector2 left, Vector2 right)
    {
        left.X += right.X;
        left.Y += right.Y;
        return left;
    }

    /// <summary>
    /// Subtracts each component of the <see cref="T:Godot.Vector2" />
    /// by the components of the given <see cref="T:Godot.Vector2" />.
    /// </summary>
    /// <param name="left">The left vector.</param>
    /// <param name="right">The right vector.</param>
    /// <returns>The subtracted vector.</returns>
    public static Vector2 operator -(Vector2 left, Vector2 right)
    {
        left.X -= right.X;
        left.Y -= right.Y;
        return left;
    }

    /// <summary>
    /// Returns the negative value of the <see cref="T:Godot.Vector2" />.
    /// This is the same as writing <c>new Vector2(-v.X, -v.Y)</c>.
    /// This operation flips the direction of the vector while
    /// keeping the same magnitude.
    /// With floats, the number zero can be either positive or negative.
    /// </summary>
    /// <param name="vec">The vector to negate/flip.</param>
    /// <returns>The negated/flipped vector.</returns>
    public static Vector2 operator -(Vector2 vec)
    {
        vec.X = -vec.X;
        vec.Y = -vec.Y;
        return vec;
    }

    /// <summary>
    /// Multiplies each component of the <see cref="T:Godot.Vector2" />
    /// by the given <see cref="T:System.Single" />.
    /// </summary>
    /// <param name="vec">The vector to multiply.</param>
    /// <param name="scale">The scale to multiply by.</param>
    /// <returns>The multiplied vector.</returns>
    public static Vector2 operator *(Vector2 vec, float scale)
    {
        vec.X *= scale;
        vec.Y *= scale;
        return vec;
    }

    /// <summary>
    /// Multiplies each component of the <see cref="T:Godot.Vector2" />
    /// by the given <see cref="T:System.Single" />.
    /// </summary>
    /// <param name="scale">The scale to multiply by.</param>
    /// <param name="vec">The vector to multiply.</param>
    /// <returns>The multiplied vector.</returns>
    public static Vector2 operator *(float scale, Vector2 vec)
    {
        vec.X *= scale;
        vec.Y *= scale;
        return vec;
    }

    /// <summary>
    /// Multiplies each component of the <see cref="T:Godot.Vector2" />
    /// by the components of the given <see cref="T:Godot.Vector2" />.
    /// </summary>
    /// <param name="left">The left vector.</param>
    /// <param name="right">The right vector.</param>
    /// <returns>The multiplied vector.</returns>
    public static Vector2 operator *(Vector2 left, Vector2 right)
    {
        left.X *= right.X;
        left.Y *= right.Y;
        return left;
    }

    /// <summary>
    /// Divides each component of the <see cref="T:Godot.Vector2" />
    /// by the given <see cref="T:System.Single" />.
    /// </summary>
    /// <param name="vec">The dividend vector.</param>
    /// <param name="divisor">The divisor value.</param>
    /// <returns>The divided vector.</returns>
    public static Vector2 operator /(Vector2 vec, float divisor)
    {
        vec.X /= divisor;
        vec.Y /= divisor;
        return vec;
    }

    /// <summary>
    /// Divides each component of the <see cref="T:Godot.Vector2" />
    /// by the components of the given <see cref="T:Godot.Vector2" />.
    /// </summary>
    /// <param name="vec">The dividend vector.</param>
    /// <param name="divisorv">The divisor vector.</param>
    /// <returns>The divided vector.</returns>
    public static Vector2 operator /(Vector2 vec, Vector2 divisorv)
    {
        vec.X /= divisorv.X;
        vec.Y /= divisorv.Y;
        return vec;
    }

    /// <summary>
    /// Gets the remainder of each component of the <see cref="T:Godot.Vector2" />
    /// with the components of the given <see cref="T:System.Single" />.
    /// This operation uses truncated division, which is often not desired
    /// as it does not work well with negative numbers.
    /// Consider using <see cref="M:Godot.Vector2.PosMod(System.Single)" /> instead
    /// if you want to handle negative numbers.
    /// </summary>
    /// <example>
    /// <code>
    /// GD.Print(new Vector2(10, -20) % 7); // Prints "(3, -6)"
    /// </code>
    /// </example>
    /// <param name="vec">The dividend vector.</param>
    /// <param name="divisor">The divisor value.</param>
    /// <returns>The remainder vector.</returns>
    public static Vector2 operator %(Vector2 vec, float divisor)
    {
        vec.X %= divisor;
        vec.Y %= divisor;
        return vec;
    }

    /// <summary>
    /// Gets the remainder of each component of the <see cref="T:Godot.Vector2" />
    /// with the components of the given <see cref="T:Godot.Vector2" />.
    /// This operation uses truncated division, which is often not desired
    /// as it does not work well with negative numbers.
    /// Consider using <see cref="M:Godot.Vector2.PosMod(Godot.Vector2)" /> instead
    /// if you want to handle negative numbers.
    /// </summary>
    /// <example>
    /// <code>
    /// GD.Print(new Vector2(10, -20) % new Vector2(7, 8)); // Prints "(3, -4)"
    /// </code>
    /// </example>
    /// <param name="vec">The dividend vector.</param>
    /// <param name="divisorv">The divisor vector.</param>
    /// <returns>The remainder vector.</returns>
    public static Vector2 operator %(Vector2 vec, Vector2 divisorv)
    {
        vec.X %= divisorv.X;
        vec.Y %= divisorv.Y;
        return vec;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the vectors are exactly equal.
    /// Note: Due to floating-point precision errors, consider using
    /// <see cref="M:Godot.Vector2.IsEqualApprox(Godot.Vector2)" /> instead, which is more reliable.
    /// </summary>
    /// <param name="left">The left vector.</param>
    /// <param name="right">The right vector.</param>
    /// <returns>Whether or not the vectors are exactly equal.</returns>
    public static bool operator ==(Vector2 left, Vector2 right) => left.Equals(right);

    /// <summary>
    /// Returns <see langword="true" /> if the vectors are not equal.
    /// Note: Due to floating-point precision errors, consider using
    /// <see cref="M:Godot.Vector2.IsEqualApprox(Godot.Vector2)" /> instead, which is more reliable.
    /// </summary>
    /// <param name="left">The left vector.</param>
    /// <param name="right">The right vector.</param>
    /// <returns>Whether or not the vectors are not equal.</returns>
    public static bool operator !=(Vector2 left, Vector2 right) => !left.Equals(right);

    /// <summary>
    /// Compares two <see cref="T:Godot.Vector2" /> vectors by first checking if
    /// the X value of the <paramref name="left" /> vector is less than
    /// the X value of the <paramref name="right" /> vector.
    /// If the X values are exactly equal, then it repeats this check
    /// with the Y values of the two vectors.
    /// This operator is useful for sorting vectors.
    /// </summary>
    /// <param name="left">The left vector.</param>
    /// <param name="right">The right vector.</param>
    /// <returns>Whether or not the left is less than the right.</returns>
    public static bool operator <(Vector2 left, Vector2 right)
    {
        return (double)left.X == (double)right.X ? (double)left.Y < (double)right.Y : (double)left.X < (double)right.X;
    }

    /// <summary>
    /// Compares two <see cref="T:Godot.Vector2" /> vectors by first checking if
    /// the X value of the <paramref name="left" /> vector is greater than
    /// the X value of the <paramref name="right" /> vector.
    /// If the X values are exactly equal, then it repeats this check
    /// with the Y values of the two vectors.
    /// This operator is useful for sorting vectors.
    /// </summary>
    /// <param name="left">The left vector.</param>
    /// <param name="right">The right vector.</param>
    /// <returns>Whether or not the left is greater than the right.</returns>
    public static bool operator >(Vector2 left, Vector2 right)
    {
        return (double)left.X == (double)right.X ? (double)left.Y > (double)right.Y : (double)left.X > (double)right.X;
    }

    /// <summary>
    /// Compares two <see cref="T:Godot.Vector2" /> vectors by first checking if
    /// the X value of the <paramref name="left" /> vector is less than
    /// or equal to the X value of the <paramref name="right" /> vector.
    /// If the X values are exactly equal, then it repeats this check
    /// with the Y values of the two vectors.
    /// This operator is useful for sorting vectors.
    /// </summary>
    /// <param name="left">The left vector.</param>
    /// <param name="right">The right vector.</param>
    /// <returns>Whether or not the left is less than or equal to the right.</returns>
    public static bool operator <=(Vector2 left, Vector2 right)
    {
        return (double)left.X == (double)right.X ? (double)left.Y <= (double)right.Y : (double)left.X < (double)right.X;
    }

    /// <summary>
    /// Compares two <see cref="T:Godot.Vector2" /> vectors by first checking if
    /// the X value of the <paramref name="left" /> vector is greater than
    /// or equal to the X value of the <paramref name="right" /> vector.
    /// If the X values are exactly equal, then it repeats this check
    /// with the Y values of the two vectors.
    /// This operator is useful for sorting vectors.
    /// </summary>
    /// <param name="left">The left vector.</param>
    /// <param name="right">The right vector.</param>
    /// <returns>Whether or not the left is greater than or equal to the right.</returns>
    public static bool operator >=(Vector2 left, Vector2 right)
    {
        return (double)left.X == (double)right.X ? (double)left.Y >= (double)right.Y : (double)left.X > (double)right.X;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the vector is exactly equal
    /// to the given object (<paramref name="obj" />).
    /// Note: Due to floating-point precision errors, consider using
    /// <see cref="M:Godot.Vector2.IsEqualApprox(Godot.Vector2)" /> instead, which is more reliable.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns>Whether or not the vector and the object are equal.</returns>
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Vector2 other && this.Equals(other);
    }

    /// <summary>
    /// Returns <see langword="true" /> if the vectors are exactly equal.
    /// Note: Due to floating-point precision errors, consider using
    /// <see cref="M:Godot.Vector2.IsEqualApprox(Godot.Vector2)" /> instead, which is more reliable.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>Whether or not the vectors are exactly equal.</returns>
    public readonly bool Equals(Vector2 other)
    {
        return (double)this.X == (double)other.X && (double)this.Y == (double)other.Y;
    }

    /// <summary>
    /// Returns <see langword="true" /> if this vector and <paramref name="other" /> are approximately equal,
    /// by running <see cref="M:Godot.Mathf.IsEqualApprox(System.Single,System.Single)" /> on each component.
    /// </summary>
    /// <param name="other">The other vector to compare.</param>
    /// <returns>Whether or not the vectors are approximately equal.</returns>
    public readonly bool IsEqualApprox(Vector2 other)
    {
        return Mathf.IsEqualApprox(this.X, other.X) && Mathf.IsEqualApprox(this.Y, other.Y);
    }
    
    public static implicit operator System.Numerics.Vector2(Vector2 self) => new(self.X, self.Y);

    /// <summary>
    /// Returns <see langword="true" /> if this vector's values are approximately zero,
    /// by running <see cref="M:Godot.Mathf.IsZeroApprox(System.Single)" /> on each component.
    /// This method is faster than using <see cref="M:Godot.Vector2.IsEqualApprox(Godot.Vector2)" /> with one value
    /// as a zero vector.
    /// </summary>
    /// <returns>Whether or not the vector is approximately zero.</returns>
    public readonly bool IsZeroApprox() => Mathf.IsZeroApprox(this.X) && Mathf.IsZeroApprox(this.Y);

    /// <summary>
    /// Serves as the hash function for <see cref="T:Godot.Vector2" />.
    /// </summary>
    /// <returns>A hash code for this vector.</returns>
    public override readonly int GetHashCode() => HashCode.Combine<float, float>(this.X, this.Y);

    /// <summary>
    /// Converts this <see cref="T:Godot.Vector2" /> to a string.
    /// </summary>
    /// <returns>A string representation of this vector.</returns>
    public override readonly string ToString() => this.ToString((string)null);

    /// <summary>
    /// Converts this <see cref="T:Godot.Vector2" /> to a string with the given <paramref name="format" />.
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
    /// Returned by <see cref="M:Godot.Vector2.MaxAxisIndex" /> and <see cref="M:Godot.Vector2.MinAxisIndex" />.
    /// </summary>
    public enum Axis
    {
        /// <summary>The vector's X axis.</summary>
        X,

        /// <summary>The vector's Y axis.</summary>
        Y,
    }
}