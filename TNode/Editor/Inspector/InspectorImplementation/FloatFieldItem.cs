using System;
using TNode.Attribute;
using UnityEngine.UIElements;

namespace TNode.Editor.Inspector.InspectorImplementation{
    [NodeComponent]
    public class FloatFieldItem:InspectorItem<float>{
        public FloatFieldItem():base(){
            CreateBindable(new FloatField());
        }
    }
}