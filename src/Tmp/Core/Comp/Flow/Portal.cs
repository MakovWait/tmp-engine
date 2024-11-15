using System.Diagnostics;

namespace Tmp.Core.Comp.Flow;

public class Portal(INodeLocator mount) : Component
{
    public INodeLocator Mount { get; set; } = mount;
    
    protected override Components Init(INodeInit self)
    {
        self.OnMount(() =>
        {
            var remoteParent = Mount.Get(self);
            Debug.Assert(remoteParent != null);
            
            foreach (var component in Children)
            {
                component.Build(Tree, remoteParent);
            }
        });
        
        return [];
    }
}