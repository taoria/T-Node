using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PlasticPipe.PlasticProtocol.Messages;
using TNodeCore.Attribute;
using TNodeCore.Models;
using UnityEngine;

namespace TNodeCore.RuntimeCache{
    public class RuntimeCache{
        //Singleton instance for the runtime cache
        private static RuntimeCache _instance;
        public static RuntimeCache Instance{
            get{ return _instance ??= new RuntimeCache(); }
        }
        //delegate return a value from a nodedata
        public delegate object GetValueDelegate(IModel nodeData);
        public delegate void SetValueDelegate(IModel nodeData,object value);

        public delegate object GetPropertyValueDelegate();
        public delegate void SetPropertyValueDelegate(object value);

        public readonly Dictionary<Type, Dictionary<string,GetValueDelegate>> CachedDelegatesForGettingValue =
            new ();
        public readonly Dictionary<Type,Dictionary<string,SetValueDelegate>> CachedDelegatesForSettingValue =
            new ();
        public readonly Dictionary<Type,Dictionary<string,GetPropertyValueDelegate>> CachedDelegatesForGettingPropertyValue =
            new ();
        public readonly Dictionary<Type,Dictionary<string,SetPropertyValueDelegate>> CachedDelegatesForSettingPropertyValue =
            new ();

        private readonly Dictionary<Type, Type> _graphBlackboardDictionary = new Dictionary<Type, Type>();

        private static readonly string[] ExcludedAssemblies = new string[]{"Microsoft", "UnityEngine","UnityEditor","mscorlib","System"};

        public RuntimeCache(){
            //Get types in all assemblies except the excluded ones
            var types = AppDomain.CurrentDomain.GetAssemblies().Where(
                assembly => !ExcludedAssemblies.Contains(assembly.GetName().Name)
            ).SelectMany(assembly => assembly.GetTypes());
            //iterate types to check if they have  GraphUsageAttribute
            foreach (var type in types){
                var attributes = type.GetCustomAttributes();
                foreach (var attribute in attributes){
                    if (attribute is GraphUsageAttribute){
                        //if the type has GraphUsageAttribute, add it to the cache
                        AddTypeToCache(type,attribute as GraphUsageAttribute);
                        
                    }
                }
            }
        }

        private void AddTypeToCache(Type type,GraphUsageAttribute attribute){
            //Check if the type is a blackboard data type
            if(typeof(BlackboardData).IsAssignableFrom(type)){
                //if it is, add it to the cache
                AddBlackboardDataTypeToCache(type,attribute);
                RegisterRuntimeBlackboard(type);
            }
            //Check if the type is a node data type
            if(typeof(NodeData).IsAssignableFrom(type)){
                //if it is, add it to the cache
                RegisterRuntimeNodeData(type);
            }
            
        }
        
        private void AddBlackboardDataTypeToCache(Type type,GraphUsageAttribute attribute){
            var graphData = attribute.GraphDataType;
            _graphBlackboardDictionary.Add(graphData,type);

        }

        public Type GetAppropriateBlackboardDataType(GraphData graphData){
            var type = graphData.GetType();
            if(_graphBlackboardDictionary.ContainsKey(type)){
                return _graphBlackboardDictionary[type];
            }
            return null;
        }
        public BlackboardData GetBlackboardData(GraphData graphData){
            var type = GetAppropriateBlackboardDataType(graphData);
            if(type != null){
                return (BlackboardData) Activator.CreateInstance(type);
            }
            return null;
        }
        public void RegisterRuntimeBlackboard(Type type){
            if (type == null) return;
            if(!CachedDelegatesForGettingValue.ContainsKey(type)){
                CachedDelegatesForGettingValue.Add(type, new Dictionary<string, GetValueDelegate>());
                CachedDelegatesForSettingValue.Add(type,new Dictionary<string, SetValueDelegate>());
              
                // var properties = type.GetProperties();
                // foreach(var property in properties){
                //     //if the property only has a setter ,skip 
                //
                //     var getValueDelegate = GetValueDelegateForProperty(property);
                //     CachedDelegatesForGettingValue[type].Add(property.Name,getValueDelegate);
                //     
                //     var setValueDelegate = SetValueDelegateForProperty(property);
                //     CachedDelegatesForSettingValue[type].Add(property.Name,setValueDelegate);
                // }
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

        public void RegisterRuntimeNodeData(Type type){
            if (type == null) return;
            if(!CachedDelegatesForGettingValue.ContainsKey(type)){
                CachedDelegatesForGettingValue.Add(type, new Dictionary<string, GetValueDelegate>());
                CachedDelegatesForSettingValue.Add(type,new Dictionary<string, SetValueDelegate>());
      
                
                var properties = type.GetProperties();
                foreach(var property in properties){
                    //if the property only has a setter ,skip 
                  
                    // if(property.GetSetMethod(false) != null){
                    //     var setValueDelegate = SetValueDelegateForProperty(property);
                    //     CachedDelegatesForSettingPropertyValue[type].Add(property.Name,setValueDelegate);
                    // }
                    // if(property.GetMethod != null){
                    //     var getValueDelegate = GetValueDelegateForProperty(property);
                    //     CachedDelegatesForGettingPropertyValue[type].Add(property.Name,getValueDelegate);
                    // }
                
                    
                 
                }
                //register the fields
                var fields = type.GetFields();
                foreach(var field in fields){
                    
                    var getValueDelegate = GetValueDelegateForField(field);
                    CachedDelegatesForGettingValue[type].Add(field.Name,getValueDelegate);
                    if (field.IsPublic){
                        var setValueDelegate = SetValueDelegateForField(field);
                        CachedDelegatesForSettingValue[type].Add(field.Name,setValueDelegate);
                    }
    
                }
            }
        }
        private GetValueDelegate GetValueDelegateForField(FieldInfo field){
       
            return field.GetValue;
        }
        private SetValueDelegate SetValueDelegateForField(FieldInfo field){
            
            return field.SetValue;
        }
        private GetPropertyValueDelegate GetValueDelegateForProperty(PropertyInfo property){
            var getValueDelegate = (GetPropertyValueDelegate)Delegate.CreateDelegate(typeof(GetPropertyValueDelegate), property.GetGetMethod());
            return getValueDelegate;
        }
        private SetPropertyValueDelegate SetValueDelegateForProperty(PropertyInfo property){
            Debug.Log(property.GetSetMethod());
      
            var setValueDelegate = (SetPropertyValueDelegate)Delegate.CreateDelegate(typeof(SetPropertyValueDelegate), property.GetSetMethod());
            return setValueDelegate;
        }
        
    }
    public static class RuntimeExtension{
     
        //todo  latter on i will try some way caching reflection more efficiently
        public static T GetValue<T>(this IModel data,string path,Type type=null){
            var method = RuntimeCache.Instance.CachedDelegatesForGettingValue[type??data.GetType()][path];
            return (T) method.Invoke(data);
        }
        public static object GetValue(this  IModel data, string path,Type type=null){
            var method = RuntimeCache.Instance.CachedDelegatesForGettingValue[type??data.GetType()][path];
            return method.Invoke(data);
        }
    
        public static void SetValue<T>(this IModel data,string path,T value,Type type=null){
            var method = RuntimeCache.Instance.CachedDelegatesForSettingValue[type??data.GetType()][path];
            method.Invoke(data,value);
        }
        public static void SetValue(this IModel data,string path,object value,Type type=null){
            var method = RuntimeCache.Instance.CachedDelegatesForSettingValue[type??data.GetType()][path];
            method.Invoke(data,value);
        }

        public static RuntimeCache.GetValueDelegate GetValueDelegate(this  IModel blackboardData,string path){
            var method = RuntimeCache.Instance.CachedDelegatesForGettingValue[blackboardData.GetType()][path];
            return method;
        }
        /// <summary>
        /// it generate a delegate that can get the value fast,but it won't cache in runtime cache system,you should put it in somewhere you need
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="PropertyNotFoundException"></exception>
        public static RuntimeCache.GetPropertyValueDelegate CacheGetProperty(this IModel data,string path){
            var type = data.GetType();
            var property = type.GetProperty(path);
            if (property == null) throw new PropertyNotFoundException(path);
            var instance = Delegate.CreateDelegate(typeof(RuntimeCache.GetPropertyValueDelegate), data,
                property.GetGetMethod());
            return instance as RuntimeCache.GetPropertyValueDelegate;
        }
        /// <summary>
        /// it generate a delegate that can get the value fast,but it won't cache in runtime cache system,you should put it in somewhere you need
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="PropertyNotFoundException"></exception>
        public static RuntimeCache.SetPropertyValueDelegate CacheSetProperty(this IModel data,string path){
            var type = data.GetType();
            var property = type.GetProperty(path);
            if (property == null) throw new PropertyNotFoundException(path);
            var instance = Delegate.CreateDelegate(typeof(RuntimeCache.SetPropertyValueDelegate), data,
                property.GetSetMethod());
            return instance as RuntimeCache.SetPropertyValueDelegate;
        }
    }

    public class PropertyNotFoundException : Exception{
        public PropertyNotFoundException(string path):base("Property not found :"+path){
            
        }
    }
}