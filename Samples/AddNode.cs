
using TNodeCore.Runtime;
using TNodeCore.Runtime.Attributes;
using TNodeCore.Runtime.Attributes.Ports;
using TNodeCore.Runtime.Models;
using UnityEngine;

namespace Samples{
    [GraphUsage(typeof(HelloGraph),"Math")]
    public class AddNode:NodeData{
        [Input]
        public Vector3 A{ get; set; }
        [Input]
        public Vector2 B{ get; set; }
        [Output]
        public Vector3 Res{ get; set; }

        public override void Process(){
            Res = A + (Vector3)B;
            Debug.Log(Res);
            this.Log(Res.ToString());
        }
    }
}