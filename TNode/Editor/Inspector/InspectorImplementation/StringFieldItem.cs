using System;
using TNode.Attribute;
using UnityEngine.UIElements;

namespace TNode.Editor.Inspector.InspectorImplementation{
    [Obsolete]
    /// <summary>
    /// Force these element to bind native c# property
    /// </summary>
    [ViewComponent]
    public class StringFieldItem:InspectorItem<string>{
        public StringFieldItem():base(){
            CreateBindable(new TextField());
        }
    }
}