using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TNodeCore.Components{
    public class RuntimeDataSaver:MonoBehaviour{
        public string saveName;
        public string saveExtension = "tng";
        public Dictionary<string, object> savedData = new();
        public void Load(){
            string path = Application.persistentDataPath + "/"+ saveName + "." + saveExtension;
            if(!File.Exists(path)){
                Debug.LogWarning("File not found: " + path);
                return;
            }
            string json = File.ReadAllText(path);
            savedData = JsonUtility.FromJson<Dictionary<string, object>>(json);
        }
        public void Save(){
            string path = Application.persistentDataPath + "/" + saveName + "." + saveExtension;
            string json = JsonUtility.ToJson(savedData);
            File.WriteAllText(path, json);
        }

        public void Write(string id,object o){
            if (savedData.ContainsKey(id)){
                savedData[id] = o;
            }
            else{
                savedData.Add(id,o);
            }
        }

        public object Read(string id){
            return savedData.ContainsKey(id) ? savedData[id] : null;
        }
        public bool Has(string id){
            return savedData.ContainsKey(id);
        }
        public void Remove(string id){
            if (savedData.ContainsKey(id)){
                savedData.Remove(id);
            }
        }
    }
}