using TNodeCore.Runtime.Models;

namespace TNodeCore.Editor.Inspector{
    public interface INodeDataBindingBase{
        public string BindingPath{ get; set; }
        public NodeData BindingNodeData{ get; set; }
    }
}