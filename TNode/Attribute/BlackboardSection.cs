using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace TNode.Attribute{
    
    /// <summary>
    /// Use this attribute to declare a blackboard section ,a blackboard section is a group of  variables with same types
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    [BaseTypeRequired(typeof(List<>))]
    
    public class BlackboardSection:System.Attribute{
        
    }
}