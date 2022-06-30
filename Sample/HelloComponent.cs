using System;
using UnityEngine;

namespace Sample{
    public class HelloComponent:MonoBehaviour{
        public void Update(){
            var trans1 = gameObject.transform;
            var trans2 = gameObject.GetComponent<Transform>();
            var tran3 = GetComponent<Transform>();
            var tran4 = transform;
        }
    }
}