using TNodeCore.Runtime;
using TNodeCore.Runtime.Attributes;
using TNodeCore.Runtime.Attributes.Ports;
using TNodeCore.Runtime.Models;
using UnityEngine;

namespace Samples.Nodes{
    [GraphUsage(typeof(HelloGraph),"Math")]
    public class CheckSizeNode:ConditionalNode{
        [Input]
        public float A{ get; set; }

        [Output]
        public TransitionCondition<float> Bigger(){
            return new TransitionCondition<float>(){
                Condition = A>0,
                Priority = 0,
                DataFunc = ()=>A
            };
        }
        [Output]
        public TransitionCondition<float> SmallerOrEqual(){
            return new TransitionCondition<float>(){
                Condition = A<=0,
                Priority = 0,
                DataFunc = ()=>A
            };
        }

        public override void Process(){
            this.Log($"{A}");
        }
    }
}