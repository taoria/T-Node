using System;
using UnityEngine;

namespace TNode{
    [Serializable]
    public class NodeData{
   
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