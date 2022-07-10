using System;
using System.Collections.Generic;
using TNode.Models;
using UnityEngine;

namespace TNode.Editor{
    /// <summary>
    /// Scriptable object wrapper enable property drawer for  t-node
    /// </summary>
    public class NodeDataWrapper<T> : ScriptableObject  where T : NodeData{
        public T Data;
        private static readonly Dictionary<T,NodeDataWrapper<T>> Cache = new ();
        public event Action<NodeDataWrapper<T>> OnValueChanged;
        public static NodeDataWrapper<T> Get(T data){
            if(Cache.ContainsKey(data)){
                return Cache[data];
            }
            var wrapper = ScriptableObject.CreateInstance<NodeDataWrapper<T>>();
            Cache.Add(data,wrapper);
            return wrapper;
        }
        public NodeDataWrapper(T data){
            this.Data = data;
        }
      
        public void SetValue(string path, object value){
            var fieldInfo = Data.GetType().GetField(path);
            fieldInfo.SetValue(Data,value);
            OnValueChanged?.Invoke(this);
        }

        public object GetValue(string path){
            var fieldInfo = Data.GetType().GetField(path);
            return fieldInfo.GetValue(Data);
        }
        public static implicit operator T(NodeDataWrapper<T> wrapper){
            if (wrapper == null)
                return null;
            return wrapper.Data;
     
        }
        public static implicit operator NodeDataWrapper<T>(T unWrapper){
            if (unWrapper == null)
                return null;
            return Get(unWrapper);
        }
    }

    public class NodeDataWrapper:ScriptableObject{
        [SerializeReference]
        public NodeData Data;
        private static readonly Dictionary<NodeData,NodeDataWrapper> Cache = new ();
        public event Action<NodeDataWrapper> OnValueChanged;
        public static NodeDataWrapper Get(NodeData data){
            if (data.GetType().IsGenericType){
                return ScriptableObject.CreateInstance<NodeDataWrapper>();
            }
            if(Cache.ContainsKey(data)){
                return Cache[data];
            }
            var wrapper = ScriptableObject.CreateInstance<NodeDataWrapper>();
            wrapper.Data = data;
            Cache.Add(data,wrapper);
            return wrapper;
        }
  
      
        public void SetValue(string path, object value){
            var fieldInfo = Data.GetType().GetField(path);
            fieldInfo.SetValue(Data,value);
            OnValueChanged?.Invoke(this);
        }

        public object GetValue(string path){
            var fieldInfo = Data.GetType().GetField(path);
            return fieldInfo.GetValue(Data);
        }
        public static implicit operator NodeData(NodeDataWrapper wrapper){
            if (wrapper == null)
                return null;
            return wrapper.Data;
     
        }
        public static implicit operator NodeDataWrapper(NodeData unWrapper){
            if (unWrapper == null)
                return null;
            return Get(unWrapper);
        }
    }
}