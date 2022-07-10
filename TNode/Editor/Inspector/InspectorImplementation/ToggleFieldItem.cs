using System;
using TNode.Attribute;
using UnityEngine.UIElements;

namespace TNode.Editor.Inspector.InspectorImplementation{
    [Obsolete]
    [NodeComponent]
    public class ToggleFieldItem:InspectorItem<bool>{
        public ToggleFieldItem(){
            CreateBindable(new Toggle());
        }
        
    }
}