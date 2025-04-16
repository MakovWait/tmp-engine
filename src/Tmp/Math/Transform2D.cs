using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Tmp.Math;

/// <summary>
/// 2×3 matrix (2 rows, 3 columns) used for 2D linear transformations.
/// It can represent transformations such as translation, rotation, or scaling.
/// It consists of a three <see cref="T:Godot.Vector2" /> values: x, y, and the origin.
/// 
/// For more information, read this documentation article:
/// https://docs.godotengine.org/en/latest/tutorials/math/matrices_and_transforms.html
/// </summary>
[Serializable]
public struct Transform2D : IEquatable<Transform2D>
{
    /// <summary>
    /// The basis matrix's X vector (column 0). Equivalent to array index <c>[0]</c>.
    /// </summary>
    public Vector2 X;

    /// <summary>
    /// The basis matrix's Y vector (column 1). Equivalent to array index <c>[1]</c>.
    /// </summary>
    public Vector2 Y;

    /// <summary>
    /// The origin vector (column 2, the third column). Equivalent to array index <c>[2]</c>.
    /// The origin vector represents translation.
    /// </summary>
    public Vector2 Origin;

    private static readonly Transform2D _identity = new Transform2D(1f, 0.0f, 0.0f, 1f, 0.0f, 0.0f);
    private static readonly Transform2D _flipX = new Transform2D(-1f, 0.0f, 0.0f, 1f, 0.0f, 0.0f);
    private static readonly Transform2D _flipY = new Transform2D(1f, 0.0f, 0.0f, -1f, 0.0f, 0.0f);

    /// <summary>Returns the transform's rotation (in radians).</summary>
    public readonly Radians Rotation => new (Mathf.Atan2(this.X.Y, this.X.X));

    /// <summary>Returns the scale.</summary>
    public readonly Vector2 Scale
    {
        get
        {
            float num = (float)Mathf.Sign(this.BasisDeterminant());
            return new Vector2(this.X.Length(), num * this.Y.Length());
        }
    }

    /// <summary>Returns the transform's skew (in radians).</summary>
    public readonly Radians Skew
    {
        get
        {
            float num = (float)Mathf.Sign(this.BasisDeterminant());
            return Mathf.Acos(this.X.Normalized().Dot(num * this.Y.Normalized())) - 1.5707964f;
        }
    }

    /// <summary>
    /// Access whole columns in the form of <see cref="T:Godot.Vector2" />.
    /// The third column is the <see cref="F:Godot.Transform2D.Origin" /> vector.
    /// </summary>
    /// <param name="column">Which column vector.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="column" /> is not 0, 1 or 2.
    /// </exception>
    public Vector2 this[int column]
    {
        readonly get
        {
            switch (column)
            {
                case 0:
                    return this.X;
                case 1:
                    return this.Y;
                case 2:
                    return this.Origin;
                default:
                    throw new ArgumentOutOfRangeException(nameof(column));
            }
        }
        set
        {
            switch (column)
            {
                case 0:
                    this.X = value;
                    break;
                case 1:
                    this.Y = value;
                    break;
                case 2:
                    this.Origin = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(column));
            }
        }
    }

    /// <summary>
    /// Access matrix elements in column-major order.
    /// The third column is the <see cref="F:Godot.Transform2D.Origin" /> vector.
    /// </summary>
    /// <param name="column">Which column, the matrix horizontal position.</param>
    /// <param name="row">Which row, the matrix vertical position.</param>
    public float this[int column, int row]
    {
        readonly get => this[column][row];
        set
        {
            Vector2 vector2 = this[column];
            vector2[row] = value;
            this[column] = vector2;
        }
    }

    /// <summary>
    /// Returns the inverse of the transform, under the assumption that
    /// the basis is invertible (must have non-zero determinant).
    /// </summary>
    /// <seealso cref="M:Godot.Transform2D.Inverse" />
    /// <returns>The inverse transformation matrix.</returns>
    public readonly Transform2D AffineInverse()
    {
        float num1 = this.BasisDeterminant();
        if ((double)num1 == 0.0)
            throw new InvalidOperationException("Matrix determinant is zero and cannot be inverted.");
        Transform2D transform2D = this;
        transform2D[0, 0] = this[1, 1];
        transform2D[1, 1] = this[0, 0];
        float num2 = 1f / num1;
        transform2D[0] *= new Vector2(num2, -num2);
        transform2D[1] *= new Vector2(-num2, num2);
        transform2D[2] = transform2D.BasisXform(-transform2D[2]);
        return transform2D;
    }

    /// <summary>
    /// Returns the determinant of the basis matrix. If the basis is
    /// uniformly scaled, its determinant is the square of the scale.
    /// 
    /// A negative determinant means the Y scale is negative.
    /// A zero determinant means the basis isn't invertible,
    /// and is usually considered invalid.
    /// </summary>
    /// <returns>The determinant of the basis matrix.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly float BasisDeterminant()
    {
        return (float)((double)this.X.X * (double)this.Y.Y - (double)this.X.Y * (double)this.Y.X);
    }

    /// <summary>
    /// Returns a vector transformed (multiplied) by the basis matrix.
    /// This method does not account for translation (the <see cref="F:Godot.Transform2D.Origin" /> vector).
    /// </summary>
    /// <seealso cref="M:Godot.Transform2D.BasisXformInv(Godot.Vector2)" />
    /// <param name="v">A vector to transform.</param>
    /// <returns>The transformed vector.</returns>
    public readonly Vector2 BasisXform(Vector2 v) => new Vector2(this.Tdotx(v), this.Tdoty(v));
    
    /// <summary>
    /// TODO I have no idea what does it do
    /// </summary>
    /// <param name="v">A vector to transform.</param>
    /// <returns>The transformed vector.</returns>
    public readonly Vector2 Xform(Vector2 v) => new Vector2(Tdotx(v), Tdoty(v)) + Origin;

    /// <summary>
    /// Returns a vector transformed (multiplied) by the inverse basis matrix,
    /// under the assumption that the basis is orthonormal (i.e. rotation/reflection
    /// is fine, scaling/skew is not).
    /// This method does not account for translation (the <see cref="F:Godot.Transform2D.Origin" /> vector).
    /// <c>transform.BasisXformInv(vector)</c> is equivalent to <c>transform.Inverse().BasisXform(vector)</c>. See <see cref="M:Godot.Transform2D.Inverse" />.
    /// For non-orthonormal transforms (e.g. with scaling) <c>transform.AffineInverse().BasisXform(vector)</c> can be used instead. See <see cref="M:Godot.Transform2D.AffineInverse" />.
    /// </summary>
    /// <seealso cref="M:Godot.Transform2D.BasisXform(Godot.Vector2)" />
    /// <param name="v">A vector to inversely transform.</param>
    /// <returns>The inversely transformed vector.</returns>
    public readonly Vector2 BasisXformInv(Vector2 v) => new Vector2(this.X.Dot(v), this.Y.Dot(v));

    /// <summary>
    /// Interpolates this transform to the other <paramref name="transform" /> by <paramref name="weight" />.
    /// </summary>
    /// <param name="transform">The other transform.</param>
    /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <returns>The interpolated transform.</returns>
    public readonly Transform2D InterpolateWith(Transform2D transform, float weight)
    {
        return new Transform2D(Mathf.LerpAngle(this.Rotation, transform.Rotation, weight), this.Scale.Lerp(transform.Scale, weight), Mathf.LerpAngle(this.Skew, transform.Skew, weight),
            this.Origin.Lerp(transform.Origin, weight));
    }

    /// <summary>
    /// Returns the inverse of the transform, under the assumption that
    /// the transformation basis is orthonormal (i.e. rotation/reflection
    /// is fine, scaling/skew is not). Use <see cref="M:Godot.Transform2D.AffineInverse" /> for
    /// non-orthonormal transforms (e.g. with scaling).
    /// </summary>
    /// <returns>The inverse matrix.</returns>
    public readonly Transform2D Inverse()
    {
        Transform2D transform2D = this;
        transform2D.X.Y = this.Y.X;
        transform2D.Y.X = this.X.Y;
        transform2D.Origin = transform2D.BasisXform(-transform2D.Origin);
        return transform2D;
    }

    /// <summary>
    /// Returns <see langword="true" /> if this transform is finite, by calling
    /// <see cref="M:Godot.Mathf.IsFinite(System.Single)" /> on each component.
    /// </summary>
    /// <returns>Whether this vector is finite or not.</returns>
    public readonly bool IsFinite()
    {
        return this.X.IsFinite() && this.Y.IsFinite() && this.Origin.IsFinite();
    }

    /// <summary>
    /// Returns the transform with the basis orthogonal (90 degrees),
    /// and normalized axis vectors (scale of 1 or -1).
    /// </summary>
    /// <returns>The orthonormalized transform.</returns>
    public readonly Transform2D Orthonormalized()
    {
        Transform2D transform2D = this;
        Vector2 x = transform2D.X;
        Vector2 y = transform2D.Y;
        x.Normalize();
        Vector2 vector2 = y - x * x.Dot(y);
        vector2.Normalize();
        transform2D.X = x;
        transform2D.Y = vector2;
        return transform2D;
    }

    /// <summary>
    /// Rotates the transform by <paramref name="angle" /> (in radians).
    /// The operation is done in the parent/global frame, equivalent to
    /// multiplying the matrix from the left.
    /// </summary>
    /// <param name="angle">The angle to rotate, in radians.</param>
    /// <returns>The rotated transformation matrix.</returns>
    public readonly Transform2D Rotated(Radians angle)
    {
        return new Transform2D(angle, new Vector2()) * this;
    }

    /// <summary>
    /// Rotates the transform by <paramref name="angle" /> (in radians).
    /// The operation is done in the local frame, equivalent to
    /// multiplying the matrix from the right.
    /// </summary>
    /// <param name="angle">The angle to rotate, in radians.</param>
    /// <returns>The rotated transformation matrix.</returns>
    public readonly Transform2D RotatedLocal(float angle)
    {
        return this * new Transform2D(angle, new Vector2());
    }

    /// <summary>
    /// Scales the transform by the given scaling factor.
    /// The operation is done in the parent/global frame, equivalent to
    /// multiplying the matrix from the left.
    /// </summary>
    /// <param name="scale">The scale to introduce.</param>
    /// <returns>The scaled transformation matrix.</returns>
    public readonly Transform2D Scaled(Vector2 scale)
    {
        Transform2D transform2D = this;
        transform2D.X *= scale;
        transform2D.Y *= scale;
        transform2D.Origin *= scale;
        return transform2D;
    }

    /// <summary>
    /// Scales the transform by the given scaling factor.
    /// The operation is done in the local frame, equivalent to
    /// multiplying the matrix from the right.
    /// </summary>
    /// <param name="scale">The scale to introduce.</param>
    /// <returns>The scaled transformation matrix.</returns>
    public readonly Transform2D ScaledLocal(Vector2 scale)
    {
        Transform2D transform2D = this;
        transform2D.X *= scale;
        transform2D.Y *= scale;
        return transform2D;
    }

    private readonly float Tdotx(Vector2 with)
    {
        return (float)((double)this[0, 0] * (double)with[0] + (double)this[1, 0] * (double)with[1]);
    }

    private readonly float Tdoty(Vector2 with)
    {
        return (float)((double)this[0, 1] * (double)with[0] + (double)this[1, 1] * (double)with[1]);
    }

    /// <summary>
    /// Translates the transform by the given <paramref name="offset" />.
    /// The operation is done in the parent/global frame, equivalent to
    /// multiplying the matrix from the left.
    /// </summary>
    /// <param name="offset">The offset to translate by.</param>
    /// <returns>The translated matrix.</returns>
    public readonly Transform2D Translated(Vector2 offset)
    {
        Transform2D transform2D = this;
        transform2D.Origin += offset;
        return transform2D;
    }

    /// <summary>
    /// Translates the transform by the given <paramref name="offset" />.
    /// The operation is done in the local frame, equivalent to
    /// multiplying the matrix from the right.
    /// </summary>
    /// <param name="offset">The offset to translate by.</param>
    /// <returns>The translated matrix.</returns>
    public readonly Transform2D TranslatedLocal(Vector2 offset)
    {
        Transform2D transform2D = this;
        transform2D.Origin += transform2D.BasisXform(offset);
        return transform2D;
    }

    /// <summary>
    /// The identity transform, with no translation, rotation, or scaling applied.
    /// This is used as a replacement for <c>Transform2D()</c> in GDScript.
    /// Do not use <c>new Transform2D()</c> with no arguments in C#, because it sets all values to zero.
    /// </summary>
    /// <value>Equivalent to <c>new Transform2D(Vector2.Right, Vector2.Down, Vector2.Zero)</c>.</value>
    public static Transform2D Identity => _identity;

    /// <summary>
    /// The transform that will flip something along the X axis.
    /// </summary>
    /// <value>Equivalent to <c>new Transform2D(Vector2.Left, Vector2.Down, Vector2.Zero)</c>.</value>
    public static Transform2D FlipX => _flipX;

    /// <summary>
    /// The transform that will flip something along the Y axis.
    /// </summary>
    /// <value>Equivalent to <c>new Transform2D(Vector2.Right, Vector2.Up, Vector2.Zero)</c>.</value>
    public static Transform2D FlipY => _flipY;

    /// <summary>
    /// Constructs a transformation matrix from 3 vectors (matrix columns).
    /// </summary>
    /// <param name="xAxis">The X vector, or column index 0.</param>
    /// <param name="yAxis">The Y vector, or column index 1.</param>
    /// <param name="originPos">The origin vector, or column index 2.</param>
    public Transform2D(Vector2 xAxis, Vector2 yAxis, Vector2 originPos)
    {
        this.X = xAxis;
        this.Y = yAxis;
        this.Origin = originPos;
    }

    /// <summary>
    /// Constructs a transformation matrix from the given components.
    /// Arguments are named such that xy is equal to calling <c>X.Y</c>.
    /// </summary>
    /// <param name="xx">The X component of the X column vector, accessed via <c>t.X.X</c> or <c>[0][0]</c>.</param>
    /// <param name="xy">The Y component of the X column vector, accessed via <c>t.X.Y</c> or <c>[0][1]</c>.</param>
    /// <param name="yx">The X component of the Y column vector, accessed via <c>t.Y.X</c> or <c>[1][0]</c>.</param>
    /// <param name="yy">The Y component of the Y column vector, accessed via <c>t.Y.Y</c> or <c>[1][1]</c>.</param>
    /// <param name="ox">The X component of the origin vector, accessed via <c>t.Origin.X</c> or <c>[2][0]</c>.</param>
    /// <param name="oy">The Y component of the origin vector, accessed via <c>t.Origin.Y</c> or <c>[2][1]</c>.</param>
    public Transform2D(float xx, float xy, float yx, float yy, float ox, float oy)
    {
        this.X = new Vector2(xx, xy);
        this.Y = new Vector2(yx, yy);
        this.Origin = new Vector2(ox, oy);
    }

    /// <summary>
    /// Constructs a transformation matrix from a <paramref name="rotation" /> value and
    /// <paramref name="origin" /> vector.
    /// </summary>
    /// <param name="rotation">The rotation of the new transform, in radians.</param>
    /// <param name="origin">The origin vector, or column index 2.</param>
    public Transform2D(float rotation, Vector2 origin)
    {
        (float Sin, float Cos) tuple = Mathf.SinCos(rotation);
        float sin = tuple.Sin;
        this.X.X = this.Y.Y = tuple.Cos;
        this.X.Y = this.Y.X = sin;
        this.Y.X *= -1f;
        this.Origin = origin;
    }

    /// <summary>
    /// Constructs a transformation matrix from a <paramref name="rotation" /> value,
    /// <paramref name="scale" /> vector, <paramref name="skew" /> value, and
    /// <paramref name="origin" /> vector.
    /// </summary>
    /// <param name="rotation">The rotation of the new transform, in radians.</param>
    /// <param name="scale">The scale of the new transform.</param>
    /// <param name="skew">The skew of the new transform, in radians.</param>
    /// <param name="origin">The origin vector, or column index 2.</param>
    public Transform2D(Radians rotation, Vector2 scale, Radians skew, Vector2 origin)
    {
        (float Sin1, float Cos1) = Mathf.SinCos(rotation);
        (float Sin2, float Cos2) = Mathf.SinCos(rotation + skew);
        this.X.X = Cos1 * scale.X;
        this.Y.Y = Cos2 * scale.Y;
        this.Y.X = -Sin2 * scale.Y;
        this.X.Y = Sin1 * scale.X;
        this.Origin = origin;
    }

    /// <summary>
    /// Composes these two transformation matrices by multiplying them
    /// together. This has the effect of transforming the second transform
    /// (the child) by the first transform (the parent).
    /// </summary>
    /// <param name="left">The parent transform.</param>
    /// <param name="right">The child transform.</param>
    /// <returns>The composed transform.</returns>
    public static Transform2D operator *(Transform2D left, Transform2D right)
    {
        left.Origin = left * right.Origin;
        float num1 = left.Tdotx(right.X);
        float num2 = left.Tdoty(right.X);
        float num3 = left.Tdotx(right.Y);
        float num4 = left.Tdoty(right.Y);
        left.X.X = num1;
        left.X.Y = num2;
        left.Y.X = num3;
        left.Y.Y = num4;
        return left;
    }

    /// <summary>
    /// Returns a Vector2 transformed (multiplied) by the transformation matrix.
    /// </summary>
    /// <param name="transform">The transformation to apply.</param>
    /// <param name="vector">A Vector2 to transform.</param>
    /// <returns>The transformed Vector2.</returns>
    public static Vector2 operator *(Transform2D transform, Vector2 vector)
    {
        return new Vector2(transform.Tdotx(vector), transform.Tdoty(vector)) + transform.Origin;
    }

    /// <summary>
    /// Returns a Vector2 transformed (multiplied) by the inverse transformation matrix,
    /// under the assumption that the transformation basis is orthonormal (i.e. rotation/reflection
    /// is fine, scaling/skew is not).
    /// <c>vector * transform</c> is equivalent to <c>transform.Inverse() * vector</c>. See <see cref="M:Godot.Transform2D.Inverse" />.
    /// For transforming by inverse of an affine transformation (e.g. with scaling) <c>transform.AffineInverse() * vector</c> can be used instead. See <see cref="M:Godot.Transform2D.AffineInverse" />.
    /// </summary>
    /// <param name="vector">A Vector2 to inversely transform.</param>
    /// <param name="transform">The transformation to apply.</param>
    /// <returns>The inversely transformed Vector2.</returns>
    public static Vector2 operator *(Vector2 vector, Transform2D transform)
    {
        Vector2 with = vector - transform.Origin;
        return new Vector2(transform.X.Dot(with), transform.Y.Dot(with));
    }

    /// <summary>
    /// Returns a Rect2 transformed (multiplied) by the transformation matrix.
    /// </summary>
    /// <param name="transform">The transformation to apply.</param>
    /// <param name="rect">A Rect2 to transform.</param>
    /// <returns>The transformed Rect2.</returns>
    public static Rect2 operator *(Transform2D transform, Rect2 rect)
    {
        Vector2 position = transform * rect.Position;
        Vector2 vector2_1 = transform.X * rect.Size.X;
        Vector2 vector2_2 = transform.Y * rect.Size.Y;
        Rect2 rect2 = new Rect2(position, new Vector2());
        rect2 = rect2.Expand(position + vector2_1);
        rect2 = rect2.Expand(position + vector2_2);
        return rect2.Expand(position + vector2_1 + vector2_2);
    }

    /// <summary>
    /// Returns a Rect2 transformed (multiplied) by the inverse transformation matrix,
    /// under the assumption that the transformation basis is orthonormal (i.e. rotation/reflection
    /// is fine, scaling/skew is not).
    /// <c>rect * transform</c> is equivalent to <c>transform.Inverse() * rect</c>. See <see cref="M:Godot.Transform2D.Inverse" />.
    /// For transforming by inverse of an affine transformation (e.g. with scaling) <c>transform.AffineInverse() * rect</c> can be used instead. See <see cref="M:Godot.Transform2D.AffineInverse" />.
    /// </summary>
    /// <param name="rect">A Rect2 to inversely transform.</param>
    /// <param name="transform">The transformation to apply.</param>
    /// <returns>The inversely transformed Rect2.</returns>
    public static Rect2 operator *(Rect2 rect, Transform2D transform)
    {
        Vector2 position = rect.Position * transform;
        Vector2 to1 = new Vector2(rect.Position.X, rect.Position.Y + rect.Size.Y) * transform;
        Vector2 to2 = new Vector2(rect.Position.X + rect.Size.X, rect.Position.Y + rect.Size.Y) * transform;
        Vector2 to3 = new Vector2(rect.Position.X + rect.Size.X, rect.Position.Y) * transform;
        Vector2 size = new Vector2();
        Rect2 rect2 = new Rect2(position, size);
        rect2 = rect2.Expand(to1);
        rect2 = rect2.Expand(to2);
        return rect2.Expand(to3);
    }

    /// <summary>
    /// Returns a copy of the given Vector2[] transformed (multiplied) by the transformation matrix.
    /// </summary>
    /// <param name="transform">The transformation to apply.</param>
    /// <param name="array">A Vector2[] to transform.</param>
    /// <returns>The transformed copy of the Vector2[].</returns>
    public static Vector2[] operator *(Transform2D transform, Vector2[] array)
    {
        Vector2[] vector2Array = new Vector2[array.Length];
        for (int index = 0; index < array.Length; ++index)
            vector2Array[index] = transform * array[index];
        return vector2Array;
    }

    /// <summary>
    /// Returns a copy of the given Vector2[] transformed (multiplied) by the inverse transformation matrix,
    /// under the assumption that the transformation basis is orthonormal (i.e. rotation/reflection
    /// is fine, scaling/skew is not).
    /// <c>array * transform</c> is equivalent to <c>transform.Inverse() * array</c>. See <see cref="M:Godot.Transform2D.Inverse" />.
    /// For transforming by inverse of an affine transformation (e.g. with scaling) <c>transform.AffineInverse() * array</c> can be used instead. See <see cref="M:Godot.Transform2D.AffineInverse" />.
    /// </summary>
    /// <param name="array">A Vector2[] to inversely transform.</param>
    /// <param name="transform">The transformation to apply.</param>
    /// <returns>The inversely transformed copy of the Vector2[].</returns>
    public static Vector2[] operator *(Vector2[] array, Transform2D transform)
    {
        Vector2[] vector2Array = new Vector2[array.Length];
        for (int index = 0; index < array.Length; ++index)
            vector2Array[index] = array[index] * transform;
        return vector2Array;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the transforms are exactly equal.
    /// Note: Due to floating-point precision errors, consider using
    /// <see cref="M:Godot.Transform2D.IsEqualApprox(Godot.Transform2D)" /> instead, which is more reliable.
    /// </summary>
    /// <param name="left">The left transform.</param>
    /// <param name="right">The right transform.</param>
    /// <returns>Whether or not the transforms are exactly equal.</returns>
    public static bool operator ==(Transform2D left, Transform2D right) => left.Equals(right);

    /// <summary>
    /// Returns <see langword="true" /> if the transforms are not equal.
    /// Note: Due to floating-point precision errors, consider using
    /// <see cref="M:Godot.Transform2D.IsEqualApprox(Godot.Transform2D)" /> instead, which is more reliable.
    /// </summary>
    /// <param name="left">The left transform.</param>
    /// <param name="right">The right transform.</param>
    /// <returns>Whether or not the transforms are not equal.</returns>
    public static bool operator !=(Transform2D left, Transform2D right) => !left.Equals(right);

    /// <summary>
    /// Returns <see langword="true" /> if the transform is exactly equal
    /// to the given object (<paramref name="obj" />).
    /// Note: Due to floating-point precision errors, consider using
    /// <see cref="M:Godot.Transform2D.IsEqualApprox(Godot.Transform2D)" /> instead, which is more reliable.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns>Whether or not the transform and the object are exactly equal.</returns>
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Transform2D other && this.Equals(other);
    }

    /// <summary>
    /// Returns <see langword="true" /> if the transforms are exactly equal.
    /// Note: Due to floating-point precision errors, consider using
    /// <see cref="M:Godot.Transform2D.IsEqualApprox(Godot.Transform2D)" /> instead, which is more reliable.
    /// </summary>
    /// <param name="other">The other transform to compare.</param>
    /// <returns>Whether or not the matrices are exactly equal.</returns>
    public readonly bool Equals(Transform2D other)
    {
        return this.X.Equals(other.X) && this.Y.Equals(other.Y) && this.Origin.Equals(other.Origin);
    }

    /// <summary>
    /// Returns <see langword="true" /> if this transform and <paramref name="other" /> are approximately equal,
    /// by running <see cref="M:Godot.Vector2.IsEqualApprox(Godot.Vector2)" /> on each component.
    /// </summary>
    /// <param name="other">The other transform to compare.</param>
    /// <returns>Whether or not the matrices are approximately equal.</returns>
    public readonly bool IsEqualApprox(Transform2D other)
    {
        return this.X.IsEqualApprox(other.X) && this.Y.IsEqualApprox(other.Y) && this.Origin.IsEqualApprox(other.Origin);
    }

    /// <summary>
    /// Serves as the hash function for <see cref="T:Godot.Transform2D" />.
    /// </summary>
    /// <returns>A hash code for this transform.</returns>
    public override readonly int GetHashCode()
    {
        return HashCode.Combine<Vector2, Vector2, Vector2>(this.X, this.Y, this.Origin);
    }

    /// <summary>
    /// Converts this <see cref="T:Godot.Transform2D" /> to a string.
    /// </summary>
    /// <returns>A string representation of this transform.</returns>
    public override readonly string ToString() => this.ToString((string)null);

    /// <summary>
    /// Converts this <see cref="T:Godot.Transform2D" /> to a string with the given <paramref name="format" />.
    /// </summary>
    /// <returns>A string representation of this transform.</returns>
    public readonly string ToString(string? format)
    {
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 3);
        interpolatedStringHandler.AppendLiteral("[X: ");
        interpolatedStringHandler.AppendFormatted(this.X.ToString(format));
        interpolatedStringHandler.AppendLiteral(", Y: ");
        interpolatedStringHandler.AppendFormatted(this.Y.ToString(format));
        interpolatedStringHandler.AppendLiteral(", O: ");
        interpolatedStringHandler.AppendFormatted(this.Origin.ToString(format));
        interpolatedStringHandler.AppendLiteral("]");
        return interpolatedStringHandler.ToStringAndClear();
    }
}