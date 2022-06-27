using TNode.Models;
using UnityEngine;

namespace TNode.Editor.Inspector{
    public interface INodeDataBinding<out T>:INodeDataBindingBase{
        protected T GetValue(){
        
            var fieldInfo = typeof(T).GetField(BindingPath, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            //check field type
            if (fieldInfo != null && fieldInfo.FieldType == typeof(T)){
                return (T)fieldInfo.GetValue(BindingNodeData);
            }
            else{
                Debug.LogError("Wrong Type for current node data");
            }
            return default;
        }        

        public T Value => GetValue();

        public void OnBindingDataUpdate(){
            
        }
    }
}