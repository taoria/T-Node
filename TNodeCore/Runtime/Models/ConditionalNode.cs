using TNodeCore.Runtime;
using TNodeCore.Runtime.Attributes.Ports;
using TNodeCore.Runtime.Models;

namespace TNode.TNodeCore.Runtime.Models{
    public class ConditionalNode:NodeData{
        [Input]
        public object In{ get; set; }
    }
    public struct TransitionCondition{
        public bool Condition;
        public int Priority;
    }
}