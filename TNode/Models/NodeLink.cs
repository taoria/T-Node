using System;
using UnityEditor.Experimental.GraphView;


namespace TNode.Models{
    //NodeAttribute links are stored in output side of the two node port.
    [Serializable]
    public class NodeLink{
       // public DialogueNodePortData From{ get; }
       public PortInfo inPort;
       public PortInfo outPort;
         public NodeLink(PortInfo inPort, PortInfo outPort){
             this.inPort = inPort;
              this.outPort = outPort;
         }
    }
}