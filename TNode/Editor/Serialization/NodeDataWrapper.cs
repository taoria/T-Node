using System;
using System.Collections.Generic;
using TNode.Models;
using UnityEngine;
using UnityEngine.Serialization;


namespace TNode.Editor.Serialization{
    [Obsolete]

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
    public class NodeDataWrapper:DataWrapper<NodeDataWrapper,NodeData>{
      
    }
    /// <summary>
    /// Scriptable object wrapper enable property drawer for  t-node
    /// instance create automatically when using get function,generic node data is not support yet because of unity serialization system.
    /// TODO : support generic node data
    /// </summary>
    // public class NodeDataWrapper:ScriptableObject{
    //     [SerializeReference]
    //     public NodeData data;
    //     private static readonly Dictionary<NodeData,NodeDataWrapper> Cache = new ();
    //     public event Action<NodeDataWrapper> OnValueChanged;
    //     /// <summary>
    //     /// Create a new wrapper or get a infoCached wrapper for the given data
    //     /// </summary>
    //     /// <param name="data">node data,an implemented type is acceptable</param>
    //     /// <returns></returns>
    //     public static NodeDataWrapper Get(NodeData data){
    //         if (data.GetType().IsGenericType){
    //             return CreateInstance<NodeDataWrapper>();
    //         }
    //         if(Cache.ContainsKey(data)){
    //             return Cache[data];
    //         }
    //         var wrapper = CreateInstance<NodeDataWrapper>();
    //         wrapper.data = data;
    //         Cache.Add(data,wrapper);
    //         return wrapper;
    //     }
    //
    //   
    //     public void SetValue(string path, object value){
    //         var fieldInfo = data.GetType().GetField(path);
    //         fieldInfo.SetValue(data,value);
    //         OnValueChanged?.Invoke(this);
    //     }
    //
    //     public object GetValue(string path){
    //         var fieldInfo = data.GetType().GetField(path);
    //         return fieldInfo.GetValue(data);
    //     }
    //     public static implicit operator NodeData(NodeDataWrapper wrapper){
    //         if (wrapper == null)
    //             return null;
    //         return wrapper.data;
    //  
    //     }
    //     /// <summary>
    //     /// Use this to get the wrapped data directly.
    //     /// </summary>
    //     /// <param name="unWrapper"></param>
    //     /// <returns></returns>
    //     public static implicit operator NodeDataWrapper(NodeData unWrapper){
    //         if (unWrapper == null)
    //             return null;
    //         return Get(unWrapper);
    //     }
    // }

  
}