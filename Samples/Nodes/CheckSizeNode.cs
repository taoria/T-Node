using TNode.TNodeCore.Runtime.Models;
using TNodeCore.Runtime.Attributes;
using TNodeCore.Runtime.Attributes.Ports;
using TNodeCore.Runtime.Models;

namespace Samples.Nodes{
    [GraphUsage(typeof(HelloGraph),"Math")]
    public class CheckSizeNode:ConditionalNode{
        [Input]
        public float A{ get; set; }

        [Output]
        public TransitionCondition Bigger(){
            return new TransitionCondition(){
                Condition = A>0,
                Priority = 0
            };
        }
        [Output]
        public TransitionCondition SmallerOrEqual(){
            return new TransitionCondition(){
                Condition = A<=0,
                Priority = 0
            };
        }
   
    }
}