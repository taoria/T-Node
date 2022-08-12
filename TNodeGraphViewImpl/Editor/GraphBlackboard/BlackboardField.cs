namespace TNodeGraphViewImpl.Editor.GraphBlackboard{
    public class BlackboardField:UnityEditor.Experimental.GraphView.BlackboardField{
         public global::TNodeGraphViewImpl.Editor.GraphBlackboard.BlackboardProperty.BlackboardProperty BlackboardProperty;
         public BlackboardField(global::TNodeGraphViewImpl.Editor.GraphBlackboard.BlackboardProperty.BlackboardProperty blackboardProperty):base(null,blackboardProperty.PropertyName,null){
                BlackboardProperty = blackboardProperty;
         }
    }
}