using System;
using System.Collections.Generic;
using Codice.Client.Common.TreeGrouper;
using TNodeCore.Attribute.Ports;
using TNodeCore.Models;
using TNodeCore.RuntimeCache;

namespace TNodeCore.Runtime{
    public class RuntimeNode{
        public NodeData NodeData { get; set; }
        //the link connect to node's in port
        public List<NodeLink> InputLink;
        //the link connect to node's out port
        public List<NodeLink> OutputLink;
        //Cache node data type for fast access
        private readonly Type _type;


        public void SetInput(string portName,object value){
            NodeData.SetValue(portName, value);
        }
        public object GetOutput(string portName){
            return NodeData.GetValue(portName);
        }



        private Dictionary<string,RuntimeCache.RuntimeCache.SetPropertyValueDelegate> _inputPorts = new();
        private Dictionary<string,RuntimeCache.RuntimeCache.GetPropertyValueDelegate> _outputPorts = new();


        private void CachingPorts(){
            var properties = _type.GetProperties();
            foreach (var propertyInfo in properties){
                //Check if the property is a port
                var attribute = propertyInfo.GetCustomAttributes(typeof(InputAttribute),true);
                if (attribute.Length > 0){
                  
                    _inputPorts.Add(propertyInfo.Name,NodeData.CacheSetProperty(propertyInfo.Name));
                }
                
                attribute = propertyInfo.GetCustomAttributes(typeof(OutputAttribute),true);
                if (attribute.Length > 0){
                    _outputPorts.Add(propertyInfo.Name,NodeData.CacheGetProperty(propertyInfo.Name));
                }
            }
        }
        public RuntimeNode(NodeData nodeData){
            NodeData = nodeData;
            //Caching the type of the node
            _type = nodeData.GetType();
            var info = nodeData.GetType().GetProperties();
            
            CachingPorts();
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