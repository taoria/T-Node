using System;
using System.Collections.Generic;
using TNode.BaseViews;
using TNode.Cache;
using TNode.Models;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNode.Editor{
    public class SearchWindowProvider:ScriptableObject,ISearchWindowProvider{
        private Type _graphType;
        private GraphView _graphView;
        public void Setup(Type graph,GraphView graphView){
            _graphType = graph;
            _graphView = graphView;
        }
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context){
            var nodeDataTypes = NodeEditorExtensions.GetGraphDataUsage(_graphType);
            
            var list = new List<SearchTreeEntry>{
                new SearchTreeGroupEntry(new GUIContent("Add New Node"), 0),
            };
            //TODO a node icon shall be given by some way
            Texture2D icon = new Texture2D(2,2);
            foreach (var nodeDataType in nodeDataTypes){
                Debug.Log(nodeDataType.Name);

                
                list.Add(new SearchTreeEntry(new GUIContent($" {nodeDataType.Name} ",icon)){
                    level =  1,
                    userData = nodeDataType,
                });
            }
            return list;
        }
        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context){
            var userData = SearchTreeEntry.userData;
            if (userData is Type type){
                var nodeView = NodeEditorExtensions.CreateNodeViewFromNodeType(type) as GraphElement;
                _graphView.AddElement(nodeView);
                return true;
            }
            return false;
        }
        
        public SearchWindowProvider(){
        }
    }
}