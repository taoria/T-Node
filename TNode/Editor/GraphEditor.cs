using Codice.CM.Common;
using TNode.BaseViews;
using TNode.Tools;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace TNode.Editor{
    
    public abstract class GraphEditor<T> : EditorWindow where T:GraphData{ 
        protected DataGraphView<T> _graphView;
        [FormerlySerializedAs("m_VisualTreeAsset")] [SerializeField]
        private VisualTreeAsset mVisualTreeAsset = default;
    
        public void CreateGUI()
        {
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
            _graphView = NodeEditorExtensions.CreateInstance<DataGraphView<T>>();
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
                    //Create a new asset file with type of T
                    T asset = ScriptableObject.CreateInstance<T>();
                    AssetDatabase.CreateAsset(asset, path);
                }
            }
  
        }
        
        protected virtual void OnCreate(){
            
        }
    }
}
