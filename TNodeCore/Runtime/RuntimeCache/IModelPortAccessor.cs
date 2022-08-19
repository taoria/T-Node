using System;
using UnityEditor.Experimental.GraphView;

namespace TNodeCore.Runtime.RuntimeCache{
    public interface IModelPortAccessor{
        object GetValue(object model);
        void SetValue(object model, object value);

        public Type Type{ get; set; }
        
         


    }
}