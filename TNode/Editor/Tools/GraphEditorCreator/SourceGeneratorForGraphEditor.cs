using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using UnityEngine;

namespace TNode.Editor.Tools.GraphEditorCreator{
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
        public string GenerateGraph(string graphClassName,string templatePath){
            //Check if graph class name is valid
            var regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9_]+$");
            if(!Regex.IsMatch(graphClassName)){
                Debug.LogError("The graph class name is invalid. It must be a valid C# identifier.");
            }
            var template = File.ReadAllText(templatePath);
            var source = template.Replace("$GraphClassName$",graphClassName);
            return source;
        }
    }
}