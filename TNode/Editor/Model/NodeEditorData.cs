using System;
using TNode.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace TNode.Editor.Model{
    [Serializable]
    
    public class NodeEditorData{
        public string nodeGuid;
        public Rect nodePos;
    }
}