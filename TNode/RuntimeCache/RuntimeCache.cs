using System;
using System.Collections.Generic;
using System.Reflection;
using EasyRandomGenerator.Blackboard;
using JetBrains.Annotations;
using TNode.Models;
using Unity.VisualScripting;
using UnityEngine.UI;

namespace TNode.RuntimeCache{
    public class RuntimeCache{
        //Singleton instance for the runtime cache
        private static RuntimeCache _instance;
        public static RuntimeCache Instance{
            get{ return _instance ??= new RuntimeCache(); }
        }
        //delegate return a value from a nodedata
        public delegate object GetValueDelegate(IModel nodeData);
        public delegate void SetValueDelegate(object nodeData,object value);

        public readonly Dictionary<Type, Dictionary<string,GetValueDelegate>> CachedDelegatesForGettingValue =
            new ();
        public readonly Dictionary<Type,Dictionary<string,SetValueDelegate>> CachedDelegatesForSettingValue =
            new ();

        private static readonly string[] ExcludedAssemblies = new string[]{"Microsoft", "UnityEngine","UnityEditor","mscorlib","System"};

        public RuntimeCache(){
            RegisterRuntimeBlackboard(typeof(EasyBlackboardData));
        }
        public void RegisterRuntimeBlackboard(Type type){
            if (type == null) return;
            if(!CachedDelegatesForGettingValue.ContainsKey(type)){
                CachedDelegatesForGettingValue.Add(type, new Dictionary<string, GetValueDelegate>());
                CachedDelegatesForSettingValue.Add(type,new Dictionary<string, SetValueDelegate>());
              
                var properties = type.GetProperties();
                foreach(var property in properties){
                    //if the property only has a setter ,skip 
          
                    var getValueDelegate = GetValueDelegateForProperty(property);
                    CachedDelegatesForGettingValue[type].Add(property.Name,getValueDelegate);
                    
                    var setValueDelegate = SetValueDelegateForProperty(property);
                    CachedDelegatesForSettingValue[type].Add(property.Name,setValueDelegate);
                }
                //register the fields
                var fields = type.GetFields();
                foreach(var field in fields){
                    var getValueDelegate = GetValueDelegateForField(field);
                    CachedDelegatesForGettingValue[type].Add(field.Name,getValueDelegate);
                    
                    var setValueDelegate = SetValueDelegateForField(field);
                    CachedDelegatesForSettingValue[type].Add(field.Name,setValueDelegate);
                }
            }
        }

        private GetValueDelegate GetValueDelegateForField(FieldInfo field){
            return field.GetValue;
        }
        private SetValueDelegate SetValueDelegateForField(FieldInfo field){
            return field.SetValue;
        }
        private GetValueDelegate GetValueDelegateForProperty(PropertyInfo property){
            var getValueDelegate = (GetValueDelegate)Delegate.CreateDelegate(typeof(GetValueDelegate), property.GetGetMethod());
            return getValueDelegate;
        }
        private SetValueDelegate SetValueDelegateForProperty(PropertyInfo property){
            var setValueDelegate = (SetValueDelegate)Delegate.CreateDelegate(typeof(SetValueDelegate), property.GetSetMethod());
            return setValueDelegate;
        }
        
    }
    public static class RuntimeExtension{
     
        //todo  latter on i will try some way caching reflection more efficiently
        public static T GetValue<T>(this BlackboardData blackboardData,string path){
            var method = RuntimeCache.Instance.CachedDelegatesForGettingValue[blackboardData.GetType()][path];
            return (T) method.Invoke(blackboardData);
        }
        public static object GetValue(this BlackboardData blackboardData, string path){
            var method = RuntimeCache.Instance.CachedDelegatesForGettingValue[blackboardData.GetType()][path];
            return method.Invoke(blackboardData);
        }
        public static void SetValue<T>(this BlackboardData blackboardData,string path,T value){
            var method = RuntimeCache.Instance.CachedDelegatesForSettingValue[blackboardData.GetType()][path];
            method.Invoke(blackboardData,value);
        }
        public static void SetValue(this BlackboardData blackboardData,string path,object value){
            var method = RuntimeCache.Instance.CachedDelegatesForSettingValue[blackboardData.GetType()][path];
            method.Invoke(blackboardData,value);
        }
        public static RuntimeCache.GetValueDelegate GetValueDelegate(this BlackboardData blackboardData,string path){
            var method = RuntimeCache.Instance.CachedDelegatesForGettingValue[blackboardData.GetType()][path];
            return method;
        }
        
    }
}