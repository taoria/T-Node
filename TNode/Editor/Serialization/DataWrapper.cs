using System;
using System.Collections.Generic;
using TNode.Models;
using UnityEngine;

namespace TNode.Editor.Serialization{
    [Serializable]
    public class DataWrapper<TWrapper,TData>:ScriptableObject where TWrapper:DataWrapper<TWrapper,TData> where TData:IModel,new(){
        [SerializeReference]
        public TData data;
        private static readonly Dictionary<TData,TWrapper> Cache = new ();
        public static TWrapper Get(TData data){
            if (data.GetType().IsGenericType){
                return CreateInstance<TWrapper>();
            }
            if(Cache.ContainsKey(data)){
                return Cache[data];
            }
            var wrapper = CreateInstance<TWrapper>();
            wrapper.data = data;
            Cache.Add(data,wrapper);
            return wrapper;
        }
        public event Action<DataWrapper<TWrapper,TData>> OnValueChanged;

        public void SetValue(string path, object value){
            var fieldInfo = data.GetType().GetField(path);
            fieldInfo.SetValue(data,value);
            OnValueChanged?.Invoke(this);
        }

        public object GetValue(string path){
            var fieldInfo = data.GetType().GetField(path);
            return fieldInfo.GetValue(data);
        }
        public static implicit operator TData(DataWrapper<TWrapper,TData> wrapper){
            if (wrapper == null)
                return default(TData);
            return wrapper.data;
     
        }
        /// <summary>
        /// Use this to get the wrapped data directly.
        /// </summary>
        /// <param name="unWrapper"></param>
        /// <returns></returns>
        public static implicit operator DataWrapper<TWrapper,TData>(TData unWrapper){
            if (unWrapper == null)
                return null;
            return Get(unWrapper);
        }
    }
}