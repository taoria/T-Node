using System.IO;
using System.Text.RegularExpressions;
using TNode.Editor.Model;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

//add an attribute right click asset panel and select "TNode/Create/Create New Graph Editor" to call this editor


namespace TNode.Editor.Tools.GraphEditorCreator{
    
    public class GraphEditorCreator : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        private TextField _editorClassNameTextField;
        private TextField _graphClassNameTextField;
        private Button _createButton;
        private readonly SourceGeneratorForGraphEditor _sourceGeneratorForGraphEditor = new SourceGeneratorForGraphEditor();
        [MenuItem("Assets/Create/TNode/Create New Graph Editor")]
        [MenuItem("TNode/Create New Graph Editor")]
        public static void ShowExample()
        {
            GraphEditorCreator wnd = GetWindow<GraphEditorCreator>();
            wnd.titleContent = new GUIContent("GraphEditorCreator");
            //Set position to the center of the screen
            wnd.position = new(Screen.width / 2, Screen.height / 2, 500, 300);
            //set this window non resizable
            wnd.minSize = new Vector2(500, 300);
            wnd.maxSize = new Vector2(500, 300);
            
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // VisualElements objects can contain other VisualElement following a tree hierarchy.
            VisualElement label = new Label("Hello World! From C#");
            root.Add(label);

            // Instantiate UXML
            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);
            
            //Register a callback when Create Button is clicked
            _createButton = root.Q<Button>("CreateButton");
            _createButton.clickable.clicked += OnCreateButtonClicked;

            _editorClassNameTextField = root.Q<TextField>("EditorClassNameTextField");
            _graphClassNameTextField = root.Q<TextField>("GraphClassNameTextField");
            
            _editorClassNameTextField.RegisterCallback<ChangeEvent<string>>((evt) => {
                CheckIfTextValid();
            });
            _graphClassNameTextField.RegisterCallback<ChangeEvent<string>>((evt) => {
                CheckIfTextValid();
            });
            
            CheckIfTextValid();

        }

        public void CheckIfTextValid(){
            Regex regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9_]+$");
            var matchEditor = regex.IsMatch(_editorClassNameTextField.value);
            var matchGraph = regex.IsMatch(_graphClassNameTextField.value);
            if (matchEditor){
                //Set background color to green
                _editorClassNameTextField.style.backgroundColor = new Color(0.5f, 1f, 0.5f);
            }
            else{
                //Set background color to red
                _editorClassNameTextField.style.backgroundColor = new Color(1f, 0.5f, 0.5f);
            }
            if (matchGraph){
                //Set background color to green
                _graphClassNameTextField.style.backgroundColor = new Color(0.5f, 1f, 0.5f);
            }
            else{
                //Set background color to red
                _graphClassNameTextField.style.backgroundColor = new Color(1f, 0.5f, 0.5f);
            }
            _createButton.SetEnabled(matchGraph && matchEditor);
         
            
        }
        
        private void OnCreateButtonClicked(){
            //Create a new .cs file at current opened asset folder
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == ""){
                path = "Assets";
               
            }
            else if (Path.GetExtension(path) != ""){
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
              
            }
            var pathBeforeEditor = path;
            //if the path is not named with editor, create a new folder called editor at the path
            if (!path.EndsWith("/Editor"))
            {
                AssetDatabase.CreateFolder(path, "Editor");
                pathBeforeEditor = path;
                path += "/Editor";
            }
            //Query the name of the graph editor
            string editorName =_editorClassNameTextField.text;
            string graphName = _graphClassNameTextField.text;
            string graphViewName = graphName+"View";
            if (editorName == "")
            {
                editorName = "NewGraphEditor";
            }
            
            CreateResource(editorName, graphName, graphViewName, path, pathBeforeEditor);
        }

        private void CreateResource(string editorName, string graphName, string graphViewName, string path,
            string pathBeforeEditor){
            var source = _sourceGeneratorForGraphEditor.GenerateGraphEditor(editorName, graphName);

            var sourceGraph = _sourceGeneratorForGraphEditor.GenerateGraph(graphName);
            var sourceGraphView = _sourceGeneratorForGraphEditor.GenerateGraphView(graphViewName, graphName);
            string editorPath = Path.Combine(path, editorName + ".cs");
            string graphPath = Path.Combine(pathBeforeEditor, graphName + ".cs");
            string graphViewPath = Path.Combine(path, graphViewName + ".cs");
            File.WriteAllText(editorPath, source);
            File.WriteAllText(graphPath, sourceGraph);
            File.WriteAllText(graphViewPath, sourceGraphView);
            //Refresh the AssetDatabase to import the new file
            AssetDatabase.Refresh();

            //Wait for the new file to be imported
            while (!AssetDatabase.LoadAssetAtPath<MonoScript>(editorPath)){
                EditorUtility.DisplayProgressBar("Generating Graph Editor",
                    "Please wait while the new graph editor is being imported", 0.5f);
                EditorApplication.update();
            }

            //Create an NodeAttribute Editor Data Instance for the new graph editor
            NodeEditorData nodeEditorData = ScriptableObject.CreateInstance<NodeEditorData>();
            nodeEditorData.name = editorName;

            VisualTreeAsset defaultEditorTree = Resources.Load<VisualTreeAsset>("GraphEditor");
            EditorUtility.SetDirty(nodeEditorData);

            //Save it at the same folder as the new graph editor
            string nodeEditorDataPath = Path.Combine(path, editorName + ".asset");
            AssetDatabase.CreateAsset(nodeEditorData, nodeEditorDataPath);
            //Wait for the new file to be imported
            while (!AssetDatabase.LoadAssetAtPath<NodeEditorData>(nodeEditorDataPath)){
                EditorUtility.DisplayProgressBar("Generating Graph Editor",
                    "Please wait while the new graph editor is being imported", 0.5f);
                EditorApplication.update();
            }

            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(editorPath);
            //Set the mono importer to the current graph editor script
            MonoImporter monoImporter = AssetImporter.GetAtPath(editorPath) as MonoImporter;

            if (monoImporter != null)
                monoImporter.SetDefaultReferences(new string[]{"nodeEditorData", "mVisualTreeAsset"},
                    new Object[]{nodeEditorData, defaultEditorTree});


            //Refresh the asset ann close it
            //Mark it dirty

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Close();
        }
    }
}
