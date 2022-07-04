using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TNode.Models;

namespace TNode.Runtime{
    public abstract class RuntimeNode{
        public NodeData NodeData;
        public List<NodeLink> NodeLinks;
        public void ProcessThisNode(){
            NodeData.OnProcess();
        }
    }
}