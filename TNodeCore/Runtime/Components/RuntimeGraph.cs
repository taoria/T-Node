using System;
using System.Collections.Generic;
using System.Linq;
using TNode.TNodeCore.Runtime.Tools;
using TNodeCore.Runtime.Models;
using TNodeCore.Runtime.RuntimeModels;
using UnityEditor;
using UnityEngine;

namespace TNodeCore.Runtime.Components{
    public class RuntimeGraph:MonoBehaviour,IRuntimeNodeGraph{
        /// <summary>
        /// Graph data reference to be used in runtime
        /// </summary>
        public GraphData graphData;
        /// <summary>
        /// Runtime copy of scene node data to hold references to scene objects
        /// </summary>
        
        /// <summary>
        /// Map of node id to runtime node
        /// </summary>
        [NonSerialized]
        public readonly Dictionary<string, RuntimeNode> RuntimeNodes = new Dictionary<string, RuntimeNode>();
        ///<summary>
        /// The graph tool the current runtime graph is using
        /// </summary>
        [NonSerialized]
        private GraphTool _graphTool;
        /// <summary>
        /// Inner graph tool to help with graph operations
        /// </summary>
        
        /// <summary>
        /// Holding the reference of the blackboard ,but it will be override by the runtime graph
        /// </summary>
        [SerializeReference]
        public BlackboardData runtimeBlackboardData;
        /// <summary>
        /// Check if the runtime graph is build .a built graph has a graph tool set up
        /// </summary>
        [NonSerialized]
        private bool _build = false;

        [NonSerialized] private IEnumerator<RuntimeNode> _runtimeNodeEnumerator;

        /// <summary>
        /// Build the graph tool and other dependencies for the runtime graph
        /// </summary>
 
        public virtual void Build(){
            if (_build) return;
            
            var link = graphData.NodeLinks;
            //iterate links and create runtime nodes
            foreach (var linkData in link){
                ModifyOrCreateInNode(linkData);
                ModifyOrCreateOutNode(linkData);
            }
            //iterate nodes and create runtime nodes
            foreach (var nodeData in graphData.NodeDictionary.Values){
                CreateRuntimeNodeIfNone(nodeData);
            }
            var nodeList = RuntimeNodes.Values;
            _graphTool = new GraphTool(this);
            var sceneNodes = RuntimeNodes.Values.Where(x => x.NodeData is SceneNode).Select(x => x.NodeData as SceneNode);
            foreach (var sceneNode in sceneNodes){
                if (sceneNode != null) sceneNode.BlackboardData = runtimeBlackboardData;
            }
#if UNITY_EDITOR
            BuildSceneNode();
#endif
            _runtimeNodeEnumerator = _graphTool.BreathFirstSearch();
            _build = true;
        }

        private void CreateRuntimeNodeIfNone(NodeData nodeData){
            if (RuntimeNodes.ContainsKey(nodeData.id)) return;
            var runtimeNode = new RuntimeNode(nodeData);
            RuntimeNodes.Add(nodeData.id,runtimeNode);
        }

        /// <summary>
        /// Cast the node data to a runtime node
        /// </summary>
        /// <param name="nodeData">Node data you provided</param>
        /// <returns></returns>
        //DFS search to run a node.
        public bool RunOnDependency(NodeData startNode){
            if(!_build)
                Build();
            if (_graphTool == null)
                return false;
            _graphTool.RunNodeDependently(GetRuntimeNode(startNode));
            return true;
        }
        public bool TraverseAll(){
            if(!_build)
                Build();
            if (_graphTool == null)
                return false;
            _graphTool.DirectlyTraversal();
            return true;
        }
        #region build scene node data
        #if UNITY_EDITOR
        public void BuildSceneNodePersistentData(SceneNode sceneNode){
            var persistentData = transform.Find("PersistentData").GetComponent<SceneDataPersistent>();
            persistentData.SceneNodeDataDictionary.Add(sceneNode.id,sceneNode);
        }

        public SceneDataPersistent CreateSceneNodePersistentGameObject(){
            var go = new GameObject("PersistentData");
            go.transform.SetParent(transform);
            return go.AddComponent<SceneDataPersistent>();
        }

        public void BuildSceneNode(){
            var fetchedSceneNode = graphData.NodeDictionary.Values.Where(x => x is SceneNode && x is  BlackboardDragNode == false).ToArray();
            if (!fetchedSceneNode.Any()) return;
            var scenePersistent = transform.Find("PersistentData")?.GetComponent<SceneDataPersistent>();
            
            if(scenePersistent == null){
               scenePersistent =  CreateSceneNodePersistentGameObject();
                
            }
            foreach (var nodeData in fetchedSceneNode){
                if (scenePersistent.SceneNodeDataDictionary.ContainsKey(nodeData.id)){
                    var sceneNodeData = scenePersistent.SceneNodeDataDictionary[nodeData.id];
                    RuntimeNodes[nodeData.id].NodeData = sceneNodeData;
                }
                else if (nodeData.Clone() is SceneNode clonedNodeData){
                    clonedNodeData.BlackboardData = runtimeBlackboardData;
                    RuntimeNodes.Remove(nodeData.id);
                    RuntimeNodes.Add(nodeData.id,new RuntimeNode(clonedNodeData));
                    BuildSceneNodePersistentData(clonedNodeData);
                }
            }
            UpdatePersistentData();
        }

        private void UpdatePersistentData(){
            var persistentData = transform.Find("PersistentData")?.GetComponent<SceneDataPersistent>();
            if (persistentData == null) return;
            var fetchedSceneNode = 
                RuntimeNodes
                    .Where(x => x.Value.NodeData is SceneNode  & x.Value.NodeData is BlackboardDragNode == false)
                    .Select(x=>x.Value.NodeData).ToArray();
 
            var dic = persistentData.SceneNodeDataDictionary;
            foreach (var sceneNodeData in dic.Values){
                if(!fetchedSceneNode.Contains(sceneNodeData)){
                    persistentData.SceneNodeDataDictionary.Remove(sceneNodeData.id);
                }
            }
        }
        #endif
        #endregion

        private void ModifyOrCreateInNode(NodeLink linkData){
            var inNodeId = linkData.inPort.nodeDataId;
            var inNode = graphData.NodeDictionary[inNodeId];
            if (!RuntimeNodes.ContainsKey(inNode.id)){
                var runtimeInNode = new RuntimeNode(inNode);
                RuntimeNodes.Add(inNode.id,runtimeInNode);
            }
            RuntimeNodes[inNode.id].InputLinks.Add(linkData);
        }
        public List<RuntimeNode> GetRuntimeNodesOfType<T>(){
            return RuntimeNodes.Values.Where(x => typeof(T).IsAssignableFrom(x.NodeType)).ToList();
        }

        public void ResetState(){
            _runtimeNodeEnumerator = _graphTool.BreathFirstSearch();
        }

        public NodeData CurrentNode(){
            return CurrentRuntimeNode().NodeData;
        }

        public RuntimeNode MoveNext(){
            _runtimeNodeEnumerator.MoveNext();
            return _runtimeNodeEnumerator.Current;
        }

        public RuntimeNode CurrentRuntimeNode(){
            if (_runtimeNodeEnumerator.Current == null){
                _runtimeNodeEnumerator.MoveNext();
            }
            return _runtimeNodeEnumerator.Current;
        }

        public  List<RuntimeNode> GetRuntimeNodesOfType(Type type){
            return RuntimeNodes.Values.Where(x => type.IsAssignableFrom(type)).ToList();
        }
        public void RunNodesOfType(Type t,bool isCaching= false){
            var nodes = GetRuntimeNodesOfType(t);
            if(isCaching)
                _graphTool.StartCachingPort();
            foreach (var runtimeNode in nodes){
                RunOnDependency(runtimeNode.NodeData);
            }
            if(isCaching)
                _graphTool.EndCachingPort();
        }
        public void RunNodesOfType<T>(bool isCaching= false){
            var nodes = GetRuntimeNodesOfType<T>();
            if(isCaching)
                _graphTool.StartCachingPort();
            foreach (var runtimeNode in nodes){
                RunOnDependency(runtimeNode.NodeData);
            }
            if(isCaching)
                _graphTool.EndCachingPort();
        }

        /// <summary>
        /// Run some nodes ,if the node is not in the graph ,then pass
        /// </summary>
        /// <param name="runtimeNodes"></param>
        /// <param name="isCaching"></param>
        public void RunNodes(List<RuntimeNode> runtimeNodes,bool isCaching= false){
            if (isCaching){
                _graphTool.StartCachingPort();
            }
            foreach (var runtimeNode in runtimeNodes){
                if(!RuntimeNodes.ContainsKey(runtimeNode.NodeData.id)){
                    continue;
                }
                RunOnDependency(runtimeNode.NodeData);
            }
            if (isCaching){
                _graphTool.EndCachingPort();
            }
        }
        private void ModifyOrCreateOutNode(NodeLink linkData){
            var outNodeId = linkData.outPort.nodeDataId;
            var outNode = graphData.NodeDictionary[outNodeId];
            if(!RuntimeNodes.ContainsKey(outNode.id)){
                var runtimeOutNode = new RuntimeNode(outNode);
                RuntimeNodes.Add(outNode.id,runtimeOutNode);
            }
            RuntimeNodes[outNode.id].OutputLinks.Add(linkData);
        }
        
        public void OnValidate(){
            if(runtimeBlackboardData==null||runtimeBlackboardData.GetType()==typeof(BlackboardData)){
                if (graphData != null)
                    runtimeBlackboardData = graphData.blackboardData?.Clone() as BlackboardData;
            }
        }

        public void OnDisable(){
            RuntimeNodes.Clear();
            _build = false;
        }

        public void OnDestroy(){
            RuntimeNodes.Clear();
            _build = false;
        }

        public void Start(){
            Build();
          
        }
        public virtual void RuntimeExecute(){
            _graphTool.DirectlyTraversal();
        }

        public RuntimeNode GetRuntimeNode(NodeData nodeData){
            if(!_build)
                Build();
            if(RuntimeNodes.ContainsKey(nodeData.id)){
                return RuntimeNodes[nodeData.id];
            }
            return null;
        }

        public RuntimeNode GetRuntimeNode(string id){
            if(!_build)
                Build();
            if(RuntimeNodes.ContainsKey(id)){
                return RuntimeNodes[id];
            }
            return null;
        }

        public BlackboardData GetBlackboardData(){
            return runtimeBlackboardData;
        }

        public List<RuntimeNode> GetRuntimeNodes(){
            return RuntimeNodes.Values.ToList();
        }

        public Dictionary<string, RuntimeNode> GetRuntimeNodesDictionary(){
            return RuntimeNodes;
        }


        public NodeData GetNode(string id){
            if(!_build)
                Build();
            if(RuntimeNodes.ContainsKey(id)){
                return RuntimeNodes[id].NodeData;
            }
            return null;
        }
    }

    public class SceneDataPersistent:MonoBehaviour,ISerializationCallbackReceiver{
        [NonSerialized]
        
        public readonly Dictionary<string,SceneNode> SceneNodeDataDictionary = new Dictionary<string,SceneNode>();
        
        [SerializeReference]
        public List<SceneNode> sceneNodeData=new List<SceneNode>();


        public void OnBeforeSerialize(){
      
            sceneNodeData.Clear();
            foreach(var node in SceneNodeDataDictionary.Values){
                sceneNodeData.Add(node);
            }
        }
        public void OnAfterDeserialize(){
            SceneNodeDataDictionary.Clear();
            foreach(var node in sceneNodeData){
                SceneNodeDataDictionary.Add(node.id,node);
            }
        }
    }
    
    public enum ProcessingStrategy{
        BreadthFirst,
        DepthFirst
    }
}