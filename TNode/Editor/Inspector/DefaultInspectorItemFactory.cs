using System;
using TNode.Cache;
using TNode.Editor.Inspector.InspectorImplementation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNode.Editor.Inspector{
    public class DefaultInspectorItemFactory{
      
        public InspectorItem<T> Create<T>(){
            //Check type of GraphDataType
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
                var textField = new TextField(){
                    name = "StringTextField"
                };
                item.foldOut.Add(textField);
                textField.RegisterCallback<ChangeEvent<string>>(e => {
                    Debug.Log(item.BindingNodeData);
                    Debug.Log(item.BindingPath);
                    item.BindingNodeData.GetType().GetField(item.BindingPath).SetValue(item.BindingNodeData, e.newValue);
                    if (item.parent.parent is NodeInspector nodeInspector){
                        Debug.Log("item 's parent 's parent is exactly a node inspector");
                        nodeInspector.NodeView.OnDataModified();
                    }
                       
                });
            }
            return item;

        }
    }
}
    