using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TNodeCore.Runtime.Attributes.Ports;
using TNodeCore.Runtime.Models;
using TNodeCore.Runtime.RuntimeCache;
using UnityEngine;

namespace TNodeCore.Runtime{
    public class RuntimeNode{
        public NodeData NodeData { get; set; }
        //the link connect to node's in port
        public List<NodeLink> InputLinks = new List<NodeLink>();
        //the link connect to node's out port
        public List<NodeLink> OutputLinks = new List<NodeLink>();
        //Cache node data type for fast access
        private readonly Type _type;
        public Type NodeType => _type;
        

        public Type GetElementTypeOfPort(string portName,bool multi = false){
            var type = _portAccessors[portName].Type;
            if (multi == false)
                return type;
            if (type.IsArray){
                return type.GetElementType();
            }

            if (type.IsGenericType){
                return type.GetGenericArguments()[0];
            } 
            return type;
        }
            
        
        public void SetInput(string portName,object value){
            var valueType = value.GetType();
            var portPath = portName.Split(':');
            if (portPath.Length ==2){
                portName = portPath[0];
                int index = int.Parse(portPath[1]);
                var realPortType = GetElementTypeOfPort(portName, true);

                if (realPortType != valueType){
                    value = RuntimeCache.RuntimeCache.Instance.GetConvertedValue(valueType,realPortType,value);
                }
                if(_portAccessors[portName].Type.IsArray){
                    if (_portAccessors[portName].GetValue(NodeData) is Array array){
                        array.SetValue(value, index);
                    }
                }
                if (_portAccessors[portName].Type.IsGenericType){
                    if (_portAccessors[portName].GetValue(NodeData) is IList list)
                        list[index] = value;
                }
      
            }
            else{
                var portType = _portAccessors[portName].Type;
                if(portType!=valueType && !portType.IsAssignableFrom(valueType)){
                    var res =RuntimeCache.RuntimeCache.Instance.GetConvertedValue(valueType, portType, value);
                    _portAccessors[portName].SetValue(NodeData, res);
                }
                else{
                    _portAccessors[portName].SetValue(NodeData,value);
                }
            }

        }
        public object GetOutput(string portName){
            var portPath = portName.Split(':');
            if (portPath.Length == 2){
                portName = portPath[0];
                int index = int.Parse(portPath[1]);
                if(_portAccessors[portName].Type.IsArray){
                    if (_portAccessors[portName].GetValue(NodeData) is Array array)
                        return array.GetValue(index);
                }

                if (_portAccessors[portName].Type.IsGenericType){
                    if (_portAccessors[portName].GetValue(NodeData) is IList list)
                        return list[index];
                }
            }
            return _portAccessors[portName].GetValue(NodeData);
        }
        public string[] GetPortsOfType<T> (){
            var ports = new List<string>();
            foreach (var port in _portAccessors.Keys){
                if(_portAccessors[port].Type==typeof(T)||typeof(T).IsAssignableFrom(_portAccessors[port].Type)){
                    ports.Add(port);
                }
            }
            return ports.ToArray();
        }
        /// <summary>
        /// Call it carefully to cache
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public  Direction GetPortDirection(string portName){
            var attribute = NodeData.GetType().GetMember(portName)[0].GetCustomAttribute<PortAttribute>();
            if (attribute is InputAttribute){
                return Direction.Input;
            }

            return Direction.Output;
        }

        private readonly Dictionary<string, IModelPortAccessor> _portAccessors;
        
        public Action Process;
   
        public RuntimeNode(NodeData nodeData){
            NodeData = nodeData;
            //Caching the type of the node
            _type = nodeData.GetType();
            var info = nodeData.GetType().GetProperties();
            _portAccessors = RuntimeCache.RuntimeCache.Instance.CachedPortAccessors[_type];
            
        }

        public void ResetPortValue(){
            foreach (var modelPortAccessor in _portAccessors){
                modelPortAccessor.Value.Reset(this);
            }
        }
        public List<string> GetInputNodesId(){
            List<string> dependencies = new List<string>();
            foreach (NodeLink link in InputLinks)
            {
                dependencies.Add(link.outPort.nodeDataId);
            }
            return dependencies;
        }
    }
    public enum Direction{
        Input,
        Output
    }
}