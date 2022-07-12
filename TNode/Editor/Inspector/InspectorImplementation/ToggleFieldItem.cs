using System;
using TNode.Attribute;
using UnityEngine.UIElements;

namespace TNode.Editor.Inspector.InspectorImplementation{
    [Obsolete]
    [ViewComponent]
    public class ToggleFieldItem:InspectorItem<bool>{
        public ToggleFieldItem(){
            CreateBindable(new Toggle());
        }
        
    }
}