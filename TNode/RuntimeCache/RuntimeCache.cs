using System;
using System.Collections.Generic;
using System.Reflection;
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
        public delegate object SetValueDelegate(IModel nodeData,object value);

        public readonly Dictionary<Type, Dictionary<string,GetValueDelegate>> CachedDelegatesForGettingValue =
            new ();
        
        private static readonly string[] ExcludedAssemblies = new string[]{"Microsoft", "UnityEngine","UnityEditor","mscorlib","System"};
        public void RegisterRuntimeBlackboard(Type type){
            if(!CachedDelegatesForGettingValue.ContainsKey(type)){
                CachedDelegatesForGettingValue.Add(type, new Dictionary<string, GetValueDelegate>());
                var properties = type.GetProperties();
                foreach(var property in properties){
                    //if the property only has a setter ,skip 
                    if(property.GetMethod == null){
                        continue;
                    }
                    var getValueDelegate = GetValueDelegateForProperty(property);
                    CachedDelegatesForGettingValue[type].Add(property.Name,getValueDelegate);
                }
                //register the fields
                var fields = type.GetFields();
                foreach(var field in fields){
                    var getValueDelegate = GetValueDelegateForField(field);
                    CachedDelegatesForGettingValue[type].Add(field.Name,getValueDelegate);
                }
            }
        }

        private GetValueDelegate GetValueDelegateForField(FieldInfo field){
            return field.GetValue;
        }


        private GetValueDelegate GetValueDelegateForProperty(PropertyInfo property){
            var getValueDelegate = (GetValueDelegate)Delegate.CreateDelegate(typeof(GetValueDelegate), property.GetGetMethod());
            return getValueDelegate;
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
        public static RuntimeCache.GetValueDelegate GetValueDelegate(this BlackboardData blackboardData,string path){
            var method = RuntimeCache.Instance.CachedDelegatesForGettingValue[blackboardData.GetType()][path];
            return method;
        }
        
    }
}