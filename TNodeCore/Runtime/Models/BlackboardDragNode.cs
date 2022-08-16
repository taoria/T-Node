using System;
using TNodeCore.Runtime.Attributes;
using TNodeCore.Runtime.Attributes.Ports;
using TNodeCore.Runtime.RuntimeCache;
using UnityEngine;
using UnityEngine.Serialization;

namespace TNodeCore.Runtime.Models{
    [Serializable]
    [InternalUsage]
    public class BlackboardDragNode:SceneNode,ISerializationCallbackReceiver{
        public string BlackDragData{
            get => blackDragData;
            set{
                blackDragData = value;
                if (blackDragData.Contains(".")){
                    isListElement = true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        
        public Type BlackboardDragType;
    
        [SerializeField]
        private string blackboardDragTypeString;
            
        public string blackDragData;
        
        /// TODO : The type handling in a safer way in the future
        [Output("",PortNameHandling.MemberType,TypeHandling.Path,TypePath = "BlackboardDragType")]
        public object Value{
            get{
                if (!isListElement){
                     return BlackboardData.GetValue(BlackDragData);
                }
                else{
                    var split = BlackDragData.Split('.');
                    var index = int.Parse(split[1]);
                    return BlackboardData.GetListValue(split[0],index);
                }
               
            }
        }

        public bool isListElement=false;
        public void OnBeforeSerialize(){
            if (BlackboardDragType != null){
                blackboardDragTypeString = BlackboardDragType.AssemblyQualifiedName;
            }
           
        }

        public void OnAfterDeserialize(){
            if(blackboardDragTypeString != null){
                BlackboardDragType = Type.GetType(blackboardDragTypeString);
            }
        
        }
    }
}