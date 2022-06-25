using TNode.Models;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNode.Editor.Inspector{
    public abstract class InspectorItem<T>:VisualElement,INodeDataBinding<T>{
        public string BindingPath{ get; set; }
        public NodeData BindingNodeData{ get; set; }
    }
}