using TNodeCore.Runtime.Attributes.Ports;
using TNodeCore.Runtime.Models;

namespace TNode.TNodeCore.Runtime.Models{
    public class ConditionalNode:NodeData{
        [Input]
        public bool In{ get; set; }
    }
}