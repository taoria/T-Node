using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace TNodeGraphViewImpl.Editor.GraphBlackboard{
    public class BlackboardField:UnityEditor.Experimental.GraphView.BlackboardField{
         public BlackboardProperty.BlackboardProperty BlackboardProperty;
         public BlackboardField(BlackboardProperty.BlackboardProperty blackboardProperty):base(null,blackboardProperty.PropertyName,null){
                BlackboardProperty = blackboardProperty;
         }
    }
}