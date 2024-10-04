namespace Tmp.Math;

public static class Extensions
{
    public static Vector2 ToCore(this System.Numerics.Vector2 self)
    {
        return new Vector2(self.X, self.Y);
    }

    public static Vector2I ToCoreI(this System.Numerics.Vector2 self)
    {
        return new Vector2I((int)self.X, (int)self.Y);
    }

    public static Degrees RadToDeg(this float self)
    {
        return new Degrees(Mathf.RadToDeg(self));
    }
    
    public static Radians DegToRad(this float self)
    {
        return new Radians(Mathf.DegToRad(self));
    }
    
    public static Radians DegToRad(this int self)
    {
        return new Radians(Mathf.DegToRad(self));
    }
    
    public static int ToInt(this float self)
    {
        return (int)self;
    }
}