using System;
using TNode.Models;
using UnityEngine;

namespace TNode.Editor.Model{
    [Serializable]

    public class NodeEditorData{
        [SerializeReference] public NodeData nodeData;
        public Rect nodePos;
    }
}