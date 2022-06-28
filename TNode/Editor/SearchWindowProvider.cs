using System;
using System.Collections.Generic;
using System.Drawing;
using TNode.BaseViews;
using TNode.Cache;
using TNode.Models;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNode.Editor{
    public class SearchWindowProvider:ScriptableObject,ISearchWindowProvider{
        private Type _graphType;
        private GraphView _graphView;
        private EditorWindow _editor;
        public void Setup(Type graph,GraphView graphView,EditorWindow editor){
            _graphType = graph;
            _graphView = graphView;
            _editor = editor;
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
            var relativePos = context.screenMousePosition - _editor.position.position;
            var localPos = _graphView.WorldToLocal(relativePos);
            if (userData is Type type){
                if (NodeEditorExtensions.CreateNodeViewFromNodeType(type) is GraphElement nodeView){
                    nodeView.SetPosition(
                        new Rect(localPos.x, localPos.y, nodeView.layout.width, nodeView.layout.height));
                    _graphView.AddElement(nodeView);
                }
                return true;
            }
            return false;
        }
        
        public SearchWindowProvider(){
        }
    }
}