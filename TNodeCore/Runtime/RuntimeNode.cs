using System;
using System.Collections.Generic;
using Codice.Client.Common.TreeGrouper;
using TNodeCore.Models;
using TNodeCore.RuntimeCache;

namespace TNodeCore.Runtime{
    public class RuntimeNode{
        public NodeData NodeData { get; set; }
        //the link connect to node's in port
        public List<NodeLink> InputLink;
        //the link connect to node's out port
        public List<NodeLink> OutputLink;
        public Type type;


        public void SetInput(string portName,object value){
            NodeData.SetValue(portName, value);
        }
        public object GetOutput(string portName){
            return NodeData.GetValue(portName);
        }
        
        public RuntimeNode(NodeData nodeData){
            NodeData = nodeData;
            //Caching the type of the node
            type = nodeData.GetType();
        }
        public List<string> GetInputNodesId(){
            List<string> dependencies = new List<string>();
            foreach (NodeLink link in InputLink)
            {
                dependencies.Add(link.outPort.nodeDataId);
            }
            return dependencies;
        }
        
    }
}