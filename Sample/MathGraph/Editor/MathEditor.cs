using TNode.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
public class MathEditor : GraphEditor<MathGraph>{
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line){
            var graph = EditorUtility.InstanceIDToObject(instanceID) as MathGraph;
            if (graph != null)
            {
                var wnd = GetWindow<MathEditor>();
                wnd.titleContent = new GUIContent("MathGraph Editor");
                wnd.CreateGUI();
                wnd._graphView.Data = graph;
                return true;
            }
            return false;
        }
}