using System;
using TNode.Cache;
using TNode.Editor.Inspector.InspectorImplementation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNode.Editor.Inspector{
    [Obsolete]
    public class InspectorItemFactory{
      
        public InspectorItem<T> Create<T>(){
            //Check type of GraphDataType
            var hasSpecificType = NodeEditorExtensions.HasSpecificTypeComponent<InspectorItem<T>>();
           
            if (hasSpecificType){
                return NodeEditorExtensions.CreateViewComponentFromBaseType<InspectorItem<T>>();
            }

            if (typeof(T).IsEnum){
              
                return NodeEditorExtensions.CreateViewComponentFromBaseType(typeof(InspectorItem<Enum>)) as InspectorItem<T>;
            }
            return null;
        }

        public INodeDataBindingBase Create(Type t){
            var genericType = typeof(InspectorItem<>).MakeGenericType(t);
            var hasSpecificType = NodeEditorExtensions.HasSpecificTypeComponent(genericType);
           
            if (hasSpecificType){
                return NodeEditorExtensions.CreateViewComponentFromBaseType(genericType) as INodeDataBindingBase;
            }

            if (t.IsEnum){
              
                return NodeEditorExtensions.CreateViewComponentFromBaseType(typeof(InspectorItem<Enum>)) as INodeDataBindingBase;
            }
            return null;
        }
        
    }
}
    