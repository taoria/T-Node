using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TNode.TNodeCore.Editor.DeveloperHelper{
    /// <summary>
    /// Helper options works on 2021.3 on newer unity versions only.call this in the early version may not work
    /// </summary>
    public static class CleanMissingTypeHelper
    
    {
        [MenuItem("TNodeCore/CleanMissingType/CleanScriptObjects")]
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
                    #if UNITY_2021_3_OR_NEWER
                    if (SerializationUtility.ClearAllManagedReferencesWithMissingTypes(obj))
                    {
                       
                        report.Append("Cleared missing types from ").Append(path).AppendLine();
                    }
                    else
                    {
                        report.Append("No missing types to clear on ").Append(path).AppendLine();
                    }
                    #endif
                }
            }
            Debug.Log(report.ToString());
        }

        [MenuItem("TNodeCore/CleanMissingType/CleanSceneGameObjects")]
        public static void CleanMissingTypesOnGameObjects(){
            var report = new StringBuilder();

            SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects()){
                foreach (var o in root.transform){
                    #if UNITY_2021_3_OR_NEWER
                    if (SerializationUtility.ClearAllManagedReferencesWithMissingTypes(o as Object))
                    {
                        report.Append("Cleared missing types from ").Append(root.name).AppendLine();
                    }
                    else
                    {
                        report.Append("No missing types to clear on ").Append(root.name).AppendLine();
                    }
                    #endif
                }

#if UNITY_2021_3_OR_NEWER
                if (SerializationUtility.ClearAllManagedReferencesWithMissingTypes(root))
                {
                    report.Append("Cleared missing types from ").Append(root.name).AppendLine();
                }
                else
                {
                    report.Append("No missing types to clear on ").Append(root.name).AppendLine();
                }
#endif
            }
            Debug.Log(report.ToString());
            
        }
        
    }
}