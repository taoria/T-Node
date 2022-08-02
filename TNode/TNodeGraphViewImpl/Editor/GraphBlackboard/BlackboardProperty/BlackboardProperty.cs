using System;

namespace TNode.TNodeGraphViewImpl.Editor.GraphBlackboard.BlackboardProperty{
    public class BlackboardProperty{
        public string PropertyName;
        public Type PropertyType;
        // public RuntimeCache.RuntimeCache.GetValueDelegate GetValue;
        // public RuntimeCache.RuntimeCache.SetValueDelegate SetValue;
        public BlackboardProperty(string propertyName, Type propertyType){
            PropertyName = propertyName;
            PropertyType = propertyType;
        }
    }
}