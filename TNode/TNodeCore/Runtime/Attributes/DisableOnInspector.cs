using System;

namespace TNodeCore.Runtime.Attributes{
    /// <summary>
    /// Use this attribute to mark a field as disabled.An disable field will not be edit by the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DisableOnInspectorAttribute:System.Attribute{
        
    }
}