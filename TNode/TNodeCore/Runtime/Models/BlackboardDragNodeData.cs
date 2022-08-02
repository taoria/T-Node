using System;
using TNodeCore.Runtime.Attributes;
using TNodeCore.Runtime.Attributes.Ports;
using TNodeCore.Runtime.RuntimeCache;
using UnityEngine;

namespace TNodeCore.Runtime.Models{
    [Serializable]
    [InternalUsage]
    public class BlackboardDragNodeData:SceneNodeData{
        public string BlackDragData{
            get => blackDragData;
            set{
                blackDragData = value;
                if (blackDragData.Contains('.')){
                    isListElement = true;
                }
            }
        }

        public string blackDragData;
        /// <summary>
        /// it's very hacky way to get blackboard data ,even when the value is null,type info is not null!
        /// </summary>
        /// TODO : The type handling in a safer way in the future
        [Output("",PortNameHandling.MemberType,TypeHandling.Implemented)]
        public object Value{
            get{
                if (!isListElement){
                     return BlackboardData.GetValue(BlackDragData);
                }
                else{
                    var split = BlackDragData.Split('.');
                    Debug.Log(blackDragData);
                    var index = int.Parse(split[1]);
              
                    return BlackboardData.GetListValue(split[0],index);
                }
               
            }
        }

        public bool isListElement=false;
        public BlackboardDragNodeData(){
            
        }
        
    }
    [Serializable]
    public class SceneNodeData:NodeData{
       
   
        public BlackboardData BlackboardData{ get; set; }
        
    }
}