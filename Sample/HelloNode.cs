using TNode.Attribute;
using TNode.Models;
using UnityEngine;

namespace Sample{
    [GraphUsage(typeof(HelloGraph))]
    public class HelloNode:NodeData{
        [ShowInNodeView]
        public string SayHelloText = "";
    }
}