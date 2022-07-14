using System.Collections.Generic;
using System.Linq;
using TNode.Editor.Serialization;
using UnityEngine;

namespace TNode.Models{
    
    /// <summary>
    /// Scene Object wrapper use to serialize blackboard data
    /// </summary>
    public class SceneObjectWrapper:DataWrapper<SceneObjectWrapper,BlackboardData>{
        public bool loadedFromScene =false;

        public List<string> sceneObjects = new List<string>();
        public void LoadFromScene(){
            
        }
        public override BlackboardData GetData(){
            if (data == null) return null;
            if (!Cache.ContainsKey(this)){
                Cache.Add(data,this);
            }

            if (loadedFromScene==false){
                loadedFromScene = true;
            }
            
            return data;
        }
    }
}