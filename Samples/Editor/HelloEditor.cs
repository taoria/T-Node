using TNodeCore.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Samples.Editor{
    public class HelloEditor : GraphEditor<HelloGraph>{
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line){
            var graph = EditorUtility.InstanceIDToObject(instanceID) as HelloGraph;
            if (graph != null)  {
                var wnd = GetWindow<HelloEditor>();
                wnd.titleContent = new GUIContent("HelloGraph Editor");
                wnd.Show();
                wnd.SetupNonRuntime(graph);
                return true;
            }
            return false;
        }
        [MenuItem("Window/HelloEditor")]
        public static void ShowWindow(){
            var res = GetWindow<HelloEditor>();
            res.titleContent = new GUIContent("HelloGraph Editor");
            res.Show();
        }
                
    }
}