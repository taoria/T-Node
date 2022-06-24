using System.IO;
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
            Button createButton = root.Q<Button>("CreateButton");
            createButton.clickable.clicked += OnCreateButtonClicked;
            
        }

        private void OnCreateButtonClicked(){
            //Create a new .cs file at current opened asset folder
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }
            //if the path is not named with editor, create a new folder called editor at the path
            if (!path.EndsWith("Editor"))
            {
                AssetDatabase.CreateFolder(path, "Editor");
                path = path + "/Editor";
            }
            //Query the name of the graph editor
            string editorName = rootVisualElement.Q<TextField>("EditorClassNameTextField").text;
            string graphName = rootVisualElement.Q<TextField>("GraphClassNameTextField").text;
            if (editorName == "")
            {
                editorName = "NewGraphEditor";
            }
    
            SourceGeneratorForGraphEditor sourceGeneratorForGraphEditor = new SourceGeneratorForGraphEditor();
            
            var source = sourceGeneratorForGraphEditor.GenerateGraphEditor(editorName,graphName);

            var sourceGraph = sourceGeneratorForGraphEditor.GenerateGraph(graphName);
            string editorPath = Path.Combine(path, editorName + ".cs");
            string graphPath = Path.Combine(path, graphName + ".cs");
            File.WriteAllText(editorPath, source);

            File.WriteAllText(graphPath, sourceGraph);
            
           
            
            //Refresh the AssetDatabase to import the new file
            
            AssetDatabase.Refresh();
            
            //Wait for the new file to be imported
            while (!AssetDatabase.LoadAssetAtPath<MonoScript>(editorPath))
            {
                EditorUtility.DisplayProgressBar("Generating Graph Editor", "Please wait while the new graph editor is being imported", 0.5f);
                EditorApplication.update();
            }
            //Create an Node Editor Data Instance for the new graph editor
            NodeEditorData nodeEditorData = ScriptableObject.CreateInstance<NodeEditorData>();
            nodeEditorData.name = editorName;
            
            EditorUtility.SetDirty(nodeEditorData);
            
            //Save it at the same folder as the new graph editor
            string nodeEditorDataPath = Path.Combine(path, editorName + ".asset");
            AssetDatabase.CreateAsset(nodeEditorData, nodeEditorDataPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            //Wait for the new file to be imported
            while (!AssetDatabase.LoadAssetAtPath<NodeEditorData>(nodeEditorDataPath))
            {
                EditorUtility.DisplayProgressBar("Generating Graph Editor", "Please wait while the new graph editor is being imported", 0.5f);
                EditorApplication.update();
            }
            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(editorPath);
            
            
            //Set the mono importer to the current graph editor script
            MonoImporter monoImporter = AssetImporter.GetAtPath(editorPath) as MonoImporter;

            if (monoImporter != null)
                monoImporter.SetDefaultReferences(new string[]{"nodeEditorData"}, new Object[]{nodeEditorData});
         
            
            


   
            //Refresh the asset ann close it
            //Mark it dirty
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Close();
        }
    }
}
