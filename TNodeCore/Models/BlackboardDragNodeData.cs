using System;
using TNodeCore.Attribute;
using TNodeCore.Attribute.Ports;
using TNodeCore.RuntimeCache;
using UnityEngine;

namespace TNodeCore.Models{
    [Serializable]
    [InternalModel]
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