using System;
using JetBrains.Annotations;
using TNode.Models;
using UnityEditor.Experimental.GraphView;

namespace TNode.Attribute{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class InputAttribute : PortAttribute{
        public InputAttribute(string portName, Type nodeLinkType, Port.Capacity capacity, string portAccepted = "*") : base(portName, nodeLinkType, capacity, portAccepted){
        }
        public InputAttribute(Type nodeLinkType, Port.Capacity capacity, string portAccepted="*") : base(nodeLinkType, capacity, portAccepted){
        }
        public InputAttribute(string portName="*",string portAccepted = "*") :base(portName, typeof(NodeLink),Port.Capacity.Multi,portAccepted){
        }
    }
}