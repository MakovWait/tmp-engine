using System.Numerics;

namespace Tmp.Math;

public interface IDegrees<T> where T : INumber<T>;

public readonly record struct Degrees(float Value)
{
    public static implicit operator Degrees(Radians value) => new(Mathf.RadToDeg(value));
    public static implicit operator Degrees(float value) => new(value);
    public static implicit operator Degrees(int value) => new(value);
    public static implicit operator float(Degrees self) => self.Value;
    
    public static Degrees operator +(Degrees a) => a;
    public static Degrees operator -(Degrees a) => new(-a.Value);
    public static Degrees operator +(Degrees a, Degrees b) => new(a.Value + b.Value);
    public static Degrees operator -(Degrees a, Degrees b) => new(a.Value - b.Value);
    public static Degrees operator +(Degrees a, Radians b) => new(a.Value + b.ToDeg());
    public static Degrees operator -(Degrees a, Radians b) => new(a.Value - b.ToDeg());
    // public static Degrees operator *(Degrees a, Degrees b) => new(a.Value * b.Value);
    public static Degrees operator *(Degrees a, float b) => new(a.Value * b);
    public static Degrees operator *(Degrees a, int b) => new(a.Value * b);
    // public static Degrees operator /(Degrees a, Degrees b) => new(a.Value / b.Value);
    public static Degrees operator /(Degrees a, float b) => new(a.Value / b);
    public static Degrees operator /(Degrees a, int b) => new(a.Value / b);
    
    public Radians ToRad() => this;
    
    public override string ToString()
    {
        return $"{Value} degrees";
    }
}