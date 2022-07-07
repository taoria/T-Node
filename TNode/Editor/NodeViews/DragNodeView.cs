using TNode.Attribute;
using TNode.Editor.BaseViews;
using TNode.Models;

namespace TNode.Editor.NodeViews{
    [NodeComponent]
    public class DragNodeView<T>:NodeView<BlackboardDragNodeData<T>>{
        public DragNodeView() : base(){
           //Make capsule like style
           
           this.titleContainer.visible = false;
           this.titleContainer.RemoveFromHierarchy();
        }
    }
}