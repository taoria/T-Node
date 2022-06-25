using Dialogue;
using TNode.Attribute;
using TNode.Models;
using UnityEditor.Experimental.GraphView;

namespace TNode.TNodeSample{
    public class NodeDataTest:NodeData{
        [InputPort(typeof(NodeLink),Port.Capacity.Multi)]
        private float _floatInput;
        public NodeDataTest(string name):base(){
     
        }
        
    }
}