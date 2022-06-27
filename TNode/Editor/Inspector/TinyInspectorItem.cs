using TNode.Models;
using UnityEngine.UIElements;

namespace TNode.Editor.Inspector{
    /// <summary>
    /// Tiny Inspector item is a simple inspector item inside a node view ,it monitor node data
    /// 
    /// </summary>
    public class TinyInspectorItem<T>:VisualElement,INodeDataBinding<T>{
        public string BindingPath{ get; set; }
        public NodeData BindingNodeData{ get; set; }
    }
}