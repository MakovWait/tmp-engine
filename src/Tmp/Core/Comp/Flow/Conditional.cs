using R3;

namespace Tmp.Core.Comp.Flow;

public class Conditional : Component
{
    public required Observable<bool> When { get; init; }

    private bool _queuedToUpdate;
    private bool _lastWhen;
    
    protected override Components Init(INodeInit self)
    {
        self.AutoDispose(When.Subscribe(this, (when, component) =>
        {
            component.QueueUpdate(when);
        }));

        return [];
    }
    
    private void QueueUpdate(bool when)
    {
        _lastWhen = when;
        if (_queuedToUpdate) return;
        _queuedToUpdate = true;
        Self.CallDeferred(confitional => confitional.Update(confitional._lastWhen), this);
    }
    
    private void Update(bool when)
    {
        if (when)
        {
            CreateChildrenAndMount(Children);   
        }
        else
        {
            ClearChildren();
        } 
        
        _queuedToUpdate = false;
    }
}