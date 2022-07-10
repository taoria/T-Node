using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace TNode.Editor.Inspector.InspectorImplementation{
    [Obsolete]
    public class PropertyFieldItem:InspectorItem<Object>{

        public PropertyFieldItem(){
           
            
            OnDataChanged += () => {
                var data = new SerializedObject(Value as Object);
                var testProperty = data.GetIterator().GetArrayElementAtIndex(0);
                PropertyField propertyField = new PropertyField(testProperty);
                this.Q<PropertyField>()?.RemoveFromHierarchy();
                this.Add(propertyField);
                
            };
        }
        
    }
}