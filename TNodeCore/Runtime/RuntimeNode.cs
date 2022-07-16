using System.Collections.Generic;
using TNodeCore.Models;

namespace TNodeCore.Runtime{
    public abstract class RuntimeNode{
        public NodeData NodeData;
        public List<NodeLink> NodeLinks;
        public void ProcessThisNode(){
            NodeData.OnProcess();
        }
    }
}