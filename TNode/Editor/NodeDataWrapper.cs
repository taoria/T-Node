using System;
using System.Collections.Generic;
using TNode.Models;

namespace TNode.Editor{
    public class NodeDataWrapper{
        private readonly NodeData _data;
        private static readonly Dictionary<NodeData,NodeDataWrapper> Cache = new ();
        public event Action<NodeDataWrapper> OnValueChanged;
        public static NodeDataWrapper Get(NodeData data){
            if(Cache.ContainsKey(data)){
                return Cache[data];
            }
            var wrapper = new NodeDataWrapper(data);
            Cache.Add(data,wrapper);
            return wrapper;
        }
        public NodeDataWrapper(NodeData data){
            this._data = data;
        }
      
        public void SetValue(string path, object value){
            var fieldInfo = _data.GetType().GetField(path);
            fieldInfo.SetValue(_data,value);
            OnValueChanged?.Invoke(this);
        }

        public object GetValue(string path){
            var fieldInfo = _data.GetType().GetField(path);
            return fieldInfo.GetValue(_data);
        }
        public static implicit operator NodeData(NodeDataWrapper wrapper){
            if (wrapper == null)
                return null;
            return wrapper._data;
        }
        public static implicit operator NodeDataWrapper(NodeData unWrapper){
            if (unWrapper == null)
                return null;
            return Get(unWrapper);
        }

    }
}