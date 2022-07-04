using System;
using TNode.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace TNode.Editor.Model{
    [Serializable]
    
    public class GraphElementEditorData{
        public string guid;
        public Rect pos;
    }
}