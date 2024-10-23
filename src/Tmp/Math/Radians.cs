namespace Tmp.Math;

public readonly record struct Radians(float Value)
{
    public static implicit operator Radians(Degrees value) => new(Mathf.DegToRad(value));
    public static implicit operator Radians(float value) => new(value);
    public static implicit operator Radians(int value) => new(value);
    public static implicit operator float(Radians self) => self.Value;

    public static Radians operator +(Radians a) => a;
    public static Radians operator -(Radians a) => new(-a.Value);
    public static Radians operator +(Radians a, Radians b) => new(a.Value + b.Value);
    public static Radians operator -(Radians a, Radians b) => new(a.Value - b.Value);
    public static Degrees operator +(Radians a, Degrees b) => new(a.Value + b.ToRad());
    public static Degrees operator -(Radians a, Degrees b) => new(a.Value - b.ToRad());
    public static Degrees operator +(Radians a, float b) => new(a.Value + b);
    public static Degrees operator -(Radians a, float b) => new(a.Value - b);
    public static Degrees operator +(Radians a, int b) => new(a.Value + b);
    public static Degrees operator -(Radians a, int b) => new(a.Value - b);
    // public static Radians operator *(Radians a, Radians b) => new(a.Value * b.Value);
    public static Radians operator *(Radians a, float b) => new(a.Value * b);
    public static Radians operator *(Radians a, int b) => new(a.Value * b);
    // public static Radians operator /(Radians a, Radians b) => new(a.Value / b.Value);
    public static Radians operator /(Radians a, float b) => new(a.Value / b);
    public static Radians operator /(Radians a, int b) => new(a.Value / b);
    
    public Degrees ToDeg() => this;

    public override string ToString()
    {
        return $"{Value} rad";
    }
}