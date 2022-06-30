using System;

namespace TNode.Attribute{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ShowInNodeViewAttribute:System.Attribute{

    }
}