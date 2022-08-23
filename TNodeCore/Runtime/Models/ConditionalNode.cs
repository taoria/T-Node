using TNodeCore.Runtime;
using TNodeCore.Runtime.Attributes.Ports;
using TNodeCore.Runtime.Models;
using Unity.Plastic.Newtonsoft.Json.Serialization;

namespace TNode.TNodeCore.Runtime.Models{
    public class ConditionalNode:NodeData{

    }
    public class TransitionCondition:IBaseTransition{
        
        
        public Func<object> DataFunc;
        
        public bool Condition{ get; set; }
        public int Priority{ get; set; }
        public object GetValue(){
           return DataFunc();
        }
    }
    public class TransitionCondition<T>:IBaseTransition{
        public Func<T> DataFunc;
        public bool Condition{ get; set; }
        public int Priority{ get; set; }
        public object GetValue(){
            return DataFunc.Invoke();
        }
    }
    public interface IBaseTransition{
        public bool Condition{ get; set; }
        public int Priority{ get; set; }

        public object GetValue();
    }

}