using System.Text.RegularExpressions;
using UnityEngine;

namespace TNodeCore.Editor.Tools.GraphEditorCreator{
    public class SourceGeneratorForGraphEditor{
        private readonly Regex Regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9_]+$");
        public string GenerateGraphEditor(string editorClassName,string graphClassName,string templateName="NewGraphEditor.cs"){
            //Load Text Asset by Name
            TextAsset template = Resources.Load<TextAsset>("ScriptTemplates/"+templateName);
            //Check if the class name is valid
            if(!Regex.IsMatch(editorClassName)){
                Debug.LogError("The editor class name is invalid. It must be a valid C# identifier.");
            }
            //Check if the graph class name is valid
            if(!Regex.IsMatch(graphClassName)){
                Debug.LogError("The graph class name is invalid. It must be a valid C# identifier.");
            }
            var source = template.text.Replace("$EditorClassName$",editorClassName).Replace("$GraphClassName$",graphClassName);
            return source;
        }  
        //what's the shortcut to navigate the files 
        
        public string GenerateGraph(string graphClassName,string templateName="NewGraph.cs"){
            TextAsset template = Resources.Load<TextAsset>("ScriptTemplates/"+templateName);
            //Check if graph class name is valid
            var regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9_]+$");
            if(!Regex.IsMatch(graphClassName)){
                Debug.LogError("The graph class name is invalid. It must be a valid C# identifier.");
            }
            var source = template.text.Replace("$GraphClassName$",graphClassName);
            return source;
        }

        public string GenerateGraphView(string graphViewClassName,string graphClassName,string templateName="NewGraphView.cs"){
            TextAsset template = Resources.Load<TextAsset>("ScriptTemplates/"+templateName);
            //Check if graph class name is valid
            var regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9_]+$");
            if(!Regex.IsMatch(graphClassName)){
                Debug.LogError("The graph class name is invalid. It must be a valid C# identifier.");
            }
            if(!Regex.IsMatch(graphViewClassName)){
                Debug.LogError("The graph view name is invalid. It must be a valid C# identifier.");
            }
            var source = template.text.Replace("$GraphClassName$",graphClassName).Replace("$GraphViewClassName$",graphViewClassName);
            return source;
        }
    }
}