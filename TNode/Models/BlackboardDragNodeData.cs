using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using TNode.Attribute;
using TNode.Attribute.Ports;
using TNode.RuntimeCache;
using UnityEngine;
using UnityEngine.Serialization;

namespace TNode.Models{
    [Serializable]
    public class BlackboardDragNodeData:NodeData{
        public string blackDragData;
        [SerializeReference]
        public BlackboardData blackboardData;
        
        [Output("",PortNameHandling.MemberType,TypeHandling.Implemented)] 
        public object Value => blackboardData.GetValue(blackDragData);
 
        public BlackboardDragNodeData(){
            
        }
        
    }
}