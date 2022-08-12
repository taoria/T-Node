using System;
using System.Collections;
using System.Collections.Generic;
using TNode.TNodeCore.Editor.Blackboard;
using TNodeCore.Editor.NodeGraphView;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TNodeGraphViewImpl.Editor.Search{
    public class BlackboardSearchWindowProvider:ScriptableObject,ISearchWindowProvider{
        private Type _graphType;
        private IBaseDataGraphView _graphView;
        private EditorWindow _editor;
        private IBlackboardView _blackboard;

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
                        var castedList = field.GetValue(blackboardData) as IList;
                        if (castedList == null){
                            field.SetValue(blackboardData, Activator.CreateInstance(field.FieldType));
                        }
                        list.Add(new SearchTreeEntry(new GUIContent(field.Name,icon)){
                            level = 1,
                            userData = new InternalSearchTreeUserData(){
                                List = field.GetValue(blackboardData) as IList,
                                Type = field.FieldType.GetGenericArguments()[0]
                            }
                          
                        });
                    }
                }

                if (field.FieldType.IsArray){
                    list.Add(new SearchTreeEntry(new GUIContent(field.Name,icon)){
                        level = 1,
                        userData = new InternalSearchTreeUserData(){
                            List = field.GetValue(blackboardData) as Array,
                            Type = field.FieldType.GetElementType()
                        }
                    });
                }
            }
            return list;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context){
            var userData = SearchTreeEntry.userData;
           
            if (userData is InternalSearchTreeUserData){
                var list = ((InternalSearchTreeUserData) userData).List;
                if (list == null){
                    
                }
                var type = ((InternalSearchTreeUserData) userData).Type;
                if (!typeof(Object).IsAssignableFrom(type)){
                    var newItem = Activator.CreateInstance(type);
                    list?.Add(newItem);
                }
                else{
                    var newItem = EditorUtility.InstanceIDToObject(EditorGUIUtility.GetObjectPickerControlID());
                    list?.Add(newItem);
                }
                _blackboard.NotifyUpdate();
                return true;
            }
            return false;
        }

        public void Setup(Type graph,IBaseDataGraphView graphView,EditorWindow editor,IBlackboardView blackboard){
            _graphType = graph;
            _graphView = graphView;
            _editor = editor;
            _blackboard = blackboard;
        }
    }
    
}