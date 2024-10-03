using System.Runtime.CompilerServices;

namespace Tmp.Math;

public static class Mathf
{
    /// <summary>
    /// The circle constant, the circumference of the unit circle in radians.
    /// </summary>
    public const float Tau = 6.2831855f;

    /// <summary>
    /// Constant that represents how many times the diameter of a circle
    /// fits around its perimeter. This is equivalent to <c>Mathf.Tau / 2</c>.
    /// </summary>
    public const float Pi = 3.1415927f;

    /// <summary>
    /// Positive infinity. For negative infinity, use <c>-Mathf.Inf</c>.
    /// </summary>
    public const float Inf = float.PositiveInfinity;

    /// <summary>
    /// "Not a Number", an invalid value. <c>NaN</c> has special properties, including
    /// that it is not equal to itself. It is output by some invalid operations,
    /// such as dividing zero by zero.
    /// </summary>
    public const float NaN = float.NaN;

    private const float DegToRadConstF = 0.017453292f;
    private const double DegToRadConstD = 0.017453292519943295;
    private const float RadToDegConstF = 57.29578f;
    private const double RadToDegConstD = 57.295779513082316;

    /// <summary>
    /// The natural number <c>e</c>.
    /// </summary>
    public const float E = 2.7182817f;

    /// <summary>The square root of 2.</summary>
    public const float Sqrt2 = 1.4142135f;

    private const float EpsilonF = 1E-06f;
    private const double EpsilonD = 1E-14;

    /// <summary>
    /// A very small number used for float comparison with error tolerance.
    /// 1e-06 with single-precision floats, but 1e-14 if <c>REAL_T_IS_DOUBLE</c>.
    /// </summary>
    public const float Epsilon = 1E-06f;

    /// <summary>
    /// Returns the absolute value of <paramref name="s" /> (i.e. positive value).
    /// </summary>
    /// <param name="s">The input number.</param>
    /// <returns>The absolute value of <paramref name="s" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Abs(int s) => System.Math.Abs(s);

    /// <summary>
    /// Returns the absolute value of <paramref name="s" /> (i.e. positive value).
    /// </summary>
    /// <param name="s">The input number.</param>
    /// <returns>The absolute value of <paramref name="s" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Abs(float s) => System.Math.Abs(s);

    /// <summary>
    /// Returns the absolute value of <paramref name="s" /> (i.e. positive value).
    /// </summary>
    /// <param name="s">The input number.</param>
    /// <returns>The absolute value of <paramref name="s" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Abs(double s) => System.Math.Abs(s);

    /// <summary>
    /// Returns the arc cosine of <paramref name="s" /> in radians.
    /// Use to get the angle of cosine <paramref name="s" />.
    /// </summary>
    /// <param name="s">The input cosine value. Must be on the range of -1.0 to 1.0.</param>
    /// <returns>
    /// An angle that would result in the given cosine value. On the range <c>0</c> to <c>Tau/2</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Acos(float s) => MathF.Acos(s);

    /// <summary>
    /// Returns the arc cosine of <paramref name="s" /> in radians.
    /// Use to get the angle of cosine <paramref name="s" />.
    /// </summary>
    /// <param name="s">The input cosine value. Must be on the range of -1.0 to 1.0.</param>
    /// <returns>
    /// An angle that would result in the given cosine value. On the range <c>0</c> to <c>Tau/2</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Acos(double s) => System.Math.Acos(s);

    /// <summary>
    /// Returns the hyperbolic arc (also called inverse) cosine of <paramref name="s" /> in radians.
    /// Use it to get the angle from an angle's cosine in hyperbolic space if
    /// <paramref name="s" /> is larger or equal to 1.
    /// </summary>
    /// <param name="s">The input hyperbolic cosine value.</param>
    /// <returns>
    /// An angle that would result in the given hyperbolic cosine value.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Acosh(float s) => MathF.Acosh(s);

    /// <summary>
    /// Returns the hyperbolic arc (also called inverse) cosine of <paramref name="s" /> in radians.
    /// Use it to get the angle from an angle's cosine in hyperbolic space if
    /// <paramref name="s" /> is larger or equal to 1.
    /// </summary>
    /// <param name="s">The input hyperbolic cosine value.</param>
    /// <returns>
    /// An angle that would result in the given hyperbolic cosine value.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Acosh(double s) => System.Math.Acosh(s);

    /// <summary>
    /// Returns the difference between the two angles,
    /// in range of -<see cref="F:Godot.Mathf.Pi" />, <see cref="F:Godot.Mathf.Pi" />.
    /// When <paramref name="from" /> and <paramref name="to" /> are opposite,
    /// returns -<see cref="F:Godot.Mathf.Pi" /> if <paramref name="from" /> is smaller than <paramref name="to" />,
    /// or <see cref="F:Godot.Mathf.Pi" /> otherwise.
    /// </summary>
    /// <param name="from">The start angle.</param>
    /// <param name="to">The destination angle.</param>
    /// <returns>The difference between the two angles.</returns>
    public static float AngleDifference(float from, float to)
    {
        float num = (float)(((double)to - (double)from) % 6.2831854820251465);
        return (float)(2.0 * (double)num % 6.2831854820251465) - num;
    }

    /// <summary>
    /// Returns the difference between the two angles,
    /// in range of -<see cref="F:Godot.Mathf.Pi" />, <see cref="F:Godot.Mathf.Pi" />.
    /// When <paramref name="from" /> and <paramref name="to" /> are opposite,
    /// returns -<see cref="F:Godot.Mathf.Pi" /> if <paramref name="from" /> is smaller than <paramref name="to" />,
    /// or <see cref="F:Godot.Mathf.Pi" /> otherwise.
    /// </summary>
    /// <param name="from">The start angle.</param>
    /// <param name="to">The destination angle.</param>
    /// <returns>The difference between the two angles.</returns>
    public static double AngleDifference(double from, double to)
    {
        double num = (to - from) % (2.0 * System.Math.PI);
        return 2.0 * num % (2.0 * System.Math.PI) - num;
    }

    /// <summary>
    /// Returns the arc sine of <paramref name="s" /> in radians.
    /// Use to get the angle of sine <paramref name="s" />.
    /// </summary>
    /// <param name="s">The input sine value. Must be on the range of -1.0 to 1.0.</param>
    /// <returns>
    /// An angle that would result in the given sine value. On the range <c>-Tau/4</c> to <c>Tau/4</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Asin(float s) => MathF.Asin(s);

    /// <summary>
    /// Returns the arc sine of <paramref name="s" /> in radians.
    /// Use to get the angle of sine <paramref name="s" />.
    /// </summary>
    /// <param name="s">The input sine value. Must be on the range of -1.0 to 1.0.</param>
    /// <returns>
    /// An angle that would result in the given sine value. On the range <c>-Tau/4</c> to <c>Tau/4</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Asin(double s) => System.Math.Asin(s);

    /// <summary>
    /// Returns the hyperbolic arc (also called inverse) sine of <paramref name="s" /> in radians.
    /// Use it to get the angle from an angle's sine in hyperbolic space if
    /// <paramref name="s" /> is larger or equal to 1.
    /// </summary>
    /// <param name="s">The input hyperbolic sine value.</param>
    /// <returns>
    /// An angle that would result in the given hyperbolic sine value.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Asinh(float s) => MathF.Asinh(s);

    /// <summary>
    /// Returns the hyperbolic arc (also called inverse) sine of <paramref name="s" /> in radians.
    /// Use it to get the angle from an angle's sine in hyperbolic space if
    /// <paramref name="s" /> is larger or equal to 1.
    /// </summary>
    /// <param name="s">The input hyperbolic sine value.</param>
    /// <returns>
    /// An angle that would result in the given hyperbolic sine value.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Asinh(double s) => System.Math.Asinh(s);

    /// <summary>
    /// Returns the arc tangent of <paramref name="s" /> in radians.
    /// Use to get the angle of tangent <paramref name="s" />.
    /// 
    /// The method cannot know in which quadrant the angle should fall.
    /// See <see cref="M:Godot.Mathf.Atan2(System.Single,System.Single)" /> if you have both <c>y</c> and <c>x</c>.
    /// </summary>
    /// <param name="s">The input tangent value.</param>
    /// <returns>
    /// An angle that would result in the given tangent value. On the range <c>-Tau/4</c> to <c>Tau/4</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Atan(float s) => MathF.Atan(s);

    /// <summary>
    /// Returns the arc tangent of <paramref name="s" /> in radians.
    /// Use to get the angle of tangent <paramref name="s" />.
    /// 
    /// The method cannot know in which quadrant the angle should fall.
    /// See <see cref="M:Godot.Mathf.Atan2(System.Double,System.Double)" /> if you have both <c>y</c> and <c>x</c>.
    /// </summary>
    /// <param name="s">The input tangent value.</param>
    /// <returns>
    /// An angle that would result in the given tangent value. On the range <c>-Tau/4</c> to <c>Tau/4</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Atan(double s) => System.Math.Atan(s);

    /// <summary>
    /// Returns the arc tangent of <paramref name="y" /> and <paramref name="x" /> in radians.
    /// Use to get the angle of the tangent of <c>y/x</c>. To compute the value, the method takes into
    /// account the sign of both arguments in order to determine the quadrant.
    /// 
    /// Important note: The Y coordinate comes first, by convention.
    /// </summary>
    /// <param name="y">The Y coordinate of the point to find the angle to.</param>
    /// <param name="x">The X coordinate of the point to find the angle to.</param>
    /// <returns>
    /// An angle that would result in the given tangent value. On the range <c>-Tau/2</c> to <c>Tau/2</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Atan2(float y, float x) => MathF.Atan2(y, x);

    /// <summary>
    /// Returns the arc tangent of <paramref name="y" /> and <paramref name="x" /> in radians.
    /// Use to get the angle of the tangent of <c>y/x</c>. To compute the value, the method takes into
    /// account the sign of both arguments in order to determine the quadrant.
    /// 
    /// Important note: The Y coordinate comes first, by convention.
    /// </summary>
    /// <param name="y">The Y coordinate of the point to find the angle to.</param>
    /// <param name="x">The X coordinate of the point to find the angle to.</param>
    /// <returns>
    /// An angle that would result in the given tangent value. On the range <c>-Tau/2</c> to <c>Tau/2</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Atan2(double y, double x) => System.Math.Atan2(y, x);

    /// <summary>
    /// Returns the hyperbolic arc (also called inverse) tangent of <paramref name="s" /> in radians.
    /// Use it to get the angle from an angle's tangent in hyperbolic space if
    /// <paramref name="s" /> is between -1 and 1 (non-inclusive).
    /// </summary>
    /// <param name="s">The input hyperbolic tangent value.</param>
    /// <returns>
    /// An angle that would result in the given hyperbolic tangent value.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Atanh(float s) => MathF.Atanh(s);

    /// <summary>
    /// Returns the hyperbolic arc (also called inverse) tangent of <paramref name="s" /> in radians.
    /// Use it to get the angle from an angle's tangent in hyperbolic space if
    /// <paramref name="s" /> is between -1 and 1 (non-inclusive).
    /// </summary>
    /// <param name="s">The input hyperbolic tangent value.</param>
    /// <returns>
    /// An angle that would result in the given hyperbolic tangent value.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Atanh(double s) => System.Math.Atanh(s);

    /// <summary>
    /// Rounds <paramref name="s" /> upward (towards positive infinity).
    /// </summary>
    /// <param name="s">The number to ceil.</param>
    /// <returns>The smallest whole number that is not less than <paramref name="s" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Ceil(float s) => MathF.Ceiling(s);

    /// <summary>
    /// Rounds <paramref name="s" /> upward (towards positive infinity).
    /// </summary>
    /// <param name="s">The number to ceil.</param>
    /// <returns>The smallest whole number that is not less than <paramref name="s" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Ceil(double s) => System.Math.Ceiling(s);

    /// <summary>
    /// Clamps a <paramref name="value" /> so that it is not less than <paramref name="min" />
    /// and not more than <paramref name="max" />.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <returns>The clamped value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(int value, int min, int max) => System.Math.Clamp(value, min, max);

    /// <summary>
    /// Clamps a <paramref name="value" /> so that it is not less than <paramref name="min" />
    /// and not more than <paramref name="max" />.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <returns>The clamped value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp(float value, float min, float max) => System.Math.Clamp(value, min, max);

    /// <summary>
    /// Clamps a <paramref name="value" /> so that it is not less than <paramref name="min" />
    /// and not more than <paramref name="max" />.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <returns>The clamped value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Clamp(double value, double min, double max) => System.Math.Clamp(value, min, max);

    /// <summary>
    /// Returns the cosine of angle <paramref name="s" /> in radians.
    /// </summary>
    /// <param name="s">The angle in radians.</param>
    /// <returns>The cosine of that angle.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cos(float s) => MathF.Cos(s);

    /// <summary>
    /// Returns the cosine of angle <paramref name="s" /> in radians.
    /// </summary>
    /// <param name="s">The angle in radians.</param>
    /// <returns>The cosine of that angle.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Cos(double s) => System.Math.Cos(s);

    /// <summary>
    /// Returns the hyperbolic cosine of angle <paramref name="s" /> in radians.
    /// </summary>
    /// <param name="s">The angle in radians.</param>
    /// <returns>The hyperbolic cosine of that angle.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cosh(float s) => MathF.Cosh(s);

    /// <summary>
    /// Returns the hyperbolic cosine of angle <paramref name="s" /> in radians.
    /// </summary>
    /// <param name="s">The angle in radians.</param>
    /// <returns>The hyperbolic cosine of that angle.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Cosh(double s) => System.Math.Cosh(s);

    /// <summary>
    /// Cubic interpolates between two values by the factor defined in <paramref name="weight" />
    /// with pre and post values.
    /// </summary>
    /// <param name="from">The start value for interpolation.</param>
    /// <param name="to">The destination value for interpolation.</param>
    /// <param name="pre">The value which before "from" value for interpolation.</param>
    /// <param name="post">The value which after "to" value for interpolation.</param>
    /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <returns>The resulting value of the interpolation.</returns>
    public static float CubicInterpolate(
        float from,
        float to,
        float pre,
        float post,
        float weight
    )
    {
        return (float)(0.5 * ((double)from * 2.0 + (-(double)pre + (double)to) * (double)weight +
                              (2.0 * (double)pre - 5.0 * (double)from + 4.0 * (double)to - (double)post) * ((double)weight * (double)weight) +
                              (-(double)pre + 3.0 * (double)from - 3.0 * (double)to + (double)post) * ((double)weight * (double)weight * (double)weight)));
    }

    /// <summary>
    /// Cubic interpolates between two values by the factor defined in <paramref name="weight" />
    /// with pre and post values.
    /// </summary>
    /// <param name="from">The start value for interpolation.</param>
    /// <param name="to">The destination value for interpolation.</param>
    /// <param name="pre">The value which before "from" value for interpolation.</param>
    /// <param name="post">The value which after "to" value for interpolation.</param>
    /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <returns>The resulting value of the interpolation.</returns>
    public static double CubicInterpolate(
        double from,
        double to,
        double pre,
        double post,
        double weight
    )
    {
        return 0.5 * (from * 2.0 + (-pre + to) * weight + (2.0 * pre - 5.0 * from + 4.0 * to - post) * (weight * weight) + (-pre + 3.0 * from - 3.0 * to + post) * (weight * weight * weight));
    }

    /// <summary>
    /// Cubic interpolates between two rotation values with shortest path
    /// by the factor defined in <paramref name="weight" /> with pre and post values.
    /// See also <see cref="M:Godot.Mathf.LerpAngle(System.Single,System.Single,System.Single)" />.
    /// </summary>
    /// <param name="from">The start value for interpolation.</param>
    /// <param name="to">The destination value for interpolation.</param>
    /// <param name="pre">The value which before "from" value for interpolation.</param>
    /// <param name="post">The value which after "to" value for interpolation.</param>
    /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <returns>The resulting value of the interpolation.</returns>
    public static float CubicInterpolateAngle(
        float from,
        float to,
        float pre,
        float post,
        float weight
    )
    {
        float from1 = from % 6.2831855f;
        float num1 = (float)(((double)pre - (double)from1) % 6.2831854820251465);
        float pre1 = from1 + (float)(2.0 * (double)num1 % 6.2831854820251465) - num1;
        float num2 = (float)(((double)to - (double)from1) % 6.2831854820251465);
        float to1 = from1 + (float)(2.0 * (double)num2 % 6.2831854820251465) - num2;
        float num3 = (float)(((double)post - (double)to1) % 6.2831854820251465);
        float post1 = to1 + (float)(2.0 * (double)num3 % 6.2831854820251465) - num3;
        return Mathf.CubicInterpolate(from1, to1, pre1, post1, weight);
    }

    /// <summary>
    /// Cubic interpolates between two rotation values with shortest path
    /// by the factor defined in <paramref name="weight" /> with pre and post values.
    /// See also <see cref="M:Godot.Mathf.LerpAngle(System.Double,System.Double,System.Double)" />.
    /// </summary>
    /// <param name="from">The start value for interpolation.</param>
    /// <param name="to">The destination value for interpolation.</param>
    /// <param name="pre">The value which before "from" value for interpolation.</param>
    /// <param name="post">The value which after "to" value for interpolation.</param>
    /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <returns>The resulting value of the interpolation.</returns>
    public static double CubicInterpolateAngle(
        double from,
        double to,
        double pre,
        double post,
        double weight
    )
    {
        double from1 = from % (2.0 * System.Math.PI);
        double num1 = (pre - from1) % (2.0 * System.Math.PI);
        double pre1 = from1 + 2.0 * num1 % (2.0 * System.Math.PI) - num1;
        double num2 = (to - from1) % (2.0 * System.Math.PI);
        double to1 = from1 + 2.0 * num2 % (2.0 * System.Math.PI) - num2;
        double num3 = (post - to1) % (2.0 * System.Math.PI);
        double post1 = to1 + 2.0 * num3 % (2.0 * System.Math.PI) - num3;
        return Mathf.CubicInterpolate(from1, to1, pre1, post1, weight);
    }

    /// <summary>
    /// Cubic interpolates between two values by the factor defined in <paramref name="weight" />
    /// with pre and post values.
    /// It can perform smoother interpolation than
    /// <see cref="M:Godot.Mathf.CubicInterpolate(System.Single,System.Single,System.Single,System.Single,System.Single)" />
    /// by the time values.
    /// </summary>
    /// <param name="from">The start value for interpolation.</param>
    /// <param name="to">The destination value for interpolation.</param>
    /// <param name="pre">The value which before "from" value for interpolation.</param>
    /// <param name="post">The value which after "to" value for interpolation.</param>
    /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <param name="toT"></param>
    /// <param name="preT"></param>
    /// <param name="postT"></param>
    /// <returns>The resulting value of the interpolation.</returns>
    public static float CubicInterpolateInTime(
        float from,
        float to,
        float pre,
        float post,
        float weight,
        float toT,
        float preT,
        float postT
    )
    {
        float num = Mathf.Lerp(0.0f, toT, weight);
        double from1 = (double)Mathf.Lerp(pre, from, (double)preT == 0.0 ? 0.0f : (float)(((double)num - (double)preT) / -(double)preT));
        float from2 = Mathf.Lerp(from, to, (double)toT == 0.0 ? 0.5f : num / toT);
        float to1 = Mathf.Lerp(to, post, (double)postT - (double)toT == 0.0 ? 1f : (float)(((double)num - (double)toT) / ((double)postT - (double)toT)));
        double to2 = (double)from2;
        double weight1 = (double)toT - (double)preT == 0.0 ? 0.0 : ((double)num - (double)preT) / ((double)toT - (double)preT);
        return Mathf.Lerp(Mathf.Lerp((float)from1, (float)to2, (float)weight1), Mathf.Lerp(from2, to1, (double)postT == 0.0 ? 1f : num / postT), (double)toT == 0.0 ? 0.5f : num / toT);
    }

    /// <summary>
    /// Cubic interpolates between two values by the factor defined in <paramref name="weight" />
    /// with pre and post values.
    /// It can perform smoother interpolation than
    /// <see cref="M:Godot.Mathf.CubicInterpolate(System.Double,System.Double,System.Double,System.Double,System.Double)" />
    /// by the time values.
    /// </summary>
    /// <param name="from">The start value for interpolation.</param>
    /// <param name="to">The destination value for interpolation.</param>
    /// <param name="pre">The value which before "from" value for interpolation.</param>
    /// <param name="post">The value which after "to" value for interpolation.</param>
    /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <param name="toT"></param>
    /// <param name="preT"></param>
    /// <param name="postT"></param>
    /// <returns>The resulting value of the interpolation.</returns>
    public static double CubicInterpolateInTime(
        double from,
        double to,
        double pre,
        double post,
        double weight,
        double toT,
        double preT,
        double postT
    )
    {
        double num = Mathf.Lerp(0.0, toT, weight);
        double from1 = Mathf.Lerp(pre, from, preT == 0.0 ? 0.0 : (num - preT) / -preT);
        double from2 = Mathf.Lerp(from, to, toT == 0.0 ? 0.5 : num / toT);
        double to1 = Mathf.Lerp(to, post, postT - toT == 0.0 ? 1.0 : (num - toT) / (postT - toT));
        double to2 = from2;
        double weight1 = toT - preT == 0.0 ? 0.0 : (num - preT) / (toT - preT);
        return Mathf.Lerp(Mathf.Lerp(from1, to2, weight1), Mathf.Lerp(from2, to1, postT == 0.0 ? 1.0 : num / postT), toT == 0.0 ? 0.5 : num / toT);
    }

    /// <summary>
    /// Cubic interpolates between two rotation values with shortest path
    /// by the factor defined in <paramref name="weight" /> with pre and post values.
    /// See also <see cref="M:Godot.Mathf.LerpAngle(System.Single,System.Single,System.Single)" />.
    /// It can perform smoother interpolation than
    /// <see cref="M:Godot.Mathf.CubicInterpolateAngle(System.Single,System.Single,System.Single,System.Single,System.Single)" />
    /// by the time values.
    /// </summary>
    /// <param name="from">The start value for interpolation.</param>
    /// <param name="to">The destination value for interpolation.</param>
    /// <param name="pre">The value which before "from" value for interpolation.</param>
    /// <param name="post">The value which after "to" value for interpolation.</param>
    /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <param name="toT"></param>
    /// <param name="preT"></param>
    /// <param name="postT"></param>
    /// <returns>The resulting value of the interpolation.</returns>
    public static float CubicInterpolateAngleInTime(
        float from,
        float to,
        float pre,
        float post,
        float weight,
        float toT,
        float preT,
        float postT
    )
    {
        float from1 = from % 6.2831855f;
        float num1 = (float)(((double)pre - (double)from1) % 6.2831854820251465);
        float pre1 = from1 + (float)(2.0 * (double)num1 % 6.2831854820251465) - num1;
        float num2 = (float)(((double)to - (double)from1) % 6.2831854820251465);
        float to1 = from1 + (float)(2.0 * (double)num2 % 6.2831854820251465) - num2;
        float num3 = (float)(((double)post - (double)to1) % 6.2831854820251465);
        float post1 = to1 + (float)(2.0 * (double)num3 % 6.2831854820251465) - num3;
        return Mathf.CubicInterpolateInTime(from1, to1, pre1, post1, weight, toT, preT, postT);
    }

    /// <summary>
    /// Cubic interpolates between two rotation values with shortest path
    /// by the factor defined in <paramref name="weight" /> with pre and post values.
    /// See also <see cref="M:Godot.Mathf.LerpAngle(System.Double,System.Double,System.Double)" />.
    /// It can perform smoother interpolation than
    /// <see cref="M:Godot.Mathf.CubicInterpolateAngle(System.Double,System.Double,System.Double,System.Double,System.Double)" />
    /// by the time values.
    /// </summary>
    /// <param name="from">The start value for interpolation.</param>
    /// <param name="to">The destination value for interpolation.</param>
    /// <param name="pre">The value which before "from" value for interpolation.</param>
    /// <param name="post">The value which after "to" value for interpolation.</param>
    /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <param name="toT"></param>
    /// <param name="preT"></param>
    /// <param name="postT"></param>
    /// <returns>The resulting value of the interpolation.</returns>
    public static double CubicInterpolateAngleInTime(
        double from,
        double to,
        double pre,
        double post,
        double weight,
        double toT,
        double preT,
        double postT
    )
    {
        double from1 = from % (2.0 * System.Math.PI);
        double num1 = (pre - from1) % (2.0 * System.Math.PI);
        double pre1 = from1 + 2.0 * num1 % (2.0 * System.Math.PI) - num1;
        double num2 = (to - from1) % (2.0 * System.Math.PI);
        double to1 = from1 + 2.0 * num2 % (2.0 * System.Math.PI) - num2;
        double num3 = (post - to1) % (2.0 * System.Math.PI);
        double post1 = to1 + 2.0 * num3 % (2.0 * System.Math.PI) - num3;
        return Mathf.CubicInterpolateInTime(from1, to1, pre1, post1, weight, toT, preT, postT);
    }

    /// <summary>
    /// Returns the point at the given <paramref name="t" /> on a one-dimensional Bezier curve defined by
    /// the given <paramref name="control1" />, <paramref name="control2" />, and <paramref name="end" /> points.
    /// </summary>
    /// <param name="start">The start value for the interpolation.</param>
    /// <param name="control1">Control point that defines the bezier curve.</param>
    /// <param name="control2">Control point that defines the bezier curve.</param>
    /// <param name="end">The destination value for the interpolation.</param>
    /// <param name="t">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <returns>The resulting value of the interpolation.</returns>
    public static float BezierInterpolate(
        float start,
        float control1,
        float control2,
        float end,
        float t
    )
    {
        float num1 = 1f - t;
        float num2 = num1 * num1;
        float num3 = num2 * num1;
        float num4 = t * t;
        float num5 = num4 * t;
        return (float)((double)start * (double)num3 + (double)control1 * (double)num2 * (double)t * 3.0 + (double)control2 * (double)num1 * (double)num4 * 3.0 + (double)end * (double)num5);
    }

    /// <summary>
    /// Returns the point at the given <paramref name="t" /> on a one-dimensional Bezier curve defined by
    /// the given <paramref name="control1" />, <paramref name="control2" />, and <paramref name="end" /> points.
    /// </summary>
    /// <param name="start">The start value for the interpolation.</param>
    /// <param name="control1">Control point that defines the bezier curve.</param>
    /// <param name="control2">Control point that defines the bezier curve.</param>
    /// <param name="end">The destination value for the interpolation.</param>
    /// <param name="t">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <returns>The resulting value of the interpolation.</returns>
    public static double BezierInterpolate(
        double start,
        double control1,
        double control2,
        double end,
        double t
    )
    {
        double num1 = 1.0 - t;
        double num2 = num1 * num1;
        double num3 = num2 * num1;
        double num4 = t * t;
        double num5 = num4 * t;
        return start * num3 + control1 * num2 * t * 3.0 + control2 * num1 * num4 * 3.0 + end * num5;
    }

    /// <summary>
    /// Returns the derivative at the given <paramref name="t" /> on a one dimensional Bezier curve defined by
    /// the given <paramref name="control1" />, <paramref name="control2" />, and <paramref name="end" /> points.
    /// </summary>
    /// <param name="start">The start value for the interpolation.</param>
    /// <param name="control1">Control point that defines the bezier curve.</param>
    /// <param name="control2">Control point that defines the bezier curve.</param>
    /// <param name="end">The destination value for the interpolation.</param>
    /// <param name="t">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <returns>The resulting value of the interpolation.</returns>
    public static float BezierDerivative(
        float start,
        float control1,
        float control2,
        float end,
        float t
    )
    {
        float num1 = 1f - t;
        float num2 = num1 * num1;
        float num3 = t * t;
        return (float)(((double)control1 - (double)start) * 3.0 * (double)num2 + ((double)control2 - (double)control1) * 6.0 * (double)num1 * (double)t +
                       ((double)end - (double)control2) * 3.0 * (double)num3);
    }

    /// <summary>
    /// Returns the derivative at the given <paramref name="t" /> on a one dimensional Bezier curve defined by
    /// the given <paramref name="control1" />, <paramref name="control2" />, and <paramref name="end" /> points.
    /// </summary>
    /// <param name="start">The start value for the interpolation.</param>
    /// <param name="control1">Control point that defines the bezier curve.</param>
    /// <param name="control2">Control point that defines the bezier curve.</param>
    /// <param name="end">The destination value for the interpolation.</param>
    /// <param name="t">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <returns>The resulting value of the interpolation.</returns>
    public static double BezierDerivative(
        double start,
        double control1,
        double control2,
        double end,
        double t
    )
    {
        double num1 = 1.0 - t;
        double num2 = num1 * num1;
        double num3 = t * t;
        return (control1 - start) * 3.0 * num2 + (control2 - control1) * 6.0 * num1 * t + (end - control2) * 3.0 * num3;
    }

    /// <summary>Converts from decibels to linear energy (audio).</summary>
    /// <seealso cref="M:Godot.Mathf.LinearToDb(System.Single)" />
    /// <param name="db">Decibels to convert.</param>
    /// <returns>Audio volume as linear energy.</returns>
    public static float DbToLinear(float db) => MathF.Exp(db * 0.115129255f);

    /// <summary>Converts from decibels to linear energy (audio).</summary>
    /// <seealso cref="M:Godot.Mathf.LinearToDb(System.Double)" />
    /// <param name="db">Decibels to convert.</param>
    /// <returns>Audio volume as linear energy.</returns>
    public static double DbToLinear(double db) => System.Math.Exp(db * 0.11512925464970228);

    /// <summary>Converts an angle expressed in degrees to radians.</summary>
    /// <param name="deg">An angle expressed in degrees.</param>
    /// <returns>The same angle expressed in radians.</returns>
    public static float DegToRad(float deg) => deg * ((float)System.Math.PI / 180f);

    /// <summary>Converts an angle expressed in degrees to radians.</summary>
    /// <param name="deg">An angle expressed in degrees.</param>
    /// <returns>The same angle expressed in radians.</returns>
    public static double DegToRad(double deg) => deg * (System.Math.PI / 180.0);

    /// <summary>
    /// Easing function, based on exponent. The <paramref name="curve" /> values are:
    /// <c>0</c> is constant, <c>1</c> is linear, <c>0</c> to <c>1</c> is ease-in, <c>1</c> or more is ease-out.
    /// Negative values are in-out/out-in.
    /// </summary>
    /// <param name="s">The value to ease.</param>
    /// <param name="curve">
    /// <c>0</c> is constant, <c>1</c> is linear, <c>0</c> to <c>1</c> is ease-in, <c>1</c> or more is ease-out.
    /// </param>
    /// <returns>The eased value.</returns>
    public static float Ease(float s, float curve)
    {
        if ((double)s < 0.0)
            s = 0.0f;
        else if ((double)s > 1.0)
            s = 1f;
        if ((double)curve > 0.0)
            return (double)curve < 1.0 ? 1f - MathF.Pow(1f - s, 1f / curve) : MathF.Pow(s, curve);
        if ((double)curve >= 0.0)
            return 0.0f;
        return (double)s < 0.5 ? MathF.Pow(s * 2f, -curve) * 0.5f : (float)((1.0 - (double)MathF.Pow((float)(1.0 - ((double)s - 0.5) * 2.0), -curve)) * 0.5 + 0.5);
    }

    /// <summary>
    /// Easing function, based on exponent. The <paramref name="curve" /> values are:
    /// <c>0</c> is constant, <c>1</c> is linear, <c>0</c> to <c>1</c> is ease-in, <c>1</c> or more is ease-out.
    /// Negative values are in-out/out-in.
    /// </summary>
    /// <param name="s">The value to ease.</param>
    /// <param name="curve">
    /// <c>0</c> is constant, <c>1</c> is linear, <c>0</c> to <c>1</c> is ease-in, <c>1</c> or more is ease-out.
    /// </param>
    /// <returns>The eased value.</returns>
    public static double Ease(double s, double curve)
    {
        if (s < 0.0)
            s = 0.0;
        else if (s > 1.0)
            s = 1.0;
        if (curve > 0.0)
            return curve < 1.0 ? 1.0 - System.Math.Pow(1.0 - s, 1.0 / curve) : System.Math.Pow(s, curve);
        if (curve >= 0.0)
            return 0.0;
        return s < 0.5 ? System.Math.Pow(s * 2.0, -curve) * 0.5 : (1.0 - System.Math.Pow(1.0 - (s - 0.5) * 2.0, -curve)) * 0.5 + 0.5;
    }

    /// <summary>
    /// The natural exponential function. It raises the mathematical
    /// constant <c>e</c> to the power of <paramref name="s" /> and returns it.
    /// </summary>
    /// <param name="s">The exponent to raise <c>e</c> to.</param>
    /// <returns><c>e</c> raised to the power of <paramref name="s" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Exp(float s) => MathF.Exp(s);

    /// <summary>
    /// The natural exponential function. It raises the mathematical
    /// constant <c>e</c> to the power of <paramref name="s" /> and returns it.
    /// </summary>
    /// <param name="s">The exponent to raise <c>e</c> to.</param>
    /// <returns><c>e</c> raised to the power of <paramref name="s" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Exp(double s) => System.Math.Exp(s);

    /// <summary>
    /// Rounds <paramref name="s" /> downward (towards negative infinity).
    /// </summary>
    /// <param name="s">The number to floor.</param>
    /// <returns>The largest whole number that is not more than <paramref name="s" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Floor(float s) => MathF.Floor(s);

    /// <summary>
    /// Rounds <paramref name="s" /> downward (towards negative infinity).
    /// </summary>
    /// <param name="s">The number to floor.</param>
    /// <returns>The largest whole number that is not more than <paramref name="s" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Floor(double s) => System.Math.Floor(s);

    /// <summary>
    /// Returns a normalized value considering the given range.
    /// This is the opposite of <see cref="M:Godot.Mathf.Lerp(System.Single,System.Single,System.Single)" />.
    /// </summary>
    /// <param name="from">The start value for interpolation.</param>
    /// <param name="to">The destination value for interpolation.</param>
    /// <param name="weight">The interpolated value.</param>
    /// <returns>
    /// The resulting value of the inverse interpolation.
    /// The returned value will be between 0.0 and 1.0 if <paramref name="weight" /> is
    /// between <paramref name="from" /> and <paramref name="to" /> (inclusive).
    /// </returns>
    public static float InverseLerp(float from, float to, float weight)
    {
        return (float)(((double)weight - (double)from) / ((double)to - (double)from));
    }

    /// <summary>
    /// Returns a normalized value considering the given range.
    /// This is the opposite of <see cref="M:Godot.Mathf.Lerp(System.Double,System.Double,System.Double)" />.
    /// </summary>
    /// <param name="from">The start value for interpolation.</param>
    /// <param name="to">The destination value for interpolation.</param>
    /// <param name="weight">The interpolated value.</param>
    /// <returns>
    /// The resulting value of the inverse interpolation.
    /// The returned value will be between 0.0 and 1.0 if <paramref name="weight" /> is
    /// between <paramref name="from" /> and <paramref name="to" /> (inclusive).
    /// </returns>
    public static double InverseLerp(double from, double to, double weight)
    {
        return (weight - from) / (to - from);
    }

    /// <summary>
    /// Returns <see langword="true" /> if <paramref name="a" /> and <paramref name="b" /> are approximately equal
    /// to each other.
    /// The comparison is done using a tolerance calculation with <see cref="F:Godot.Mathf.Epsilon" />.
    /// </summary>
    /// <param name="a">One of the values.</param>
    /// <param name="b">The other value.</param>
    /// <returns>A <see langword="bool" /> for whether or not the two values are approximately equal.</returns>
    public static bool IsEqualApprox(float a, float b)
    {
        if ((double)a == (double)b)
            return true;
        float num = 1E-06f * System.Math.Abs(a);
        if ((double)num < 9.999999974752427E-07)
            num = 1E-06f;
        return (double)System.Math.Abs(a - b) < (double)num;
    }

    /// <summary>
    /// Returns <see langword="true" /> if <paramref name="a" /> and <paramref name="b" /> are approximately equal
    /// to each other.
    /// The comparison is done using a tolerance calculation with <see cref="F:Godot.Mathf.Epsilon" />.
    /// </summary>
    /// <param name="a">One of the values.</param>
    /// <param name="b">The other value.</param>
    /// <returns>A <see langword="bool" /> for whether or not the two values are approximately equal.</returns>
    public static bool IsEqualApprox(double a, double b)
    {
        if (a == b)
            return true;
        double num = 1E-14 * System.Math.Abs(a);
        if (num < 1E-14)
            num = 1E-14;
        return System.Math.Abs(a - b) < num;
    }

    /// <summary>
    /// Returns whether <paramref name="s" /> is a finite value, i.e. it is not
    /// <see cref="F:Godot.Mathf.NaN" />, positive infinite, or negative infinity.
    /// </summary>
    /// <param name="s">The value to check.</param>
    /// <returns>A <see langword="bool" /> for whether or not the value is a finite value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFinite(float s) => float.IsFinite(s);

    /// <summary>
    /// Returns whether <paramref name="s" /> is a finite value, i.e. it is not
    /// <see cref="F:Godot.Mathf.NaN" />, positive infinite, or negative infinity.
    /// </summary>
    /// <param name="s">The value to check.</param>
    /// <returns>A <see langword="bool" /> for whether or not the value is a finite value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFinite(double s) => double.IsFinite(s);

    /// <summary>
    /// Returns whether <paramref name="s" /> is an infinity value (either positive infinity or negative infinity).
    /// </summary>
    /// <param name="s">The value to check.</param>
    /// <returns>A <see langword="bool" /> for whether or not the value is an infinity value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInf(float s) => float.IsInfinity(s);

    /// <summary>
    /// Returns whether <paramref name="s" /> is an infinity value (either positive infinity or negative infinity).
    /// </summary>
    /// <param name="s">The value to check.</param>
    /// <returns>A <see langword="bool" /> for whether or not the value is an infinity value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInf(double s) => double.IsInfinity(s);

    /// <summary>
    /// Returns whether <paramref name="s" /> is a <c>NaN</c> ("Not a Number" or invalid) value.
    /// </summary>
    /// <param name="s">The value to check.</param>
    /// <returns>A <see langword="bool" /> for whether or not the value is a <c>NaN</c> value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNaN(float s) => float.IsNaN(s);

    /// <summary>
    /// Returns whether <paramref name="s" /> is a <c>NaN</c> ("Not a Number" or invalid) value.
    /// </summary>
    /// <param name="s">The value to check.</param>
    /// <returns>A <see langword="bool" /> for whether or not the value is a <c>NaN</c> value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNaN(double s) => double.IsNaN(s);

    /// <summary>
    /// Returns <see langword="true" /> if <paramref name="s" /> is zero or almost zero.
    /// The comparison is done using a tolerance calculation with <see cref="F:Godot.Mathf.Epsilon" />.
    /// 
    /// This method is faster than using <see cref="M:Godot.Mathf.IsEqualApprox(System.Single,System.Single)" /> with
    /// one value as zero.
    /// </summary>
    /// <param name="s">The value to check.</param>
    /// <returns>A <see langword="bool" /> for whether or not the value is nearly zero.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZeroApprox(float s) => (double)System.Math.Abs(s) < 9.999999974752427E-07;

    /// <summary>
    /// Returns <see langword="true" /> if <paramref name="s" /> is zero or almost zero.
    /// The comparison is done using a tolerance calculation with <see cref="F:Godot.Mathf.Epsilon" />.
    /// 
    /// This method is faster than using <see cref="M:Godot.Mathf.IsEqualApprox(System.Double,System.Double)" /> with
    /// one value as zero.
    /// </summary>
    /// <param name="s">The value to check.</param>
    /// <returns>A <see langword="bool" /> for whether or not the value is nearly zero.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZeroApprox(double s) => System.Math.Abs(s) < 1E-14;

    /// <summary>
    /// Linearly interpolates between two values by a normalized value.
    /// This is the opposite <see cref="M:Godot.Mathf.InverseLerp(System.Single,System.Single,System.Single)" />.
    /// </summary>
    /// <param name="from">The start value for interpolation.</param>
    /// <param name="to">The destination value for interpolation.</param>
    /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <returns>The resulting value of the interpolation.</returns>
    public static float Lerp(float from, float to, float weight) => from + (to - from) * weight;

    /// <summary>
    /// Linearly interpolates between two values by a normalized value.
    /// This is the opposite <see cref="M:Godot.Mathf.InverseLerp(System.Double,System.Double,System.Double)" />.
    /// </summary>
    /// <param name="from">The start value for interpolation.</param>
    /// <param name="to">The destination value for interpolation.</param>
    /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <returns>The resulting value of the interpolation.</returns>
    public static double Lerp(double from, double to, double weight) => from + (to - from) * weight;

    /// <summary>
    /// Linearly interpolates between two angles (in radians) by a normalized value.
    /// 
    /// Similar to <see cref="M:Godot.Mathf.Lerp(System.Single,System.Single,System.Single)" />,
    /// but interpolates correctly when the angles wrap around <see cref="F:Godot.Mathf.Tau" />.
    /// </summary>
    /// <param name="from">The start angle for interpolation.</param>
    /// <param name="to">The destination angle for interpolation.</param>
    /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <returns>The resulting angle of the interpolation.</returns>
    public static float LerpAngle(float from, float to, float weight)
    {
        return from + Mathf.AngleDifference(from, to) * weight;
    }

    /// <summary>
    /// Linearly interpolates between two angles (in radians) by a normalized value.
    /// 
    /// Similar to <see cref="M:Godot.Mathf.Lerp(System.Double,System.Double,System.Double)" />,
    /// but interpolates correctly when the angles wrap around <see cref="F:Godot.Mathf.Tau" />.
    /// </summary>
    /// <param name="from">The start angle for interpolation.</param>
    /// <param name="to">The destination angle for interpolation.</param>
    /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <returns>The resulting angle of the interpolation.</returns>
    public static double LerpAngle(double from, double to, double weight)
    {
        return from + Mathf.AngleDifference(from, to) * weight;
    }

    /// <summary>
    /// Converts from linear energy to decibels (audio).
    /// This can be used to implement volume sliders that behave as expected (since volume isn't linear).
    /// </summary>
    /// <seealso cref="M:Godot.Mathf.DbToLinear(System.Single)" />
    /// <example>
    /// <code>
    /// // "slider" refers to a node that inherits Range such as HSlider or VSlider.
    /// // Its range must be configured to go from 0 to 1.
    /// // Change the bus name if you'd like to change the volume of a specific bus only.
    /// AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), GD.LinearToDb(slider.value));
    /// </code>
    /// </example>
    /// <param name="linear">The linear energy to convert.</param>
    /// <returns>Audio as decibels.</returns>
    public static float LinearToDb(float linear) => MathF.Log(linear) * 8.685889f;

    /// <summary>
    /// Converts from linear energy to decibels (audio).
    /// This can be used to implement volume sliders that behave as expected (since volume isn't linear).
    /// </summary>
    /// <seealso cref="M:Godot.Mathf.DbToLinear(System.Double)" />
    /// <example>
    /// <code>
    /// // "slider" refers to a node that inherits Range such as HSlider or VSlider.
    /// // Its range must be configured to go from 0 to 1.
    /// // Change the bus name if you'd like to change the volume of a specific bus only.
    /// AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), GD.LinearToDb(slider.value));
    /// </code>
    /// </example>
    /// <param name="linear">The linear energy to convert.</param>
    /// <returns>Audio as decibels.</returns>
    public static double LinearToDb(double linear) => System.Math.Log(linear) * 8.685889638065037;

    /// <summary>
    /// Natural logarithm. The amount of time needed to reach a certain level of continuous growth.
    /// 
    /// Note: This is not the same as the "log" function on most calculators, which uses a base 10 logarithm.
    /// </summary>
    /// <param name="s">The input value.</param>
    /// <returns>The natural log of <paramref name="s" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Log(float s) => MathF.Log(s);

    /// <summary>
    /// Natural logarithm. The amount of time needed to reach a certain level of continuous growth.
    /// 
    /// Note: This is not the same as the "log" function on most calculators, which uses a base 10 logarithm.
    /// </summary>
    /// <param name="s">The input value.</param>
    /// <returns>The natural log of <paramref name="s" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Log(double s) => System.Math.Log(s);

    /// <summary>Returns the maximum of two values.</summary>
    /// <param name="a">One of the values.</param>
    /// <param name="b">The other value.</param>
    /// <returns>Whichever of the two values is higher.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Max(int a, int b) => System.Math.Max(a, b);

    /// <summary>Returns the maximum of two values.</summary>
    /// <param name="a">One of the values.</param>
    /// <param name="b">The other value.</param>
    /// <returns>Whichever of the two values is higher.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Max(float a, float b) => System.Math.Max(a, b);

    /// <summary>Returns the maximum of two values.</summary>
    /// <param name="a">One of the values.</param>
    /// <param name="b">The other value.</param>
    /// <returns>Whichever of the two values is higher.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Max(double a, double b) => System.Math.Max(a, b);

    /// <summary>Returns the minimum of two values.</summary>
    /// <param name="a">One of the values.</param>
    /// <param name="b">The other value.</param>
    /// <returns>Whichever of the two values is lower.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Min(int a, int b) => System.Math.Min(a, b);

    /// <summary>Returns the minimum of two values.</summary>
    /// <param name="a">One of the values.</param>
    /// <param name="b">The other value.</param>
    /// <returns>Whichever of the two values is lower.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Min(float a, float b) => System.Math.Min(a, b);

    /// <summary>Returns the minimum of two values.</summary>
    /// <param name="a">One of the values.</param>
    /// <param name="b">The other value.</param>
    /// <returns>Whichever of the two values is lower.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Min(double a, double b) => System.Math.Min(a, b);

    /// <summary>
    /// Moves <paramref name="from" /> toward <paramref name="to" /> by the <paramref name="delta" /> value.
    /// 
    /// Use a negative <paramref name="delta" /> value to move away.
    /// </summary>
    /// <param name="from">The start value.</param>
    /// <param name="to">The value to move towards.</param>
    /// <param name="delta">The amount to move by.</param>
    /// <returns>The value after moving.</returns>
    public static float MoveToward(float from, float to, float delta)
    {
        return (double)System.Math.Abs(to - from) <= (double)delta ? to : from + (float)System.Math.Sign(to - from) * delta;
    }

    /// <summary>
    /// Moves <paramref name="from" /> toward <paramref name="to" /> by the <paramref name="delta" /> value.
    /// 
    /// Use a negative <paramref name="delta" /> value to move away.
    /// </summary>
    /// <param name="from">The start value.</param>
    /// <param name="to">The value to move towards.</param>
    /// <param name="delta">The amount to move by.</param>
    /// <returns>The value after moving.</returns>
    public static double MoveToward(double from, double to, double delta)
    {
        return System.Math.Abs(to - from) <= delta ? to : from + (double)System.Math.Sign(to - from) * delta;
    }

    /// <summary>
    /// Returns the nearest larger power of 2 for the integer <paramref name="value" />.
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <returns>The nearest larger power of 2.</returns>
    public static int NearestPo2(int value)
    {
        --value;
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;
        ++value;
        return value;
    }

    /// <summary>
    /// Performs a canonical Modulus operation, where the output is on the range [0, <paramref name="b" />).
    /// </summary>
    /// <param name="a">The dividend, the primary input.</param>
    /// <param name="b">The divisor. The output is on the range [0, <paramref name="b" />).</param>
    /// <returns>The resulting output.</returns>
    public static int PosMod(int a, int b)
    {
        int num = a % b;
        if (num < 0 && b > 0 || num > 0 && b < 0)
            num += b;
        return num;
    }

    /// <summary>
    /// Performs a canonical Modulus operation, where the output is on the range [0, <paramref name="b" />).
    /// </summary>
    /// <param name="a">The dividend, the primary input.</param>
    /// <param name="b">The divisor. The output is on the range [0, <paramref name="b" />).</param>
    /// <returns>The resulting output.</returns>
    public static float PosMod(float a, float b)
    {
        float num = a % b;
        if ((double)num < 0.0 && (double)b > 0.0 || (double)num > 0.0 && (double)b < 0.0)
            num += b;
        return num;
    }

    /// <summary>
    /// Performs a canonical Modulus operation, where the output is on the range [0, <paramref name="b" />).
    /// </summary>
    /// <param name="a">The dividend, the primary input.</param>
    /// <param name="b">The divisor. The output is on the range [0, <paramref name="b" />).</param>
    /// <returns>The resulting output.</returns>
    public static double PosMod(double a, double b)
    {
        double num = a % b;
        if (num < 0.0 && b > 0.0 || num > 0.0 && b < 0.0)
            num += b;
        return num;
    }

    /// <summary>
    /// Returns the result of <paramref name="x" /> raised to the power of <paramref name="y" />.
    /// </summary>
    /// <param name="x">The base.</param>
    /// <param name="y">The exponent.</param>
    /// <returns><paramref name="x" /> raised to the power of <paramref name="y" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Pow(float x, float y) => MathF.Pow(x, y);

    /// <summary>
    /// Returns the result of <paramref name="x" /> raised to the power of <paramref name="y" />.
    /// </summary>
    /// <param name="x">The base.</param>
    /// <param name="y">The exponent.</param>
    /// <returns><paramref name="x" /> raised to the power of <paramref name="y" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Pow(double x, double y) => System.Math.Pow(x, y);

    /// <summary>Converts an angle expressed in radians to degrees.</summary>
    /// <param name="rad">An angle expressed in radians.</param>
    /// <returns>The same angle expressed in degrees.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float RadToDeg(float rad) => rad * 57.29578f;

    /// <summary>Converts an angle expressed in radians to degrees.</summary>
    /// <param name="rad">An angle expressed in radians.</param>
    /// <returns>The same angle expressed in degrees.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double RadToDeg(double rad) => rad * (180.0 / System.Math.PI);

    /// <summary>
    /// Maps a <paramref name="value" /> from [<paramref name="inFrom" />, <paramref name="inTo" />]
    /// to [<paramref name="outFrom" />, <paramref name="outTo" />].
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="inFrom">The start value for the input interpolation.</param>
    /// <param name="inTo">The destination value for the input interpolation.</param>
    /// <param name="outFrom">The start value for the output interpolation.</param>
    /// <param name="outTo">The destination value for the output interpolation.</param>
    /// <returns>The resulting mapped value mapped.</returns>
    public static float Remap(float value, float inFrom, float inTo, float outFrom, float outTo)
    {
        return Mathf.Lerp(outFrom, outTo, Mathf.InverseLerp(inFrom, inTo, value));
    }

    /// <summary>
    /// Maps a <paramref name="value" /> from [<paramref name="inFrom" />, <paramref name="inTo" />]
    /// to [<paramref name="outFrom" />, <paramref name="outTo" />].
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="inFrom">The start value for the input interpolation.</param>
    /// <param name="inTo">The destination value for the input interpolation.</param>
    /// <param name="outFrom">The start value for the output interpolation.</param>
    /// <param name="outTo">The destination value for the output interpolation.</param>
    /// <returns>The resulting mapped value mapped.</returns>
    public static double Remap(
        double value,
        double inFrom,
        double inTo,
        double outFrom,
        double outTo
    )
    {
        return Mathf.Lerp(outFrom, outTo, Mathf.InverseLerp(inFrom, inTo, value));
    }

    /// <summary>
    /// Rotates <paramref name="from" /> toward <paramref name="to" /> by the <paramref name="delta" /> amount. Will not go past <paramref name="to" />.
    /// Similar to <see cref="M:Godot.Mathf.MoveToward(System.Single,System.Single,System.Single)" /> but interpolates correctly when the angles wrap around <see cref="F:Godot.Mathf.Tau" />.
    /// If <paramref name="delta" /> is negative, this function will rotate away from <paramref name="to" />, toward the opposite angle, and will not go past the opposite angle.
    /// </summary>
    /// <param name="from">The start angle.</param>
    /// <param name="to">The angle to move towards.</param>
    /// <param name="delta">The amount to move by.</param>
    /// <returns>The angle after moving.</returns>
    public static float RotateToward(float from, float to, float delta)
    {
        float num = Mathf.AngleDifference(from, to);
        float max = System.Math.Abs(num);
        return from + System.Math.Clamp(delta, max - 3.1415927f, max) * ((double)num >= 0.0 ? 1f : -1f);
    }

    /// <summary>
    /// Rotates <paramref name="from" /> toward <paramref name="to" /> by the <paramref name="delta" /> amount. Will not go past <paramref name="to" />.
    /// Similar to <see cref="M:Godot.Mathf.MoveToward(System.Double,System.Double,System.Double)" /> but interpolates correctly when the angles wrap around <see cref="F:Godot.Mathf.Tau" />.
    /// If <paramref name="delta" /> is negative, this function will rotate away from <paramref name="to" />, toward the opposite angle, and will not go past the opposite angle.
    /// </summary>
    /// <param name="from">The start angle.</param>
    /// <param name="to">The angle to move towards.</param>
    /// <param name="delta">The amount to move by.</param>
    /// <returns>The angle after moving.</returns>
    public static double RotateToward(double from, double to, double delta)
    {
        double num = Mathf.AngleDifference(from, to);
        double max = System.Math.Abs(num);
        return from + System.Math.Clamp(delta, max - System.Math.PI, max) * (num >= 0.0 ? 1.0 : -1.0);
    }

    /// <summary>
    /// Rounds <paramref name="s" /> to the nearest whole number,
    /// with halfway cases rounded towards the nearest multiple of two.
    /// </summary>
    /// <param name="s">The number to round.</param>
    /// <returns>The rounded number.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Round(float s) => MathF.Round(s);

    /// <summary>
    /// Rounds <paramref name="s" /> to the nearest whole number,
    /// with halfway cases rounded towards the nearest multiple of two.
    /// </summary>
    /// <param name="s">The number to round.</param>
    /// <returns>The rounded number.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Round(double s) => System.Math.Round(s);

    /// <summary>
    /// Returns the sign of <paramref name="s" />: <c>-1</c> or <c>1</c>.
    /// Returns <c>0</c> if <paramref name="s" /> is <c>0</c>.
    /// </summary>
    /// <param name="s">The input number.</param>
    /// <returns>One of three possible values: <c>1</c>, <c>-1</c>, or <c>0</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign(int s) => System.Math.Sign(s);

    /// <summary>
    /// Returns the sign of <paramref name="s" />: <c>-1</c> or <c>1</c>.
    /// Returns <c>0</c> if <paramref name="s" /> is <c>0</c>.
    /// </summary>
    /// <param name="s">The input number.</param>
    /// <returns>One of three possible values: <c>1</c>, <c>-1</c>, or <c>0</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign(float s) => System.Math.Sign(s);

    /// <summary>
    /// Returns the sign of <paramref name="s" />: <c>-1</c> or <c>1</c>.
    /// Returns <c>0</c> if <paramref name="s" /> is <c>0</c>.
    /// </summary>
    /// <param name="s">The input number.</param>
    /// <returns>One of three possible values: <c>1</c>, <c>-1</c>, or <c>0</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign(double s) => System.Math.Sign(s);

    /// <summary>
    /// Returns the sine of angle <paramref name="s" /> in radians.
    /// </summary>
    /// <param name="s">The angle in radians.</param>
    /// <returns>The sine of that angle.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sin(float s) => MathF.Sin(s);

    /// <summary>
    /// Returns the sine of angle <paramref name="s" /> in radians.
    /// </summary>
    /// <param name="s">The angle in radians.</param>
    /// <returns>The sine of that angle.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Sin(double s) => System.Math.Sin(s);

    /// <summary>
    /// Returns the hyperbolic sine of angle <paramref name="s" /> in radians.
    /// </summary>
    /// <param name="s">The angle in radians.</param>
    /// <returns>The hyperbolic sine of that angle.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sinh(float s) => MathF.Sinh(s);

    /// <summary>
    /// Returns the hyperbolic sine of angle <paramref name="s" /> in radians.
    /// </summary>
    /// <param name="s">The angle in radians.</param>
    /// <returns>The hyperbolic sine of that angle.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Sinh(double s) => System.Math.Sinh(s);

    /// <summary>
    /// Returns a number smoothly interpolated between <paramref name="from" /> and <paramref name="to" />,
    /// based on the <paramref name="weight" />. Similar to <see cref="M:Godot.Mathf.Lerp(System.Single,System.Single,System.Single)" />,
    /// but interpolates faster at the beginning and slower at the end.
    /// </summary>
    /// <param name="from">The start value for interpolation.</param>
    /// <param name="to">The destination value for interpolation.</param>
    /// <param name="weight">A value representing the amount of interpolation.</param>
    /// <returns>The resulting value of the interpolation.</returns>
    public static float SmoothStep(float from, float to, float weight)
    {
        if (Mathf.IsEqualApprox(from, to))
            return from;
        float num = System.Math.Clamp((float)(((double)weight - (double)from) / ((double)to - (double)from)), 0.0f, 1f);
        return (float)((double)num * (double)num * (3.0 - 2.0 * (double)num));
    }

    /// <summary>
    /// Returns a number smoothly interpolated between <paramref name="from" /> and <paramref name="to" />,
    /// based on the <paramref name="weight" />. Similar to <see cref="M:Godot.Mathf.Lerp(System.Double,System.Double,System.Double)" />,
    /// but interpolates faster at the beginning and slower at the end.
    /// </summary>
    /// <param name="from">The start value for interpolation.</param>
    /// <param name="to">The destination value for interpolation.</param>
    /// <param name="weight">A value representing the amount of interpolation.</param>
    /// <returns>The resulting value of the interpolation.</returns>
    public static double SmoothStep(double from, double to, double weight)
    {
        if (Mathf.IsEqualApprox(from, to))
            return from;
        double num = System.Math.Clamp((weight - from) / (to - from), 0.0, 1.0);
        return num * num * (3.0 - 2.0 * num);
    }

    /// <summary>
    /// Returns the square root of <paramref name="s" />, where <paramref name="s" /> is a non-negative number.
    /// 
    /// If you need negative inputs, use <see cref="T:System.Numerics.Complex" />.
    /// </summary>
    /// <param name="s">The input number. Must not be negative.</param>
    /// <returns>The square root of <paramref name="s" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sqrt(float s) => MathF.Sqrt(s);

    /// <summary>
    /// Returns the square root of <paramref name="s" />, where <paramref name="s" /> is a non-negative number.
    /// 
    /// If you need negative inputs, use <see cref="T:System.Numerics.Complex" />.
    /// </summary>
    /// <param name="s">The input number. Must not be negative.</param>
    /// <returns>The square root of <paramref name="s" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Sqrt(double s) => System.Math.Sqrt(s);

    /// <summary>
    /// Returns the position of the first non-zero digit, after the
    /// decimal point. Note that the maximum return value is 10,
    /// which is a design decision in the implementation.
    /// </summary>
    /// <param name="step">The input value.</param>
    /// <returns>The position of the first non-zero digit.</returns>
    public static int StepDecimals(double step)
    {
        double[] numArray = new double[9]
        {
            0.9999,
            0.09999,
            0.009999,
            0.0009999,
            9.999E-05,
            9.999E-06,
            9.999E-07,
            9.999E-08,
            9.999E-09
        };
        double num1;
        double num2 = (num1 = System.Math.Abs(step)) - num1;
        for (int index = 0; index < numArray.Length; ++index)
        {
            if (num2 >= numArray[index])
                return index;
        }
        return 0;
    }

    /// <summary>
    /// Snaps float value <paramref name="s" /> to a given <paramref name="step" />.
    /// This can also be used to round a floating point number to an arbitrary number of decimals.
    /// </summary>
    /// <param name="s">The value to snap.</param>
    /// <param name="step">The step size to snap to.</param>
    /// <returns>The snapped value.</returns>
    public static float Snapped(float s, float step)
    {
        return (double)step != 0.0 ? MathF.Floor((float)((double)s / (double)step + 0.5)) * step : s;
    }

    /// <summary>
    /// Snaps float value <paramref name="s" /> to a given <paramref name="step" />.
    /// This can also be used to round a floating point number to an arbitrary number of decimals.
    /// </summary>
    /// <param name="s">The value to snap.</param>
    /// <param name="step">The step size to snap to.</param>
    /// <returns>The snapped value.</returns>
    public static double Snapped(double s, double step)
    {
        return step != 0.0 ? System.Math.Floor(s / step + 0.5) * step : s;
    }

    /// <summary>
    /// Returns the tangent of angle <paramref name="s" /> in radians.
    /// </summary>
    /// <param name="s">The angle in radians.</param>
    /// <returns>The tangent of that angle.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Tan(float s) => MathF.Tan(s);

    /// <summary>
    /// Returns the tangent of angle <paramref name="s" /> in radians.
    /// </summary>
    /// <param name="s">The angle in radians.</param>
    /// <returns>The tangent of that angle.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Tan(double s) => System.Math.Tan(s);

    /// <summary>
    /// Returns the hyperbolic tangent of angle <paramref name="s" /> in radians.
    /// </summary>
    /// <param name="s">The angle in radians.</param>
    /// <returns>The hyperbolic tangent of that angle.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Tanh(float s) => MathF.Tanh(s);

    /// <summary>
    /// Returns the hyperbolic tangent of angle <paramref name="s" /> in radians.
    /// </summary>
    /// <param name="s">The angle in radians.</param>
    /// <returns>The hyperbolic tangent of that angle.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Tanh(double s) => System.Math.Tanh(s);

    /// <summary>
    /// Wraps <paramref name="value" /> between <paramref name="min" /> and <paramref name="max" />.
    /// Usable for creating loop-alike behavior or infinite surfaces.
    /// If <paramref name="min" /> is <c>0</c>, this is equivalent
    /// to <see cref="M:Godot.Mathf.PosMod(System.Int32,System.Int32)" />, so prefer using that instead.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <param name="min">The minimum allowed value and lower bound of the range.</param>
    /// <param name="max">The maximum allowed value and upper bound of the range.</param>
    /// <returns>The wrapped value.</returns>
    public static int Wrap(int value, int min, int max)
    {
        int num = max - min;
        return num == 0 ? min : min + ((value - min) % num + num) % num;
    }

    /// <summary>
    /// Wraps <paramref name="value" /> between <paramref name="min" /> and <paramref name="max" />.
    /// Usable for creating loop-alike behavior or infinite surfaces.
    /// If <paramref name="min" /> is <c>0</c>, this is equivalent
    /// to <see cref="M:Godot.Mathf.PosMod(System.Single,System.Single)" />, so prefer using that instead.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <param name="min">The minimum allowed value and lower bound of the range.</param>
    /// <param name="max">The maximum allowed value and upper bound of the range.</param>
    /// <returns>The wrapped value.</returns>
    public static float Wrap(float value, float min, float max)
    {
        float s = max - min;
        return Mathf.IsZeroApprox(s) ? min : min + ((value - min) % s + s) % s;
    }

    /// <summary>
    /// Wraps <paramref name="value" /> between <paramref name="min" /> and <paramref name="max" />.
    /// Usable for creating loop-alike behavior or infinite surfaces.
    /// If <paramref name="min" /> is <c>0</c>, this is equivalent
    /// to <see cref="M:Godot.Mathf.PosMod(System.Double,System.Double)" />, so prefer using that instead.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <param name="min">The minimum allowed value and lower bound of the range.</param>
    /// <param name="max">The maximum allowed value and upper bound of the range.</param>
    /// <returns>The wrapped value.</returns>
    public static double Wrap(double value, double min, double max)
    {
        double s = max - min;
        return Mathf.IsZeroApprox(s) ? min : min + ((value - min) % s + s) % s;
    }

    /// <summary>
    /// Returns the <paramref name="value" /> wrapped between <c>0</c> and the <paramref name="length" />.
    /// If the limit is reached, the next value the function returned is decreased to the <c>0</c> side
    /// or increased to the <paramref name="length" /> side (like a triangle wave).
    /// If <paramref name="length" /> is less than zero, it becomes positive.
    /// </summary>
    /// <param name="value">The value to pingpong.</param>
    /// <param name="length">The maximum value of the function.</param>
    /// <returns>The ping-ponged value.</returns>
    public static float PingPong(float value, float length)
    {
        return (double)length == 0.0 ? 0.0f : System.Math.Abs((float)((double)Fract((float)(((double)value - (double)length) / ((double)length * 2.0))) * (double)length * 2.0) - length);

        static float Fract(float value) => value - MathF.Floor(value);
    }

    /// <summary>
    /// Returns the <paramref name="value" /> wrapped between <c>0</c> and the <paramref name="length" />.
    /// If the limit is reached, the next value the function returned is decreased to the <c>0</c> side
    /// or increased to the <paramref name="length" /> side (like a triangle wave).
    /// If <paramref name="length" /> is less than zero, it becomes positive.
    /// </summary>
    /// <param name="value">The value to pingpong.</param>
    /// <param name="length">The maximum value of the function.</param>
    /// <returns>The ping-ponged value.</returns>
    public static double PingPong(double value, double length)
    {
        return length == 0.0 ? 0.0 : System.Math.Abs(Fract((value - length) / (length * 2.0)) * length * 2.0 - length);

        static double Fract(double value) => value - System.Math.Floor(value);
    }

    /// <summary>Returns the amount of digits after the decimal place.</summary>
    /// <param name="s">The input value.</param>
    /// <returns>The amount of digits.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int DecimalCount(double s) => Mathf.DecimalCount((Decimal)s);

    /// <summary>Returns the amount of digits after the decimal place.</summary>
    /// <param name="s">The input <see langword="decimal" /> value.</param>
    /// <returns>The amount of digits.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int DecimalCount(Decimal s)
    {
        return (int)BitConverter.GetBytes(Decimal.GetBits(s)[3])[2];
    }

    /// <summary>
    /// Rounds <paramref name="s" /> upward (towards positive infinity).
    /// 
    /// This is the same as <see cref="M:Godot.Mathf.Ceil(System.Single)" />, but returns an <see langword="int" />.
    /// </summary>
    /// <param name="s">The number to ceil.</param>
    /// <returns>The smallest whole number that is not less than <paramref name="s" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CeilToInt(float s) => (int)MathF.Ceiling(s);

    /// <summary>
    /// Rounds <paramref name="s" /> upward (towards positive infinity).
    /// 
    /// This is the same as <see cref="M:Godot.Mathf.Ceil(System.Double)" />, but returns an <see langword="int" />.
    /// </summary>
    /// <param name="s">The number to ceil.</param>
    /// <returns>The smallest whole number that is not less than <paramref name="s" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CeilToInt(double s) => (int)System.Math.Ceiling(s);

    /// <summary>
    /// Rounds <paramref name="s" /> downward (towards negative infinity).
    /// 
    /// This is the same as <see cref="M:Godot.Mathf.Floor(System.Single)" />, but returns an <see langword="int" />.
    /// </summary>
    /// <param name="s">The number to floor.</param>
    /// <returns>The largest whole number that is not more than <paramref name="s" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FloorToInt(float s) => (int)MathF.Floor(s);

    /// <summary>
    /// Rounds <paramref name="s" /> downward (towards negative infinity).
    /// 
    /// This is the same as <see cref="M:Godot.Mathf.Floor(System.Double)" />, but returns an <see langword="int" />.
    /// </summary>
    /// <param name="s">The number to floor.</param>
    /// <returns>The largest whole number that is not more than <paramref name="s" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FloorToInt(double s) => (int)System.Math.Floor(s);

    /// <summary>
    /// Rounds <paramref name="s" /> to the nearest whole number.
    /// 
    /// This is the same as <see cref="M:Godot.Mathf.Round(System.Single)" />, but returns an <see langword="int" />.
    /// </summary>
    /// <param name="s">The number to round.</param>
    /// <returns>The rounded number.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RoundToInt(float s) => (int)MathF.Round(s);

    /// <summary>
    /// Rounds <paramref name="s" /> to the nearest whole number.
    /// 
    /// This is the same as <see cref="M:Godot.Mathf.Round(System.Double)" />, but returns an <see langword="int" />.
    /// </summary>
    /// <param name="s">The number to round.</param>
    /// <returns>The rounded number.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RoundToInt(double s) => (int)System.Math.Round(s);

    /// <summary>
    /// Returns the sine and cosine of angle <paramref name="s" /> in radians.
    /// </summary>
    /// <param name="s">The angle in radians.</param>
    /// <returns>The sine and cosine of that angle.</returns>
    public static (float Sin, float Cos) SinCos(float s) => MathF.SinCos(s);

    /// <summary>
    /// Returns the sine and cosine of angle <paramref name="s" /> in radians.
    /// </summary>
    /// <param name="s">The angle in radians.</param>
    /// <returns>The sine and cosine of that angle.</returns>
    public static (double Sin, double Cos) SinCos(double s) => System.Math.SinCos(s);

    /// <summary>
    /// Returns <see langword="true" /> if <paramref name="a" /> and <paramref name="b" /> are approximately
    /// equal to each other.
    /// The comparison is done using the provided tolerance value.
    /// If you want the tolerance to be calculated for you, use <see cref="M:Godot.Mathf.IsEqualApprox(System.Single,System.Single)" />.
    /// </summary>
    /// <param name="a">One of the values.</param>
    /// <param name="b">The other value.</param>
    /// <param name="tolerance">The pre-calculated tolerance value.</param>
    /// <returns>A <see langword="bool" /> for whether or not the two values are equal.</returns>
    public static bool IsEqualApprox(float a, float b, float tolerance)
    {
        return (double)a == (double)b || (double)System.Math.Abs(a - b) < (double)tolerance;
    }

    /// <summary>
    /// Returns <see langword="true" /> if <paramref name="a" /> and <paramref name="b" /> are approximately
    /// equal to each other.
    /// The comparison is done using the provided tolerance value.
    /// If you want the tolerance to be calculated for you, use <see cref="M:Godot.Mathf.IsEqualApprox(System.Double,System.Double)" />.
    /// </summary>
    /// <param name="a">One of the values.</param>
    /// <param name="b">The other value.</param>
    /// <param name="tolerance">The pre-calculated tolerance value.</param>
    /// <returns>A <see langword="bool" /> for whether or not the two values are equal.</returns>
    public static bool IsEqualApprox(double a, double b, double tolerance)
    {
        return a == b || System.Math.Abs(a - b) < tolerance;
    }
}