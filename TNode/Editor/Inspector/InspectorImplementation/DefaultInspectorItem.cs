using UnityEngine.UIElements;

namespace TNode.Editor.Inspector.InspectorImplementation{
    public class DefaultInspectorItem<T>:InspectorItem<T>{
        public Foldout FoldOut;
        public DefaultInspectorItem(){
            var foldout = new Foldout{
                text = ""
            };
            this.Add(foldout);
            OnValueChanged += () => {
                foldout.text = this.BindingPath;
            };
        }
    }
}