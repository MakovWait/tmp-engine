using Tmp.Core.Redot;

namespace Tmp.Math.Components;

public class CNode2DTransformRoot() : Component(self =>
{
    self.CreateContext(new СNode2DTransform
    {
        Transform = Transform2D.Identity
    });
});

public sealed class СNode2DTransform : INode2DTransform
{
    private СNode2DTransform? parent;
    private Transform2D _transform;

    public Transform2D Transform
    {
        get => _transform;
        init => _transform = value;
    }

    public Vector2 Position
    {
        get => Transform.Origin;
        set => _transform = _transform.TranslatedLocal(value - Position);
    }
    
    public float Rotation
    {
        get => _transform.Rotation;
        set => _transform = _transform.RotatedLocal(value - Rotation);
    }

    public Vector2 GlobalPosition => GlobalTransform.Origin;

    public Transform2D GlobalTransform => GetGlobalTransform();

    private Transform2D GetGlobalTransform()
    {
        if (parent != null)
        {
            return parent.GetGlobalTransform() * Transform;
        }
        else
        {
            return Transform;
        }
    }

    void INode2DTransform.AddTo(СNode2DTransform transform)
    {
        parent = transform;
    }

    void INode2DTransform.ClearParent()
    {
        parent = null;
    }
}

internal interface INode2DTransform
{
    internal void AddTo(СNode2DTransform transform);

    internal void ClearParent();
}

public static class NodeTransformExtensions
{
    public static IContextValue<СNode2DTransform> UseParentTransform2D(this Component.Self self)
    {
        return self.Use<СNode2DTransform>();
    }

    public static IDeferredValue<СNode2DTransform> UseTransform2D(this Component.Self self, Transform2D? initial = null)
    {
        var parentTransform = self.UseParentTransform2D();
        var transform = self.CreateContext(
            new СNode2DTransform { Transform = initial ?? Transform2D.Identity }
        );

        self.UseEffect(() =>
        {
            ((INode2DTransform)transform.Get()).AddTo(parentTransform.Get());
            return () => ((INode2DTransform)transform.Get()).ClearParent();
        }, [parentTransform]);

        return new DeferredValueAnonymous<СNode2DTransform>(transform.Get);
    }

    public static Vector2 Position(this IDeferredValue<СNode2DTransform> self)
    {
        return self.Get().Position;
    }

    public static Vector2 GlobalPosition(this IDeferredValue<СNode2DTransform> self)
    {
        return self.Get().GlobalPosition;
    }
}