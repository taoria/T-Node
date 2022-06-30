using System;
using TNode.BaseViews;
using TNode.Models;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNode.Editor.Inspector{
    public abstract class InspectorItem<T>:VisualElement,INodeDataBinding<T> {
        private NodeData _bindingNodeData;
        private string _bindingFieldName;
        protected BaseField<T> Bindable;
        protected event System.Action OnDataChanged;

        public string BindingPath{
            get => _bindingFieldName;
            set{
                _bindingFieldName = value;
                if(_bindingFieldName!=null&&_bindingNodeData!=null){
                    OnDataChanged?.Invoke();
                }
            }
        }

        public NodeData BindingNodeData{
            get => _bindingNodeData;
            set{
                var oldWrapper = ((NodeDataWrapper) _bindingNodeData);
                if(oldWrapper!=null){
                    oldWrapper.OnValueChanged -= OnNodeDataValueChanged;
                }
                _bindingNodeData = value;
                if(_bindingFieldName!=null&&_bindingNodeData!=null){
                    OnDataChanged?.Invoke();
                }
                if(_bindingNodeData!=null)
                    ((NodeDataWrapper) _bindingNodeData).OnValueChanged += OnNodeDataValueChanged;
            }
        }

        private T GetValue(){
        
            var fieldInfo = _bindingNodeData.GetType().GetField(BindingPath, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
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
            NodeDataWrapper wrapper = _bindingNodeData;
            Debug.Log(wrapper);
            wrapper.SetValue(BindingPath,value);
        }
        public  InspectorItem(){
           
            OnDataChanged+= OnDataChangedHandler;
        }
        /*
         *      e => {
            SetValue(e.newValue);
        }
         */
        private void OnInspectorItemValueChanged(ChangeEvent<T> e){
            SetValue(e.newValue);
        }

        public void CreateBindable(BaseField<T> bindable){
            if (Bindable != null){
                Bindable.Clear();
                Bindable.UnregisterValueChangedCallback(OnInspectorItemValueChanged);
            }
            Bindable = bindable;
            this.Add(Bindable);
            Bindable?.RegisterValueChangedCallback(OnInspectorItemValueChanged);
        }
        private void OnDataChangedHandler(){
            Bindable = this.Q<BaseField<T>>();
            if(Bindable!= null){
                Bindable.value = Value;
                Bindable.label = BindingPath;
            }
        }

        private  void OnNodeDataValueChanged(NodeDataWrapper wrapper){
            var value = (T) wrapper.GetValue(BindingPath) ;
            if(Bindable!=null){
                Bindable.value = value;
            }
        }

        ~InspectorItem(){
            OnDataChanged-= OnDataChangedHandler;
        }
    }
}