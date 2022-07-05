using UnityEditor.Experimental.GraphView;

namespace TNode.Editor.GraphBlackboard{
    public class BlackboardPropertyField:BlackboardField{
         public BlackboardProperty BlackboardProperty;
         public BlackboardPropertyField(BlackboardProperty blackboardProperty):base(null,blackboardProperty.PropertyName,null){
                BlackboardProperty = blackboardProperty;
         }
         
            
    }
}