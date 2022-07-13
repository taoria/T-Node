using UnityEditor.Experimental.GraphView;

namespace TNodeGraphViewImpl.Editor.GraphBlackboard{
    public class BlackboardPropertyField:BlackboardField{
         public BlackboardProperty.BlackboardProperty BlackboardProperty;
         public BlackboardPropertyField(BlackboardProperty.BlackboardProperty blackboardProperty):base(null,blackboardProperty.PropertyName,null){
                BlackboardProperty = blackboardProperty;
         }
         
            
    }
}