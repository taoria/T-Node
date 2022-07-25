using System;

namespace TNodeCore.RuntimeCache{
    public interface IModelPropertyAccessor{
        object GetValue(object model);
        void SetValue(object model, object value);

        public Type Type{ get; set; }


    }
}