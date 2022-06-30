using TNode.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Sample.Editor{
    public class HelloEditor : GraphEditor<HelloGraph>{
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line){
            var graph = EditorUtility.InstanceIDToObject(instanceID) as HelloGraph;
            if (graph != null)
            {
                var wnd = GetWindow<HelloEditor>();
                wnd.titleContent = new GUIContent("HelloGraph Editor");
                wnd.CreateGUI();
                wnd._graphView.Data = graph;
                return true;
            }
            return false;
        }

        public HelloEditor(){
            
        }
    }
}