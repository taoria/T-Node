using System;
using TNodeCore.Runtime.Attributes;
using TNodeCore.Runtime.Attributes.Ports;
using TNodeCore.Runtime.RuntimeCache;

namespace TNodeCore.Runtime.Models{
    [Serializable]
    [InternalUsage]
    public class BlackboardDragNodeData:SceneNodeData{
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
    public class SceneNodeData:NodeData{
       
   
        public BlackboardData BlackboardData{ get; set; }
        
    }
}