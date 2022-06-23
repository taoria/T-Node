using System;
using System.Collections.Generic;
using TNode.Attribute;
using TNode.BaseViews;
using UnityEngine;

namespace TNode.Cache{
    internal class NodeEditorSingleton{
        private static NodeEditorSingleton _instance;
        public readonly Dictionary<Type,Type> FromGenericToSpecific = new Dictionary<Type, Type>();
        public static NodeEditorSingleton Instance{
            get{ return _instance ??= new NodeEditorSingleton(); }
        }
        private NodeEditorSingleton(){
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()){
                foreach(var type in assembly.GetTypes()){
                    if(type.IsClass && !type.IsAbstract){
                        foreach(var attribute in type.GetCustomAttributes(typeof(NodeComponentAttribute), false)){
                            //fetch this type 's parent class
                            var parent = type.BaseType;
                            //Check if this type is a generic type and is a generic type of NodeView or DataGraphView
                            if(parent is{IsGenericType: true} && (parent.GetGenericTypeDefinition() == typeof(NodeView<>) || parent.GetGenericTypeDefinition() == typeof(DataGraphView<>))){
                                //Get the generic type of this type
                                //Add this type to the dictionary
                                Debug.Log($"Find a component named {type} and its parent is {parent}" );
                                FromGenericToSpecific.Add(parent, type);
                            }
                            
                        }
                    }
                }
            }
        }
    }
    public static class NodeEditorExtensions{
        public static T CreateInstance<T>(){
            Debug.Log($"Create A instance of {typeof(T)}");
            var implementedType = NodeEditorSingleton.Instance.FromGenericToSpecific[typeof(T)];
            var instance = (T)Activator.CreateInstance(implementedType);
            return instance;
        }
        public static object CreateInstance(Type t){
            Debug.Log($"Create A instance of {t}");
            var implementedType = NodeEditorSingleton.Instance.FromGenericToSpecific[t];
            var instance = Activator.CreateInstance(implementedType);
            return instance;
        }
    }
}