using Tmp.Core.Redot;

namespace Tmp.Math.Components;

public class CNode2DTransformRoot() : Component(self =>
{
    self.CreateContext(new СNode2DTransform
    {
        Local = Transform2D.Identity
    });
});

public sealed class СNode2DTransform : INode2DTransform
{
    private СNode2DTransform? _parent;
    private Transform2D _transform = Transform2D.Identity;
    private Transform2D _globalTransform = Transform2D.Identity;
    private bool _isXFormDirty = true;
    private bool _isGlobalInvalid = true;
    private Vector2 _position;
    private Radians _rotation;
    private float _skew;
    private Vector2 _scale = new(1, 1);

    public Transform2D Local
    {
        get => GetTransform();
        set => SetTransform(value);
    }

    public Vector2 Position
    {
        get => GetPosition();
        set => SetPosition(value);
    }
    
    public Radians Rotation
    {
        get => GetRotation();
        set => SetRotation(value);
    }

    public Vector2 Scale
    {
        get => GetScale();
        set => SetScale(value);
    }

    public float Skew
    {
        get => GetSkew();
        set => SetSkew(value);
    }

    public Vector2 GlobalPosition
    {
        get => GetGlobalPosition();
        set => SetGlobalPosition(value);
    }

    public Transform2D Global
    {
        get => GetGlobalTransform();
        set => SetGlobalTransform(value);
    }
    
    public Vector2 GlobalScale
    {
        get => GetGlobalScale();
        set => SetGlobalScale(value);
    }
    
    public float GlobalSkew
    {
        get => GetGlobalSkew();
        set => SetGlobalSkew(value);
    }
    
    public Radians GlobalRotation
    {
        get => GetGlobalRotation();
        set => SetGlobalRotation(value);
    }

    private void SetGlobalPosition(Vector2 value)
    {
        if (_parent != null)
        {
            var inv = _parent.Global.AffineInverse();
            SetPosition(inv.Xform(value));
        }
        else
        {
            SetPosition(value);
        }
    }

    private Vector2 GetGlobalPosition()
    {
        return Global.Origin;
    }
    
    private void SetGlobalRotation(Radians value)
    {
        if (_parent != null)
        {
            var parentGlobalTransform = _parent.Global;
            var newTransform = parentGlobalTransform * _transform;
            newTransform = new Transform2D(value, newTransform.Scale, newTransform.Skew, newTransform.Origin);
            newTransform = parentGlobalTransform.AffineInverse() * newTransform;
            SetRotation(newTransform.Skew);
        }
        else
        {
            SetRotation(value);
        }
    }

    private Radians GetGlobalRotation()
    {
        return Global.Rotation;
    }

    private void SetGlobalSkew(float value)
    {
        if (_parent != null)
        {
            var parentGlobalTransform = _parent.Global;
            var newTransform = parentGlobalTransform * _transform;
            newTransform = new Transform2D(newTransform.Rotation, newTransform.Scale, value, newTransform.Origin);
            newTransform = parentGlobalTransform.AffineInverse() * newTransform;
            SetSkew(newTransform.Skew);
        }
        else
        {
            SetSkew(value);
        }
    }

    private float GetGlobalSkew()
    {
        return Global.Skew;
    }

    private void SetGlobalScale(Vector2 scale)
    {
        if (_parent != null)
        {
            var parentGlobalTransform = _parent.Global;
            var newTransform = parentGlobalTransform * _transform;
            newTransform = new Transform2D(newTransform.Rotation, scale, newTransform.Skew, newTransform.Origin);
            newTransform = parentGlobalTransform.AffineInverse() * newTransform;
            SetScale(newTransform.Scale);
        }
        else
        {
            SetScale(scale);
        }
    }

    private Vector2 GetGlobalScale()
    {
        return Global.Scale;
    }
    
    private void SetGlobalTransform(Transform2D transform)
    {
        if (_parent != null)
        {
            SetTransform(_parent.GetGlobalTransform().AffineInverse() * Local);
        }
        else
        {
            SetTransform(transform);
        }
    }
    
    private Transform2D GetGlobalTransform()
    {
        if (_isGlobalInvalid)
        {
            Transform2D newGlobal;
            if (_parent != null)
            {
                newGlobal = _parent.Global * Local;
            }
            else
            {
                newGlobal = Local;
            }
            _globalTransform = newGlobal;
            // _isGlobalInvalid = false;
        }
        return _globalTransform;
    }

    private Transform2D GetTransform()
    {
        return _transform;
    }

    private void SetTransform(Transform2D transform)
    {
        _isXFormDirty = true;
        _transform = transform;
    }
    
    private void SetPosition(Vector2 position)
    {
        UpdateXFormValues();
        _position = position;
        UpdateTransform();
    }
    
    private void SetRotation(Radians rotation)
    {
        UpdateXFormValues();
        _rotation = rotation;
        UpdateTransform();
    }

    public void SetSkew(float skew)
    {
        UpdateXFormValues();
        _skew = skew;
        UpdateTransform();
    }

    public void SetScale(Vector2 scale)
    {
        UpdateXFormValues();
        _scale = scale;
        if (Mathf.IsZeroApprox(scale.X))
        {
            scale.X = Mathf.Epsilon;
        }
        if (Mathf.IsZeroApprox(scale.Y))
        {
            scale.Y = Mathf.Epsilon;
        }
        UpdateTransform();
    }

    public Vector2 GetPosition()
    {
        UpdateXFormValues();
        return _position;
    }

    public Radians GetRotation()
    {
        UpdateXFormValues();
        return _rotation;
    }

    public float GetSkew()
    {
        UpdateXFormValues();
        return _skew;
    }

    public Vector2 GetScale()
    {
        UpdateXFormValues();
        return _scale;
    }
    
    private void UpdateXFormValues()
    {
        if (!_isXFormDirty) return;
        _rotation = _transform.Rotation;
        _skew = _transform.Skew;
        _position = _transform.Origin;
        _scale = _transform.Scale;
        _isXFormDirty = false;
    }

    private void UpdateTransform()
    {
        _transform = new Transform2D(_rotation, _scale, _skew, _position);
    }

    void INode2DTransform.AddTo(СNode2DTransform transform)
    {
        _parent = transform;
        _isGlobalInvalid = true;
    }

    void INode2DTransform.ClearParent()
    {
        _parent = null;
        _isGlobalInvalid = true;
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
            new СNode2DTransform { Local = initial ?? Transform2D.Identity }
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