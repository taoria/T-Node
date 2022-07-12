using Codice.CM.Common;
using TNode.BaseViews;
using TNode.Cache;
using TNode.Editor.BaseViews;
using TNode.Editor.Inspector;
using TNode.Editor.Model;
using TNode.Models;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace TNode.Editor{
    
    
    public abstract class GraphEditor<T> : EditorWindow where T:GraphData{ 
        protected DataGraphView<T> _graphView;
        [SerializeField]
        private VisualTreeAsset mVisualTreeAsset = default;
        //Persist editor data ,such as node position,node size ,etc ,in this script object
        [FormerlySerializedAs("nodeEditorData")] public GraphEditorData graphEditorData;
    
        public void CreateGUI(){
            
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            
            // Instantiate UXML
            VisualElement labelFromUXML = mVisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);
       
            BuildGraphView();
            DefineGraphEditorActions();
            OnCreate();
        }
        
        private void BuildGraphView(){
            _graphView = NodeEditorExtensions.CreateViewComponentFromBaseType<DataGraphView<T>>();
            rootVisualElement.Add(_graphView);
            _graphView.StretchToParentSize();
        }

        private void DefineGraphEditorActions(){
            //Register a  event when user press ctrl + s
            rootVisualElement.RegisterCallback<KeyUpEvent>((evt) => {
                if (evt.keyCode == KeyCode.S && evt.ctrlKey)
                {
                    Save();
                }
            });
            
        }

        private void Save(){
            //if no graph is loaded ,create a file save dialogue
            if (_graphView.Data == null)
            {
                string path = EditorUtility.SaveFilePanel("Save Graph", "", "", "asset");
                if (path.Length != 0){
                    //Create a new asset file with type of GraphDataType
                    T asset = ScriptableObject.CreateInstance<T>();
                    AssetDatabase.CreateAsset(asset, path);
                    AssetDatabase.SaveAssets();
                    
                }
            }
            else{
                _graphView.SaveWithEditorData(graphEditorData);
                AssetDatabase.Refresh();
                
            }
  
        }
        
        protected virtual void OnCreate(){
            
        }
  
    }
}
