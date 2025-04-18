using R3;

namespace Tmp.Core.Comp.Flow;

public class Conditional : Component
{
    public required Observable<bool> When { get; init; }

    private bool _queuedToUpdate;
    
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
        if (_queuedToUpdate) return;
        _queuedToUpdate = true;
        Self.CallDeferred(state => state.confitional.Update(state.when), (confitional: this, when));
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