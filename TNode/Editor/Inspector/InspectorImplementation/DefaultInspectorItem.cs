using UnityEngine.UIElements;

namespace TNode.Editor.Inspector.InspectorImplementation{
    public class DefaultInspectorItem<T>:InspectorItem<T>{
        public readonly Foldout foldOut;
        public DefaultInspectorItem():base(){
            foldOut = new Foldout{
                text = ""
            };
            this.Add(foldOut);
            OnValueChanged += () => {
                foldOut.text = this.BindingPath;
                var textField = this.Q<TextField>();
                if(textField != null){
                    textField.value = this.Value.ToString();
                }
            };
        }
    }
}