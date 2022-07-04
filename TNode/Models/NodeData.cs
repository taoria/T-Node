using System;
using TNode.Attribute;
using UnityEngine;

namespace TNode.Models{
    /// <summary>
    /// this class is used to store the data of a node
    /// inherit it to implement your own node
    /// when declare a port for this node,you can use attribute [PortTypeName] on a field to claim a port.a port will not be inspected by default inspector.
    /// fields that are not marked with [PortTypeName] will be inspected by default inspector.
    /// 
    /// </summary>
    [Serializable]
    public class NodeData:IModel{
   
        public NodeData() : base(){
            //Object Registration
           
        }
        [DisableOnInspector]
        public string id;
        public string nodeName;
        public bool entryPoint;


        public virtual void OnProcess(){
            
        }
        
// #if UNITY_EDITOR
//         public Rect rect;
// #endif
    }
}