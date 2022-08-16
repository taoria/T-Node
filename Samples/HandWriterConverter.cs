using TNodeCore.Runtime.Attributes;
using TNodeCore.Runtime.Interfaces;
using UnityEngine;

namespace Samples{
    [GraphUsage(typeof(HelloGraph))]
    public class HandWriterConverter:PortTypeConversion<GameObject,Vector3>{
        public override Vector3 Convert(GameObject tFrom){
            return tFrom.gameObject.transform.position;
        }
    }
}