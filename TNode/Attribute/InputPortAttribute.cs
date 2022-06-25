using System;
using JetBrains.Annotations;
using TNode.Models;
using UnityEditor.Experimental.GraphView;

namespace TNode.Attribute{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class InputPortAttribute : PortAttribute{
        public InputPortAttribute(string portName, Type nodeLinkType, Port.Capacity capacity, string portAccepted = "*") : base(portName, nodeLinkType, capacity, portAccepted){
        }
        public InputPortAttribute(Type nodeLinkType, Port.Capacity capacity, string portAccepted="*") : base(nodeLinkType, capacity, portAccepted){
        }
        public InputPortAttribute(string portName="*",string portAccepted = "*") :base(portName, typeof(NodeLink),Port.Capacity.Multi,portAccepted){
        }
    }
}