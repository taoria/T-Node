using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Codice.Client.Common.TreeGrouper;
using TNodeCore.Runtime.Models;
using TNodeCore.Runtime.RuntimeCache;
using UnityEngine;

namespace TNodeCore.Runtime{
    public class RuntimeNode{
        public NodeData NodeData { get; set; }
        //the link connect to node's in port
        public List<NodeLink> InputLink = new List<NodeLink>();
        //the link connect to node's out port
        public List<NodeLink> OutputLink = new List<NodeLink>();
        //Cache node data type for fast access
        private readonly Type _type;
        public Type NodeType => _type;

        public void SetInput(string portName,object value){
            var valueType = value.GetType();
            var portType = _portAccessors[portName].Type;
            if(portType!=valueType && !portType.IsAssignableFrom(valueType)){
                var res =RuntimeCache.RuntimeCache.Instance.GetConvertedValue(valueType, portType, value);
                _portAccessors[portName].SetValue(this.NodeData, res);
            }
            else{
                    
                _portAccessors[portName].SetValue(this.NodeData,value);
            }
        }
        public object GetOutput(string portName){
            
            return _portAccessors[portName].GetValue(this.NodeData);
        }


        private readonly Dictionary<string, IModelPropertyAccessor> _portAccessors;
        public Action Process;
   
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