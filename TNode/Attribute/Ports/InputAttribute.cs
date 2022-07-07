using System;
using JetBrains.Annotations;
using TNode.Models;
using UnityEditor.Experimental.GraphView;

namespace TNode.Attribute{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class InputAttribute : PortAttribute{
 
    }
}