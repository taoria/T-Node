using UnityEngine;

namespace Sample{
    // Create at Asset/Test
    [CreateAssetMenu(fileName = "NewData", menuName = "Test/Data", order = 1)]
    public class TestExposedReference:ScriptableObject{
        public ExposedReference<Camera> camera;
        public ExposedReference<GameObject> go;
    }
}