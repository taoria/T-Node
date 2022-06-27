using TNode.Models;

namespace TNode.Editor.Inspector{
    public interface INodeDataBindingBase{
        public string BindingPath{ get; set; }
        public NodeData BindingNodeData{ get; set; }
    }
}