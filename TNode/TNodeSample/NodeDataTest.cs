using Dialogue;
using TNode.Attribute;

namespace TNode.TNodeSample{
    public class NodeDataTest:NodeData{
        [InputPort] private float _floatInput;
        public NodeDataTest(string name):base(){
     
        }
        
    }
}