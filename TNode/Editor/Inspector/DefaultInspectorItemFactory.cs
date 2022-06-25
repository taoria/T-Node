using System;
using TNode.Cache;
using TNode.Editor.Inspector.InspectorImplementation;
using UnityEditor;
using UnityEngine.UIElements;

namespace TNode.Editor.Inspector{
    public class DefaultInspectorItemFactory{
      
        public InspectorItem<T> Create<T>(){
            //Check type of T
            var hasSpecificType = NodeEditorExtensions.HasSpecificType<InspectorItem<T>>();
            if (hasSpecificType){
                return NodeEditorExtensions.CreateInstance<InspectorItem<T>>();
            }
            else{
                return DefaultInspectorItem<T>();
            }
        }
        
        public static InspectorItem<T> DefaultInspectorItem<T>(){
            DefaultInspectorItem<T> item = new DefaultInspectorItem<T>();
            if (typeof(string) == typeof(T)){
                item.FoldOut.Add(new TextField(){
                    name = "StringTextField"
                });
            }

            return item;

        }
    }
}
    