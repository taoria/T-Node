using System;
using TNodeCore.Attribute;
using TNodeCore.Attribute.Ports;
using TNodeCore.RuntimeCache;
using UnityEngine;

namespace TNodeCore.Models{
    [Serializable]
    [InternalUsage]
    public class BlackboardDragNodeData:RuntimeNodeData{
        public string blackDragData;
      
        /// <summary>
        /// it's very hacky way to get blackboard data ,even when the value is null,type info is not null!
        /// </summary>
        /// TODO : The type handling in a safer way in the future
        [Output("",PortNameHandling.MemberType,TypeHandling.Implemented)]
        public object Value => BlackboardData.GetValue(blackDragData);
 
        public BlackboardDragNodeData(){
            
        }
        
    }
    [Serializable]
    public class RuntimeNodeData:NodeData{
   
        public BlackboardData BlackboardData{ get; set; }
        
    }
}