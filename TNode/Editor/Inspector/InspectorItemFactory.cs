using System;
using TNode.Cache;
using TNode.Editor.Inspector.InspectorImplementation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNode.Editor.Inspector{
    public class InspectorItemFactory{
      
        public InspectorItem<T> Create<T>(){
            //Check type of GraphDataType
            var hasSpecificType = NodeEditorExtensions.HasSpecificType<InspectorItem<T>>();
            Debug.Log(typeof(InspectorItem<T>));
            Debug.Log(hasSpecificType);
            if (hasSpecificType){
                return NodeEditorExtensions.CreateInstance<InspectorItem<T>>();
            }
            return null;
        }
        
    }
}
    