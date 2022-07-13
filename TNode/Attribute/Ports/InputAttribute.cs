using System;
using JetBrains.Annotations;

namespace TNode.Attribute.Ports{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class InputAttribute : PortAttribute{
        public InputAttribute(string name="", PortNameHandling nameHandling = PortNameHandling.Auto) : base(name, nameHandling){
        }
    }
}