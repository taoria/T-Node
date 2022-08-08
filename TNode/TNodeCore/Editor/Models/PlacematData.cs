using System;
using System.Collections.Generic;
using TNodeCore.Runtime.Models;
using UnityEngine;

namespace TNodeCore.Editor.Models{
    [Serializable]
    public class PlacematData:EditorModel{
        [SerializeReference]
        public List<Model> hostModels = new List<Model>();

        public int zOrder;
        public string title;
    }
}