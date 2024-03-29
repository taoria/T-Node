﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace TNodeCore.Runtime{
    [Serializable]
    public class DataWrapper<TWrapper,TData>:ScriptableObject where TWrapper:DataWrapper<TWrapper,TData>,new(){
        public const string DataPath = "data";
        [SerializeReference]
        public TData data;
        protected static readonly Dictionary<TData,TWrapper> Cache = new Dictionary<TData,TWrapper> ();
        public static TWrapper Get(TData data){
            if (data.GetType().IsGenericType){
                return CreateInstance<TWrapper>();
            }
            if(Cache.ContainsKey(data)){
                return Cache[data]==null?CreateInstance<TWrapper>():Cache[data];
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
        public void ForceNotify(){
            OnValueChanged?.Invoke(this);
        }
        public object GetValue(string path){
            var fieldInfo = data.GetType().GetField(path);
            return fieldInfo.GetValue(data);
        }

        public KeyValuePair<string,object>[] GetAllFieldValue(){
            var fieldInfos = data.GetType().GetFields();
            var list = new List<KeyValuePair<string,object>>();
            foreach (var fieldInfo in fieldInfos){
                list.Add(new KeyValuePair<string, object>(fieldInfo.Name,fieldInfo.GetValue(data)));
            }
            return list.ToArray();
        }
        public virtual TData GetData(){
            return data;
        }
        
        public static implicit operator TData(DataWrapper<TWrapper,TData> wrapper){
            if (wrapper == null)
                return default(TData);
            return wrapper.GetData();
     
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