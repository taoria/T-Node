using System;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace TNode.Attribute{
    [MeansImplicitUse]
    public class InputPortAttribute : System.Attribute{
        public Type NodeLinkType;
        public string PortAccepted;
        public Port.Capacity Capacity;
    }
}