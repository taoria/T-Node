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
        [DisableOnInspector]
        public string id;
        [NonSerialized]
        private int _fastAccessId=0;
        public object Clone(){
            var memberwiseClone = this.MemberwiseClone();
            return memberwiseClone;
        }
        /// <summary>
        /// Record and map the node by a string is cost.converted it to an integer to speed the process.
        /// </summary>
        public int FastAccessId => _fastAccessId==0?_fastAccessId=GetHashCode():_fastAccessId;
    }
}