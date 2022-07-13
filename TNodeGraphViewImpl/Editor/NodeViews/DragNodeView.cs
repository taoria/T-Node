﻿using TNode.Attribute;
using TNode.Models;

namespace TNode.Editor.NodeViews{
    [ViewComponent]
    public class DragBaseNodeView<T>:BaseNodeView<BlackboardDragNodeData<T>>{
        public DragBaseNodeView() : base(){
           //Make capsule like style
           
           this.titleContainer.visible = false;
           this.titleContainer.RemoveFromHierarchy();
        }
    }
}