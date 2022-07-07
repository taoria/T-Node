using System;
using System.Collections.Generic;
using System.Linq;
using TNode.Attribute;
using TNode.BaseViews;
using TNode.Editor;
using TNode.Editor.BaseViews;
using TNode.Editor.Inspector;
using TNode.Models;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace TNode.Cache{
    /// <summary>
    /// Internal singleton class for caching TNode reflection Data.
    /// </summary>
    internal class NodeEditorSingleton{
        private static NodeEditorSingleton _instance;
        public readonly Dictionary<Type,Type> FromGenericToSpecific = new Dictionary<Type, Type>();
        public readonly Dictionary<Type, List<Type>> GraphDataUsage = new Dictionary<Type, List<Type>>();
        public Dictionary<Type, Type> GraphBlackboard = new();
        public static NodeEditorSingleton Instance{
            get{ return _instance ??= new NodeEditorSingleton(); }
        }

        private static readonly string[] ExcludedAssemblies = new string[]{"Microsoft", "UnityEngine","UnityEditor","mscorlib","System"};

        private NodeEditorSingleton(){
            //exclude  unity ,system ,and microsoft types
            var assemblies = AppDomain.
                    CurrentDomain.GetAssemblies()
                    .Where(x=>ExcludedAssemblies.All(y=>!x.GetName().Name.Split(".")[0].Equals(y)));
            
            foreach(var assembly in assemblies){
                foreach(var type in assembly.GetTypes()){
                    if(type.IsClass && !type.IsAbstract){
                        //Register Node View And Graph View via its parent class
                        SetNodeComponentAttribute(type);
                        //Register Node Data by GraphUsageAttribute.
                        SetGraphUsageAttribute(type);
                    }
                }
            }
        }

        private void SetGraphUsageAttribute(Type type){
            foreach (var attribute in type.GetCustomAttributes(typeof(GraphUsageAttribute), true)){
                var parent = type.BaseType;
                if (typeof(IModel).IsAssignableFrom(type.BaseType)){
                    //Check if GraphDataUsage dictionary has GraphDataType of attribute

                    if (typeof(NodeData).IsAssignableFrom(type)){
                        if (attribute is GraphUsageAttribute attributeCasted){
                            if (GraphDataUsage.ContainsKey(attributeCasted.GraphDataType)){
                                GraphDataUsage[attributeCasted.GraphDataType].Add(type);
                            }
                            else{
                                GraphDataUsage.Add(attributeCasted.GraphDataType, new List<Type>{type});
                            }
                        }
                    }

                    if (typeof(BlackboardData).IsAssignableFrom(type)){
                        if (attribute is GraphUsageAttribute attributeCasted){
                            if (GraphBlackboard.ContainsKey(attributeCasted.GraphDataType)){
                                GraphBlackboard[attributeCasted.GraphDataType] = type;
                                
                            }
                            else{
                                GraphBlackboard.Add(attributeCasted.GraphDataType, type);
                            }
            
                        }
                    }
                }
            }
        }
        private readonly Type[] _acceptedTypesForGenericToSpecific = new Type[]{typeof(NodeView<>),typeof(DataGraphView<>),typeof(InspectorItem<>)};
        private void SetNodeComponentAttribute(Type type){
            foreach (var attribute in type.GetCustomAttributes(typeof(NodeComponentAttribute), false)){
                //fetch this type 's parent class
                var parent = type.BaseType;
                //Check if this type is a generic type and is a generic type of NodeView or DataGraphView
                if (parent is{IsGenericType: true} && _acceptedTypesForGenericToSpecific.Contains(parent.GetGenericTypeDefinition())){
                    //Get the generic type of this type
                    //Add this type to the dictionary
                    FromGenericToSpecific.Add(parent, type);
                }
                //TODO Note that a node component only applied to a specific type of editor,so ,same GraphView could behave differently in different editor.it's a todo feature.
            }
        }
    }
    //Outer wrapper for the singleton class
    public static class NodeEditorExtensions{
        public static T CreateNodeComponentFromGenericType<T>(){
            var implementedType = NodeEditorSingleton.Instance.FromGenericToSpecific[typeof(T)];
            var instance = (T)Activator.CreateInstance(implementedType);
            return instance;
        }
        public static object CreateNodeComponentFromGenericType(Type t){
            if (NodeEditorSingleton.Instance.FromGenericToSpecific.ContainsKey(t)){
                var implementedType = NodeEditorSingleton.Instance.FromGenericToSpecific[t];
                var instance = Activator.CreateInstance(implementedType);
                return instance;
            }
            Debug.Log($"No given type found {t}");
            //check if t is a generic type node view
            if (t is{IsGenericType: true} && t.GetGenericTypeDefinition() == typeof(NodeView<>)){
                var instance = Activator.CreateInstance(typeof(NodeView<NodeData>));
                return instance;
            }
            return null;
        }
        public static bool HasSpecificTypeComponent<T>() where T : class{

            return NodeEditorSingleton.Instance.FromGenericToSpecific.ContainsKey(typeof(T));
        }
        public static bool HasSpecificTypeComponent(Type t) {

            return NodeEditorSingleton.Instance.FromGenericToSpecific.ContainsKey(t);
        }
        public static List<Type> GetGraphDataUsage(Type t){
            if (NodeEditorSingleton.Instance.GraphDataUsage.ContainsKey(t)){
                return NodeEditorSingleton.Instance.GraphDataUsage[t];
            }
            return new List<Type>();
        }
        public static BlackboardData GetAppropriateBlackboardData(Type t){
            if (NodeEditorSingleton.Instance.GraphBlackboard.ContainsKey(t)){
                return (BlackboardData)Activator.CreateInstance(NodeEditorSingleton.Instance.GraphBlackboard[t]);
            }
            return null;
        }
        public static object CreateNodeViewFromNodeType<T>() where  T:NodeData,new(){
            //Check specific derived type exists or not.
            var type = typeof(NodeView<T>);
            if (NodeEditorSingleton.Instance.FromGenericToSpecific.ContainsKey(type)){
                var implementedType = NodeEditorSingleton.Instance.FromGenericToSpecific[type];
                var instance = (NodeView<T>)Activator.CreateInstance(implementedType);
                return instance;
            }
            else{
                return new DefaultNodeView();
            }
            
        }
        public static object CreateNodeViewFromNodeType(Type t){
            //Check the generic type of NodeView by t
            var type = typeof(NodeView<>).MakeGenericType(t);
            if (NodeEditorSingleton.Instance.FromGenericToSpecific.ContainsKey(type)){
                var implementedType = NodeEditorSingleton.Instance.FromGenericToSpecific[type];
                var instance = Activator.CreateInstance(implementedType);
                return instance;
            }
            else{
                return new DefaultNodeView();
            }
            
        }
    }
}