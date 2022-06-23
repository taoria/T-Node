using System;
using TNode.BaseModels;
using UnityEngine;

namespace TNode.Models{
    [Serializable]
    public class NodeData:IModel{
   
        public NodeData() : base(){
            //Object Registration
        }
        public string nodeName;
        public bool entryPoint;
#if UNITY_EDITOR
        public Rect rect;
#endif

    }
}