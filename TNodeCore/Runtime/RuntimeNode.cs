using System;
using System.Collections.Generic;
using System.Reflection;
using Codice.Client.Common.TreeGrouper;
using TNodeCore.Attribute.Ports;
using TNodeCore.Models;
using TNodeCore.RuntimeCache;

namespace TNodeCore.Runtime{
    public class RuntimeNode{
        public NodeData NodeData { get; set; }
        //the link connect to node's in port
        public List<NodeLink> InputLink = new List<NodeLink>();
        //the link connect to node's out port
        public List<NodeLink> OutputLink = new List<NodeLink>();
        //Cache node data type for fast access
        private readonly Type _type;


        public void SetInput(string portName,object value){
            _portAccessors[portName].SetValue(this.NodeData,value);
           
        }
        public object GetOutput(string portName){
            
            return _portAccessors[portName].GetValue(this.NodeData);
        }


        private readonly Dictionary<string, IModelPropertyAccessor> _portAccessors;

   
        public RuntimeNode(NodeData nodeData){
            NodeData = nodeData;
            //Caching the type of the node
            _type = nodeData.GetType();
            var info = nodeData.GetType().GetProperties();

            _portAccessors = RuntimeCache.RuntimeCache.Instance.CachedPropertyAccessors[_type];
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