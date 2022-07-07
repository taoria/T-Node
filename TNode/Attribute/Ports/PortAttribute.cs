using System;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace TNode.Attribute{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    
    public class PortAttribute:System.Attribute{
  
    }
}