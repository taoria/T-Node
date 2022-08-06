using System;
using System.Collections.Generic;
using Samples;
using TNodeCore.Runtime.Attributes;
using TNodeCore.Runtime.Models;
using UnityEngine;

namespace TNode.Samples{
    [GraphUsage(typeof(HelloGraph))]
    public class HelloBlackboard:BlackboardData{
        public string HelloString;
        public GameObject HelloGameObject;
        public List<Vector3> V3S;
        public List<Vector2> V2S;

    }
}