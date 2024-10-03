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

    public static float ToDeg(this float self)
    {
        return Mathf.RadToDeg(self);
    }
    
    public static float ToRad(this float self)
    {
        return Mathf.DegToRad(self);
    }
    
    public static float ToRad(this int self)
    {
        return Mathf.DegToRad(self);
    }
    
    public static int ToInt(this float self)
    {
        return (int)self;
    }
}