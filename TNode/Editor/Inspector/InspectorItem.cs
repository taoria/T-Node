using TNode.Models;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNode.Editor.Inspector{
    public abstract class InspectorItem<T>:VisualElement,INodeDataBinding<T>{
        private NodeData _bindingNodeData;
        private string _bindingFieldName;
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

        public  InspectorItem(){
            OnValueChanged+= OnValueChangedHandler;
        }

        private void OnValueChangedHandler(){


        }
        ~InspectorItem(){
            OnValueChanged-= OnValueChangedHandler;
        }
    }
}