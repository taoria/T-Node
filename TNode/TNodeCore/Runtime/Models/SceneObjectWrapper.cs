using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace TNodeCore.Models{
    
    /// <summary>
    /// Scene Object wrapper use to serialize blackboard data
    /// </summary>
    public class SceneObjectWrapper:DataWrapper<SceneObjectWrapper,BlackboardData>{
        public bool loadedFromScene = false;
        [NonSerialized]
        public List<string> PossibleSceneFields;
        public List<SceneSerializationEntry> SceneFields;
        public struct SceneSerializationEntry{
            public string ScenePath;
            public string FieldName;
            public string SceneObject;

        }
        public bool infoCached;
        //Reflection may be expensive, so we cache the type
        
        public Type Type;
        //we need to cache blackboard data info
        public void LoadBlackboardInfo(){
            if (infoCached) return;
            infoCached = true;
            Type = data.GetType();
            PossibleSceneFields = new List<string>();
            foreach (var field in Type.GetFields()){
                if (field.FieldType == typeof(Object)){
                    PossibleSceneFields.Add(field.Name);
                }
            }
        }
        #region Oboselete Region for serialization

  //
  //       public string BuildThroughTransform(Object go, Transform root,int id){
  //           if (root.gameObject == go) return $"/{id}";
  //           var res = root.transform.Find(go.name);
  //           if (res != null){
  //               for (int j = 0; j < root.transform.childCount; j++){
  //                   var child = root.transform.GetChild(j);
  //                   var childRes = BuildThroughTransform(go,child,j);
  //                   if(childRes=="") continue;
  //                   return $"/{id}{childRes}";
  //               }
  //           }
  //           return "";
  //       }
  //       public string BuildPath(Object go,GameObject[] roots){
  //          
  //           //Search root objects
  //           for(int i=0;i<roots.Length;i++){
  //               var res = BuildThroughTransform(go,roots[i].transform,i);
  //               if (res != "") return res;
  //           }
  //
  //           return "";
  //       }
  //       public string GetSceneObjectSerializationInfo(Object value){
  //          
  //           var type = value.GetType();
  //           if (value is GameObject go){
  //               //check if go is a scene object
  //               if (go.scene.IsValid()){
  //                   //TODO costly operation
  //                   var roots = go.scene.GetRootGameObjects();
  //                   var path = BuildPath(go,roots);
  //                   return path;
  //               }
  //               else{
  //                   return "$not in scene$";
  //               }
  //           }
  //
  //           if (value is Behaviour be){
  //               if (be.gameObject.scene.IsValid()){
  //                   //TODO costly operation
  //                   GameObject gameObject;
  //                   var roots = (gameObject = be.gameObject).scene.GetRootGameObjects();
  //                   var path = BuildPath(gameObject,roots);
  //                   return path + $"/{value.GetType()}";
  //               }
  //               else{
  //                   return "$not in scene$";
  //               }
  //           }
  //
  //           return "";
  //
  //       }
  //       
  //       private void SaveToSceneFields(Object value,string fieldName){
  //           if (value == null) return;
  //           var scenePath = SceneManager.GetActiveScene().path;
  //           SceneFields.Add(new SceneSerializationEntry(){
  //                   ScenePath = scenePath,
  //                   FieldName = fieldName,
  //                   SceneObject = GetSceneObjectSerializationInfo(value)
  //           });
  //       }
  //       
   // public void Save(){
   //          SceneFields.Clear();
   //          //Search all possible scene fields to check if they have values
   //          foreach (var s in PossibleSceneFields){
   //              var value = data.GetValue(s) as Object;
   //              SaveToSceneFields(value,s);
   //          }
   //      }
   //
   //      public void Load(){
   //          Scene scene = SceneManager.GetActiveScene();
   //          if (scene.IsValid()){
   //              foreach (var s in SceneFields){
   //                  GameObject currentGo = null;
   //                  bool isBehaviour = false;
   //                  Behaviour behaviour = null;
   //                  if (s.ScenePath == scene.path){
   //                      var value = data.GetValue(s.FieldName) as Object;
   //                      if (value == null){
   //                          var path = s.SceneObject.Split('/');
   //                          for (var i = 0; i < path.Length; i++){
   //                              if (currentGo == null){
   //                                  var index = int.Parse(path[i]);
   //                                  currentGo = scene.GetRootGameObjects()[index];
   //                              }
   //                              if (i == path.Length - 1){
   //                                  var firstChar = path[i][0];
   //                                  //check if it's a number
   //                                  if (firstChar is >= '0' and <= '9'){
   //                                      var index = int.Parse(path[i]);
   //                                      currentGo = currentGo.transform.GetChild(index).gameObject;
   //                                      data.SetValue(s.FieldName,currentGo);
   //                                  }
   //                                  else{
   //                                      isBehaviour = true;
   //                                      behaviour= currentGo.GetComponent(path[i]) as Behaviour;
   //                                      data.SetValue(s.FieldName,behaviour);
   //                                  }
   //                              }
   //                              else{
   //                                  var index = int.Parse(path[i]);
   //                                  currentGo = currentGo?.transform?.GetChild(index).gameObject;
   //                              }
   //                          }
   //                      }
   //                  }
   //              }
   //          }
   //      }

        #endregion
      
       
        public override BlackboardData GetData(){
            if (data == null) return null;
            LoadBlackboardInfo();
            if (!Cache.ContainsKey(this)){
                Cache.Add(data,this);
            }
            if (loadedFromScene==false){
                loadedFromScene = true;
                Load();
            }
            return data;
        }

        private void Load(){
            var res = SceneManager.GetActiveScene();
            GameObject.FindGameObjectsWithTag("SerializedData");
        }
    }
}