using TNode.Attribute;
using TNode.Models;
using TNodeGraphViewImpl.Editor.NodeViews;

namespace TNode.Editor.NodeViews{
    [ViewComponent]
    public class DragBaseNodeView:BaseNodeView<BlackboardDragNodeData>{
        public DragBaseNodeView() : base(){
            this.titleContainer.visible = false;
           this.titleContainer.RemoveFromHierarchy();
        }
    }
}