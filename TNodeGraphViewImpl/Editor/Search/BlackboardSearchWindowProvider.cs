using System;
using System.Collections;
using System.Collections.Generic;
using TNode.Editor.NodeGraphView;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace TNode.Editor.Search{
    public class BlackboardSearchWindowProvider:ScriptableObject,ISearchWindowProvider{
        private Type _graphType;
        private IBaseDataGraphView _graphView;
        private EditorWindow _editor;

        private struct InternalSearchTreeUserData{
            public IList List;
            public Type Type;

        }
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context){
            var blackboardData = _graphView.GetBlackboardData();
            var type = blackboardData.GetType();
            var list = new List<SearchTreeEntry>(){
                new SearchTreeGroupEntry(new GUIContent("Add New Blackboard Data"), 0),
            };
                
            if (list == null) throw new ArgumentNullException(nameof(list));
            //search fields with List type
            Texture2D icon = new Texture2D(2,2);
  
            foreach (var field in type.GetFields()){
                if (field.FieldType.IsGenericType){
                    var genericType = field.FieldType.GetGenericTypeDefinition();
                    if (genericType == typeof(List<>)){
                        list.Add(new SearchTreeEntry(new GUIContent(field.Name,icon)){
                            level = 1,
                            userData = new InternalSearchTreeUserData(){
                                List = field.GetValue(blackboardData) as IList,
                                Type = field.FieldType.GetGenericArguments()[0]
                            }
                            
                        });
                    }
                }
            }
            Debug.Log($"{list.Count}");
            return list;

        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context){
            var userData = SearchTreeEntry.userData;
            if (userData is InternalSearchTreeUserData){
                var list = ((InternalSearchTreeUserData) userData).List;
                var type = ((InternalSearchTreeUserData) userData).Type;
                var newItem = Activator.CreateInstance(type);
                list?.Add(newItem);
                return true;
            }
            return false;
        }

        public void Setup(Type graph,IBaseDataGraphView graphView,EditorWindow editor){
            _graphType = graph;
            _graphView = graphView;
            _editor = editor;
        }
    }
    
}