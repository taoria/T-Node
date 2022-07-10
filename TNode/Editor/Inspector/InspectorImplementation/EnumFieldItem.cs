using System;
using TNode.Attribute;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNode.Editor.Inspector.InspectorImplementation{
    [NodeComponent]
    [Obsolete]
    public class EnumFieldItem:InspectorItem<Enum>{
        public EnumFieldItem() : base(){
            var field = new EnumField();
            Debug.Log("An Enum Field is created");
            CreateBindable(field);
            OnDataChanged += () => {
                
                field.Init(Value);
                Debug.Log(Value.GetType());
            };
        }
    }
}