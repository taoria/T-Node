using System;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace TNode.Attribute{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    
    public class PortAttribute:System.Attribute{
        public string PortName;
        public string PortAccepted;
        public Type NodeLinkType;
        public Port.Capacity Capacity;

        public PortAttribute(string portName, Type nodeLinkType, Port.Capacity capacity,string portAccepted="*"){
            PortName = portName;
            PortAccepted = portAccepted;
            NodeLinkType = nodeLinkType;
            Capacity = capacity;
        }
        //Auto generate port name via variable use this attribute
        public PortAttribute(Type nodeLinkType, Port.Capacity capacity, string portAccepted = "*"){
            PortAccepted = portAccepted;
            NodeLinkType = nodeLinkType;
            Capacity = capacity;
        }
    }
}