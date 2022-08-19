using TNodeCore.Editor.EditorPersistence;
using TNodeCore.Editor.NodeGraphView;
using TNodeCore.Runtime.Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace TNodeCore.Editor{
    
    // public class SelectGraphWindow : EditorWindow{
    //     public EditorWindow parent;
    //     public Type graphType;
    //     public static void ShowWindow<type> (GraphEditor<type> parent) where type:GraphData{
    //         var window = GetWindow<SelectGraphWindow>();
    //         window.graphType = typeof(type);
    //         window.Show();
    //         window.parent = parent;
    //     }
    //     private void OnGUI(){
    //             
    //         if(GUILayout.Button("CreateProp An Graph")){
    //             //Add a save file dialog to save the graph
    //             //CreateProp the graph
    //             var graphAsset = ScriptableObject.CreateInstance(graphType);
    //             var path = EditorUtility.SaveFilePanel("Save Graph", "", "", "asset");
    //             //Save the graph
    //             AssetDatabase.CreateAsset(graphAsset, path); 
    //             AssetDatabase.SaveAssets();
    //             AssetDatabase.Refresh();
    //             //Load the graph
    //             var graph = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path) as GraphData;
    //             var graphEditor = parent as IGraphEditor;
    //             if (graphEditor.GetGraphView() != null){
    //                 graphEditor.GetGraphView().SetGraphData(graph);
    //                 Debug.Log(graph);
    //             }
    //         }
    //         //Drag and drop a graph asset to load it
    //         if(Event.current.type == EventType.DragUpdated){
    //             DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
    //             Event.current.Use();
    //         }
    //     }
    // }
    public abstract class GraphEditor<T> : EditorWindow,IGraphEditor where T:GraphData{ 
        protected IDataGraphView<T> GraphView;
        [SerializeField]
        private VisualTreeAsset mVisualTreeAsset = default;
        //Persist editor data ,such as node position,node size ,etc ,in this script object
        [FormerlySerializedAs("nodeEditorData")] public GraphEditorData graphEditorData;
        private bool _windowShowed=false;

        public void CreateGUI(){
            
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            VisualElement labelFromUxml = null;
            if(mVisualTreeAsset==null){
                Debug.LogWarning("No visual tree asset found ,enable the default visual tree asset ");
                 labelFromUxml = Resources.Load<VisualTreeAsset>("GraphEditor").Instantiate();
               
            }
            else{
                labelFromUxml = mVisualTreeAsset.Instantiate();
            }
            // Instantiate UXML
         
   
            root.Add(labelFromUxml);
            
            BuildGraphView();
            DefineGraphEditorActions();
            GraphView.Owner = this;
            OnCreate();
        }
  
        public void Update(){
            if (GraphView == null) return;
            if (GraphView.Data != null) return;
            if (_windowShowed==false){
                _windowShowed = true;
            }
        }


        public void SetupNonRuntime(T graphData){
            GraphView.Data = graphData;
            GraphView.IsRuntimeGraph = false;
        }
        private void BuildGraphView(){
           
            GraphView = graphEditorData.GetGraphView<T>();
            GraphView.Owner = this;
       
            rootVisualElement.Add((VisualElement)GraphView);

            GraphView.AfterEditorLoadGraphView();
            ((VisualElement)GraphView).StretchToParentSize();
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
            if (GraphView.Data == null)
            {
                string path = EditorUtility.SaveFilePanel("Save Graph", "", "", "asset");
                if (path.Length != 0){
                    //CreateProp a new asset file with type of GraphDataType
                    T asset = ScriptableObject.CreateInstance<T>();
                    AssetDatabase.CreateAsset(asset, path);
                    AssetDatabase.SaveAssets();
                    
                }
            }
            else{
                GraphView.SaveWithEditorData(graphEditorData);
                AssetDatabase.Refresh();
                
            }
  
        }
        
        protected virtual void OnCreate(){
            
        }

        public void SetGraphView(IBaseDataGraphView graphView){
            GraphView = (IDataGraphView<T>)graphView;
        }

        public IBaseDataGraphView GetGraphView(){
            return GraphView;
        }
    }
}
