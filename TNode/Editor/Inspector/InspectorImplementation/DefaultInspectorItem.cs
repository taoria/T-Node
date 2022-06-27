using UnityEngine.UIElements;

namespace TNode.Editor.Inspector.InspectorImplementation{
    public class DefaultInspectorItem<T>:InspectorItem<T>{
        public readonly Foldout foldOut;
        public DefaultInspectorItem(){
            foldOut = new Foldout{
                text = ""
            };
            this.Add(foldOut);
            OnValueChanged += () => {
                foldOut.text = this.BindingPath;
            };
        }
    }
}