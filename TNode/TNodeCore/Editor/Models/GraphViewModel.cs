using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace TNode.TNodeCore.Editor.Models{
    /// <summary>
    /// It's called the graphview - model .not the ViewModel concept in MVVM.
    /// Help graph view to persist its own data
    /// </summary>
    [Serializable]
    public class GraphViewModel:EditorModel{
        /// <summary>
        /// The scaling factor of a graph view.
        /// </summary>
        public float persistScale = 1f;
        /// <summary>
        /// The offset of a graph view in the canvas
        /// </summary>
        public Vector2 persistOffset = Vector2.zero;
        /// <summary>
        ///Is this graph view have a blackboard turn on.
        /// </summary>
        public bool isBlackboardOn;
    }
}