using TNode.Attribute;
using UnityEngine.UIElements;

namespace TNode.Editor.Inspector.InspectorImplementation{
    /// <summary>
    /// Force these element to bind native c# property
    /// </summary>
    [NodeComponent]
    public class StringFieldItem:InspectorItem<string>{
        public StringFieldItem():base(){
            CreateBindable(new TextField());
        }
    }
}