using TNode.Models;
using UnityEngine;

namespace TNode.Editor.Inspector{
    public interface INodeDataBinding<out T>:INodeDataBindingBase{

        

        public void OnBindingDataUpdate(){
            
        }
    }
}