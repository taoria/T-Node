using System;
using UnityEditor.Experimental.GraphView;

namespace TNode.Attribute.Ports{
    /// <summary>
    /// Batch out port attribute could specify a batch out port,allowing large scale calculation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class BatchOutputAttribute:PortAttribute{

    }
}