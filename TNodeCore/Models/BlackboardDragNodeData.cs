using System;
using TNodeCore.Attribute.Ports;
using TNodeCore.RuntimeCache;
using UnityEngine;

namespace TNodeCore.Models{
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