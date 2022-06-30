using TNode.Attribute;
using TNode.Editor.BaseViews;
using TNode.Editor.Inspector;
using UnityEngine;

namespace Sample.Editor{
    [NodeComponent]
    public class HelloGraphView : DataGraphView<HelloGraph>{
        public override void OnGraphViewCreate(){
            CreateInspector();
        }


    }
}