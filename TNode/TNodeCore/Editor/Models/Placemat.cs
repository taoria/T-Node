using System;
using System.Collections.Generic;
using TNodeCore.Runtime.Models;
using UnityEngine;

namespace TNodeCore.Editor.Models{
    [Serializable]
    public class Placemat:EditorModel{
        [SerializeReference]
        public List<Model> hostModels = new List<Model>();
    }
}