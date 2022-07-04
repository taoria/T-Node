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
        public delegate object GetValueDelegate(NodeData nodeData);

        public readonly Dictionary<Type, List<GetValueDelegate>> CachedDelegatesForGettingValue =
            new ();

        public void ExecuteOutput<T>(T nodeData) where  T:NodeData{
            var type = typeof(T);
            if(!CachedDelegatesForGettingValue.ContainsKey(type)){
                return;
            }
            var delegates = CachedDelegatesForGettingValue[type];
            foreach(var delegateInstance in delegates){
                var value = delegateInstance(nodeData);
            }
        }

        public void RegisterRuntimeNode<T>() where T:NodeData{
            var type = typeof(T);
            if(!CachedDelegatesForGettingValue.ContainsKey(type)){
                CachedDelegatesForGettingValue.Add(type, new List<GetValueDelegate>());
            }
            //iterate properties of the nodeData and add them to the cache
            var properties = type.GetProperties();
            foreach(var property in properties){
                var getValueDelegate = GetValueDelegateForProperty(property);
                CachedDelegatesForGettingValue[type].Add(getValueDelegate);
            }
            
        }

        private GetValueDelegate GetValueDelegateForProperty(PropertyInfo property){
            var getValueDelegate = (GetValueDelegate)Delegate.CreateDelegate(typeof(GetValueDelegate), property.GetGetMethod());
            return getValueDelegate;
        }
    }
}