﻿using System;
using System.Collections.Generic;
using System.Linq;
using TNodeCore.Editor.NodeGraphView;
using TNodeCore.Editor.Tools.NodeCreator;
using TNodeCore.Runtime.Models;
using TNodeGraphViewImpl.Editor.Cache;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNodeGraphViewImpl.Editor.Search{
    public class NodeSearchWindowProvider:ScriptableObject,ISearchWindowProvider{
        private Type _graphType;
        private GraphView _graphView;
        private EditorWindow _editor;
        private Vector2 _createLocation;

        public void Setup(Type graph,GraphView graphView,EditorWindow editor,Vector2 createLocation=default){
            _graphType = graph;
            _graphView = graphView;
            _editor = editor;
            _createLocation = createLocation;
        }
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context){
            var nodeDataTypes = NodeEditorExtensions.GetGraphDataUsage(_graphType);
            var categories = NodeEditorExtensions.GetGraphCategories(_graphType);
            
            var list = new List<SearchTreeEntry>{
               
            };
            var root = new SearchTreeGroupEntry(new GUIContent("CreateProp"),0);
            list.Add(root);
            Texture2D icon = new Texture2D(2,2);
            foreach (var category in categories){
                var categoryEntry = new SearchTreeGroupEntry(new GUIContent(category),1);
                list.Add(categoryEntry);
                nodeDataTypes.Where(x => NodeEditorExtensions.GetTypeCategory(x).Equals(category)).ToList().ForEach(x => {
                    var nodeDataType = x;
                    var nodeDataTypeEntry = new SearchTreeEntry(new GUIContent($" {nodeDataType.Name} ",icon)){
                        level = 2,
                        userData = nodeDataType
                    };
                    list.Add(nodeDataTypeEntry);
                });

            }
            return list;
        }

        public Vector2 FromScreenToViewPos(VisualElement visualElement,Vector2 screenPos){
            var vector = visualElement.worldTransform.inverse.MultiplyVector(screenPos);
            return vector;
        }
        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context){
            var userData = SearchTreeEntry.userData;
 


            if (userData is Type type){
                //Check if type is derived from NodeData
                if (typeof(NodeData).IsAssignableFrom(type)){
                    //Make an instance of the type
                    if (NodeCreator.InstantiateNodeData(type) is { } nodeData){
                        nodeData.nodeName = $"{type.Name}";
                        ((IBaseDataGraphView) _graphView).AddTNode(nodeData, new Rect(_createLocation.x, _createLocation.y, 100, 100));
                    }
                }
                return true;
            }
            return false;
        }

        private void UndoCreateNodePerformed(){
            
        }

   
    }
}