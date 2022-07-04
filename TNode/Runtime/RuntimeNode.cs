using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TNode.Models;

namespace TNode.Runtime{
    public class RuntimeNode<T> where T:NodeData{
        public T NodeData{ get; set; }

        //Links related to runtime node,for fast access.only remember out links
        public List<NodeLink> NodeLinks;


        public void Process(){
      
            var outputPorts = NodeLinks.Select(x => x.outPort.portName);
        }
 
    }
}