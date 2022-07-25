
// namespace TNode.Editor{
//     [CustomPropertyDrawer(typeof(BlackboardData))]
//     public class BlackboardDataPropertyDrawer:PropertyDrawer{
//         public float height = 0;
//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//         {
//             // Using BeginProperty / EndProperty on the parent property means that
//             // prefab override logic works on the entire property.
//             var to = property.serializedObject.targetObject;
//           
//             if (to is RuntimeGraph runtimeGraph){
//                 var blackboardData = property.boxedValue;
//                 var graphType = runtimeGraph.graphData.GetType();
//        
//                 Debug.Log(blackboardData);
//                 
//                 if (blackboardData == null || blackboardData.GetType()==typeof(BlackboardData))
//                 {   blackboardData= NodeEditorExtensions.GetAppropriateBlackboardData(graphType);
//                     property.boxedValue = blackboardData;
//                     
//                 }
//                 
//                 
//                 var posy = position.y;
//                 EditorGUI.BeginProperty(position, label, property);
//
//                 // Draw label
//                 EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
//                 height = EditorGUIUtility.singleLineHeight;
//
//                 // Don't make child fields be indented
//                 var indent = EditorGUI.indentLevel;
//                 EditorGUI.indentLevel = 0;
//
//                 //find the blackboard data
//      
//                 var blackboardDataFields = blackboardData.GetType().GetFields();
//                 posy += EditorGUIUtility.singleLineHeight;
//                 foreach (var blackboardDataField in blackboardDataFields){
//                     var newPosition = new Rect(position.x, posy, position.width, EditorGUIUtility.singleLineHeight);
//                     EditorGUI.PropertyField(newPosition, property.FindPropertyRelative(blackboardDataField.Name), new GUIContent(blackboardDataField.Name));
//                     posy += EditorGUIUtility.singleLineHeight;
//                     height+=EditorGUIUtility.singleLineHeight;
//                 }
//
//                 // Set indent back to what it was
//                 EditorGUI.indentLevel = indent;
//            
//                 EditorGUI.EndProperty();
//             }
//             
//          
//         }
//         public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//         {
//             return base.GetPropertyHeight(property, label) + height;
//         }
//     }
// }