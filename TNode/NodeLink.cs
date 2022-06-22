using System;

namespace Dialogue{
    //Node links are stored in output side of the two node port.
    [Serializable]
    public class NodeLink{
       // public DialogueNodePortData From{ get; }
        public bool ConditionEdge = false;
        public DialogueNodePortData To{ get; }
        public NodeLink(DialogueNodePortData  to){
           // From = from;
            To = to;
        }
        public delegate bool Condition(DialogueNodePortData to);
        public Condition ConditionFunction;

        public bool Accessible{
            get{
                if (To == null) return false;
                if(ConditionFunction == null)
                    return true;
                return ConditionFunction(To);
            }
        }
        public void SetCondition(Condition condition){
            ConditionFunction = condition;
        }
    }
}