using UnityEditor;

namespace TNodeCore.Extensions{
    public static class SerializedPropertyExtensions{
        public static object BoxedValue(this SerializedProperty serializedProperty){
            var targetObject = serializedProperty.serializedObject.targetObject;
            var targetObjectClassType = targetObject.GetType();
            var field = targetObjectClassType.GetField(serializedProperty.propertyPath);
            if (field != null)
            {
                var value = field.GetValue(targetObject);
                return value;
            }

            return null;
        }
    }
}