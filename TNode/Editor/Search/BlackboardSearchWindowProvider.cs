using System;
using System.Collections;
using System.Collections.Generic;
using TNode.Editor.BaseViews;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace TNode.Editor{
    public class BlackboardSearchWindowProvider:ISearchWindowProvider{
        private Type _graphType;
        private IDataGraphView _graphView;
        private EditorWindow _editor;

        private struct InternalSearchTreeUserData{
            public IList List;
            public Type Type;

        }
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context){
            var blackboardData = _graphView.GetBlackboardData();
            var type = blackboardData.GetType();
            var entries = new List<SearchTreeEntry>();
            if (entries == null) throw new ArgumentNullException(nameof(entries));
            //search fields with List type
            Texture2D icon = new Texture2D(2,2);
            foreach (var field in type.GetFields()){
                if (field.FieldType.IsGenericType){
                    var genericType = field.FieldType.GetGenericTypeDefinition();
                    if (genericType == typeof(List<>)){
                        entries.Add(new SearchTreeEntry(new GUIContent(field.Name,icon)){
                            level = 1,
                            userData = new InternalSearchTreeUserData(){
                                List = field.GetValue(blackboardData) as IList,
                                Type = field.FieldType.GetGenericArguments()[0]
                            }
                            
                        });
                    }
                }
            }

            return entries;

        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context){
            var userData = SearchTreeEntry.userData;
            var relativePos = context.screenMousePosition - _editor.position.position;
            var blackboardData  = _graphView.GetBlackboardData();
            
            if (userData is InternalSearchTreeUserData){
                var list = ((InternalSearchTreeUserData) userData).List;
                var type = ((InternalSearchTreeUserData) userData).Type;
                var newItem = Activator.CreateInstance(type);
                list.Add(newItem);
                return true;
            }

            return false;
        }

        public void Setup(Type graph,IDataGraphView graphView,EditorWindow editor){
            _graphType = graph;
            _graphView = graphView;
            _editor = editor;
        }
    }
    
}