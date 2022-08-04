using System;
using TNodeCore.Runtime.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace TNodeCore.Runtime.Models{
    [Serializable]
    public abstract class Model:ICloneable{
        #if UNITY_EDITOR
        [HideInBlackboard]
        public Rect positionInView;
        #endif

        public object Clone(){
            var memberwiseClone = this.MemberwiseClone();
            return memberwiseClone;
        }
    }
}