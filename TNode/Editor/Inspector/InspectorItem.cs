using TNode.BaseViews;
using TNode.Models;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNode.Editor.Inspector{
    public abstract class InspectorItem<T>:VisualElement,INodeDataBinding<T>{
        private NodeData _bindingNodeData;
        private string _bindingFieldName;
        protected BaseField<T> Bindable;
        protected event System.Action OnValueChanged;

        public string BindingPath{
            get => _bindingFieldName;
            set{
                _bindingFieldName = value;
                if(_bindingFieldName!=null&&_bindingNodeData!=null){
                    OnValueChanged?.Invoke();
                }
            }
        }

        public NodeData BindingNodeData{
            get => _bindingNodeData;
            set{
                _bindingNodeData = value;
                if(_bindingFieldName!=null&&_bindingNodeData!=null){
                    OnValueChanged?.Invoke();
                }
                
            }
        }

        private T GetValue(){
        
            var fieldInfo = _bindingNodeData.GetType().GetField(BindingPath, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            Debug.Log(fieldInfo);
            Debug.Log(fieldInfo?.FieldType );
            //check field type
            if (fieldInfo != null && fieldInfo.FieldType == typeof(T)){
                return (T)fieldInfo.GetValue(BindingNodeData);
            }
            else{
                Debug.LogError("Wrong Type for current node data");
            }
            return default;
        }

        protected T Value => GetValue();

        protected void SetValue(T value){
            var fieldInfo = _bindingNodeData.GetType().GetField(BindingPath, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            Debug.Log(fieldInfo);
            Debug.Log(fieldInfo?.FieldType );
            if (fieldInfo != null && fieldInfo.FieldType == typeof(T)){
                fieldInfo.SetValue(_bindingNodeData,value);
                
                //if value changed ,notify node inspector's current inspecting node view
                if (parent.parent is NodeInspector nodeInspector){
                    nodeInspector.NodeView.OnDataModified();
                }
            }
            else{
                Debug.LogError("Wrong Type for current node data");
            }
        }
        public  InspectorItem(){
           
            OnValueChanged+= OnValueChangedHandler;
        }
        public void CreateBindable(BaseField<T> bindable){
            Bindable = bindable;
            this.Add(Bindable);
            Bindable?.RegisterValueChangedCallback(e => {
                SetValue(e.newValue);
            });
        }
        private void OnValueChangedHandler(){
            Bindable = this.Q<BaseField<T>>();
            if(Bindable!= null){
                Bindable.value = Value;
                Bindable.label = BindingPath;
            }
        }
        ~InspectorItem(){
            OnValueChanged-= OnValueChangedHandler;
        }
    }
}