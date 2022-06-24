using System;
using JetBrains.Annotations;
using UnityEngine;

namespace TNode.Attribute{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [BaseTypeRequired(typeof(ScriptableObject))]
    public class AssetMenuCreateAsGraphEditorAttribute:System.Attribute{
        
    }
}