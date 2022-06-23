using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace TNode.Models{
    [Serializable]
    public class NodeEditorData<T> where T:NodeData{
        [SerializeReference]
        private T nodeData;
        
        public Rect nodePos;
    }
}