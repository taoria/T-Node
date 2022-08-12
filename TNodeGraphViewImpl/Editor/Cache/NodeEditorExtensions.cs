using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TNode.TNodeCore.Editor.Blackboard;
using TNode.TNodeCore.Editor.EditorPersistence;
using TNodeCore.Editor.NodeGraphView;
using TNodeCore.Runtime.Attributes;
using TNodeCore.Runtime.Models;
using TNodeGraphViewImpl.Editor.GraphBlackboard;
using TNodeGraphViewImpl.Editor.NodeGraphView;
using TNodeGraphViewImpl.Editor.NodeViews;
using UnityEditor;
using UnityEngine;

namespace TNodeGraphViewImpl.Editor.Cache{
    /// <summary>
    /// Internal singleton class for caching TNodeCore reflection Data.
    /// </summary>
    internal class NodeEditorTypeDictionary:Dictionary<Type, Type>{
        private class NodeEditorTypeDictionaryComparer : IEqualityComparer<Type>{
            public bool Equals(Type x, Type y){
                return x?.ToString() == y?.ToString();
            }

            public int GetHashCode(Type obj){
                return obj.ToString().GetHashCode();
            }
        }
        public NodeEditorTypeDictionary():base(new NodeEditorTypeDictionaryComparer()){
            
        }
    }
    
    internal class NodeEditorSingleton{
        private static NodeEditorSingleton _instance;
        public readonly Dictionary<Type,Type> FromGenericToSpecific = new NodeEditorTypeDictionary();
        public readonly Dictionary<Type, List<Type>> GraphDataUsage = new Dictionary<Type, List<Type>>();
        public Dictionary<Type, Type> GraphBlackboard = new Dictionary<Type, Type> ();
        public static NodeEditorSingleton Instance{
            get{ return _instance ??= new NodeEditorSingleton(); }
        }

        
        private static readonly string[] ExcludedAssemblies = new[]{
            "Microsoft", "UnityEngine","UnityEditor","mscorlib",
            "System","Mono","PlasticPipe","unityplastic","ExCSS",
            "Unity","PlayerBuildProgramLibrary","netstandard","log4net","Newtonsoft","Bee","nunit","PsdPlugin"
        };
        public static T CreateViewComponentFromBaseType<T>(){
            var implementedType = NodeEditorSingleton.Instance.FromGenericToSpecific[typeof(T)];
            var instance = (T)Activator.CreateInstance(implementedType);
            return instance;
        }
        public static object CreateViewComponentFromBaseType(Type t){
            if (NodeEditorSingleton.Instance.FromGenericToSpecific.ContainsKey(t)){
                var implementedType = NodeEditorSingleton.Instance.FromGenericToSpecific[t];
                var instance = Activator.CreateInstance(implementedType);
                return instance;
            }
            
            //check if t is a generic type node view
            if (t is{IsGenericType: true} && t.GetGenericTypeDefinition() == typeof(BaseNodeView<>)){
                var instance = Activator.CreateInstance(typeof(BaseNodeView<NodeData>));
                return instance;
            }
            return null;
        }

        private NodeEditorSingleton(){
            //exclude  unity ,system ,and microsoft types
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
 

            assemblies = assemblies.Where(x => !ExcludedAssemblies.Contains(x.FullName.Split('.',',',' ')[0])).ToArray();
            foreach (var ass in assemblies){
            }
            foreach(var assembly in assemblies){
                foreach(var type in assembly.GetTypes()){
                    if(type.IsClass && !type.IsAbstract){
                        //Register Node View And Graph View via its parent class
                        SetViewComponentAttribute(type);
                        //Register Node Data by GraphUsageAttribute.
                        SetGraphUsageAttribute(type);
                     
                    }
                }
            }
            GraphEditorData.GraphViewImplCreator+=GraphViewImplCreator;
        }

        private IBaseDataGraphView GraphViewImplCreator(Type arg){
            var genericType = typeof(BaseDataGraphView<>).MakeGenericType(arg);
            var instance = CreateViewComponentFromBaseType(genericType) as IBaseDataGraphView;
            if (instance == null){
                //fallback to default graph view
                instance = (IBaseDataGraphView) Activator.CreateInstance(genericType);
            }
            return instance;
        }
        private void SetGraphUsageAttribute(Type type){
      
            foreach (var attribute in type.GetCustomAttributes(typeof(GraphUsageAttribute), true)){
                var parent = type.BaseType;
                if (typeof(Model).IsAssignableFrom(type.BaseType)){
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
        private readonly Type[] _acceptedTypesForGenericToSpecific = new Type[]{typeof(BaseNodeView<>),typeof(BaseDataGraphView<>),typeof(GraphBlackboardView<>)};
        private readonly Type[] _defaultTypes = new []{typeof(DefaultBaseNodeView),typeof(DefaultGraphBlackboardView)};
        private void SetViewComponentAttribute(Type type){
            foreach (var attribute in type.GetCustomAttributes(typeof(ViewComponentAttribute), false)){
                //fetch this type 's parent class
                var parent = type.BaseType;
                //Check if this type is a generic type and is a generic type of BaseNodeView or BaseDataGraphView,
                //Two level generic definition is now supported by TNodeCore
                //Deeper nested generic definition is not supported by TNodeCore
                if (parent is{IsGenericType: true} && 
                    (_acceptedTypesForGenericToSpecific.Contains(parent.GetGenericTypeDefinition()) ||
                     (parent.GetGenericTypeDefinition().IsGenericType && _acceptedTypesForGenericToSpecific.Contains(parent.GetGenericTypeDefinition().GetGenericTypeDefinition()))
                     )
                    ){
                    //Get the generic type of this type
                    //Add this type to the dictionary
                    FromGenericToSpecific.Add(parent, type);
                }
                //TODO Note that a node component only applied to a specific type of editor,so ,same GraphView could behave differently in different editor.it's a todo feature.
            }
        }

        public void Initialize(){
            //Do nothing  indeed
        }
    }
    //Outer wrapper for the singleton class
    public static class NodeEditorExtensions{
        /// <summary>
        ///  by given a generic type T,return the implementation instance  of the generic type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>

        public static string GetTypeCategory(Type type){
            var category = type.GetCustomAttribute<GraphUsageAttribute>();
            return category?.Category ?? "";
        }
        
        /// <summary>
        /// by given a generic type t,return the implementation instance  of the generic type
        /// </summary>
        /// <returns></returns>

        public static IBlackboardView CreateBlackboardDataFromBlackboardDataType(Type t){
            var type = typeof(GraphBlackboardView<>).MakeGenericType(t);
            var res = NodeEditorSingleton.CreateViewComponentFromBaseType(type) as IBlackboardView;
            return res ?? new DefaultGraphBlackboardView();
        }

        public static IBlackboardView CreateBlackboardWithGraphData(GraphData graphData){
            var graphType = graphData.GetType();
            if (NodeEditorSingleton.Instance.GraphBlackboard.ContainsKey(graphType)){
                var type = NodeEditorSingleton.Instance.GraphBlackboard[graphType];
                return CreateBlackboardDataFromBlackboardDataType(type);

            }
            return null;
        }
        public static IBlackboardView CreateBlackboardWithGraphData(Type graphType){
            if (NodeEditorSingleton.Instance.GraphBlackboard.ContainsKey(graphType)){
                var type = NodeEditorSingleton.Instance.GraphBlackboard[graphType];
                return CreateBlackboardDataFromBlackboardDataType(type);

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

        public static List<string> GetGraphCategories(Type t){
            if(!NodeEditorSingleton.Instance.GraphDataUsage.ContainsKey(t)){
                return new List<string>();
            }
            var list = NodeEditorSingleton.Instance.GraphDataUsage[t];
            //Merge same category
            var res = list.Select(x=>x.GetCustomAttribute<GraphUsageAttribute>().Category).Distinct().ToList();
            return res;
        }
        //TODO Move this method to runtime place
        public static BlackboardData GetAppropriateBlackboardData(Type t){
            if (NodeEditorSingleton.Instance.GraphBlackboard.ContainsKey(t)){
                return (BlackboardData)Activator.CreateInstance(NodeEditorSingleton.Instance.GraphBlackboard[t]);
            }
            return null;
        }
        public static object CreateNodeViewFromNodeType(Type t){
            //Check the generic type of BaseNodeView by t
           
            if (t.IsGenericType){
                //AKA if BlackboardDragNode<Camera> is pulled 
                //Get BlackboardDragNode<T> as generic type 
                
          
                var genericTypeDefinition = t.GetGenericTypeDefinition();
                
                //What you want is a BaseNodeView<BlackboardDragNode<T>> to be created
                var genericViewType = typeof(BaseNodeView<>).MakeGenericType(genericTypeDefinition);
             
                //search for the specific type of genericViewType in the dictionary
                if (NodeEditorSingleton.Instance.FromGenericToSpecific.ContainsKey(genericViewType)){
            
                    var implementedType = NodeEditorSingleton.Instance.FromGenericToSpecific[genericViewType];
                    //The implementedType is still a generic type ,so we make it a specific type by using MakeGenericType
                    //Get argument type of t
                    var argumentType = t.GetGenericArguments()[0];
                    var instance = Activator.CreateInstance(implementedType.MakeGenericType(argumentType));
                 
                    return instance;

                }
                else{
                    return new DefaultBaseNodeView();
                }

            }
            var type = typeof(BaseNodeView<>).MakeGenericType(t);
            if (NodeEditorSingleton.Instance.FromGenericToSpecific.ContainsKey(type)){
            
                var implementedType = NodeEditorSingleton.Instance.FromGenericToSpecific[type];
                var instance = Activator.CreateInstance(implementedType);
                return instance;
            }
            else{
                
                return new DefaultBaseNodeView();
            }
            
        }
    }
    [InitializeOnLoad]

    public class Launcher{
        static Launcher(){
            //Get version of the package
            Debug.Log("TNodeCore v0.01 is launched");
            NodeEditorSingleton.Instance.Initialize();
        }
    }
}