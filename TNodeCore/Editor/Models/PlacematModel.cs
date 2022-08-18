using System;
using System.Collections.Generic;
using TNode.TNodeCore.Editor.Models;
using TNodeCore.Runtime.Models;
using UnityEngine;

namespace TNodeCore.Editor.Models{
    /// <summary>
    /// Placemats hold the nodes
    /// </summary>
    [Serializable]
    public class PlacematModel:EditorModel{
        /// <summary>
        /// In the experimental Graphview, no hostModels should be add.ignore it in most cases.
        /// </summary>
        [SerializeReference]
        public List<Model> hostModels = new List<Model>();
        /// <summary>
        /// zOrder of the placemat
        /// </summary>
        public int zOrder;
        /// <summary>
        /// title of the placemat
        /// </summary>
        public string title;
    }
}