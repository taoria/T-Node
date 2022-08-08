using System;
using TNodeCore.Editor.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace TNode.TNodeCore.Editor.Models{
    [Serializable]
    public class GraphViewData:EditorModel{
        public float persistScale = 1f;
        public Vector2 persistOffset = Vector2.zero;
        public bool isBlackboardOn;
    }
}