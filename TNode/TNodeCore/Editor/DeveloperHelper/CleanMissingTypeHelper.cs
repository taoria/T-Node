using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TNodeCore.Editor.DeveloperHelper{
    public class CleanMissingTypeHelper
    {
        [MenuItem("TNode/CleanMissingType/CleanScriptObjects")]
        public static void CleanMissingTypesOnScriptableObjects()
        {
            var report = new StringBuilder();

            var guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] {"Assets"});
            foreach (string guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                Object obj = AssetDatabase.LoadMainAssetAtPath(path);
                if (obj != null)
                {
                    if (SerializationUtility.ClearAllManagedReferencesWithMissingTypes(obj))
                    {
                        report.Append("Cleared missing types from ").Append(path).AppendLine();
                    }
                    else
                    {
                        report.Append("No missing types to clear on ").Append(path).AppendLine();
                    }
                }
            }
            Debug.Log(report.ToString());
        }

        [MenuItem("TNode/CleanMissingType/CleanSceneGameObjects")]
        public static void CleanMissingTypesOnGameObjects(){
            var report = new StringBuilder();

            SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects()){
                foreach (var o in root.transform){
                    if (SerializationUtility.ClearAllManagedReferencesWithMissingTypes(o as Object))
                    {
                        report.Append("Cleared missing types from ").Append(root.name).AppendLine();
                    }
                    else
                    {
                        report.Append("No missing types to clear on ").Append(root.name).AppendLine();
                    }
                }


                if (SerializationUtility.ClearAllManagedReferencesWithMissingTypes(root))
                {
                    report.Append("Cleared missing types from ").Append(root.name).AppendLine();
                }
                else
                {
                    report.Append("No missing types to clear on ").Append(root.name).AppendLine();
                }
            }
            Debug.Log(report.ToString());
            
        }
        
    }
}