using System.Linq;
using TNodeCore.Editor.NodeGraphView;
using TNodeGraphViewImpl.Editor.NodeGraphView;
using TNodeGraphViewImpl.Editor.NodeViews;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNodeGraphViewImpl.Editor.GraphWatcherView{
    public class GraphWatcherView:SimpleGraphSubWindow{
        private Node _highlightedNode;

        public GraphWatcherView() : base(Resources.Load<VisualTreeAsset>("GraphWatcher")){
            styleSheets.Add(Resources.Load<StyleSheet>("GraphWatcherStyle"));
            var button = this.Q<Button>("Next");
            var button2 = this.Q<Button>("Reset");
            var label = this.Q<Label>();
            button.clicked += () => {
                var graphView = GetFirstAncestorOfType<IBaseDataGraphView>();
                var runtimeNodeGraph = graphView.GetRuntimeNodeGraph();
                runtimeNodeGraph.MoveNext();
                label.text = runtimeNodeGraph.CurrentNode().id;
                if (graphView is GraphView gv){
                    if (_highlightedNode != null){
                        _highlightedNode.RemoveFromClassList("highlightNode");
                        _highlightedNode.style.borderBottomWidth = _highlightedNode.style.borderLeftWidth =
                            _highlightedNode.style.borderRightWidth = _highlightedNode.style.borderTopWidth = 0;
                    }
                    var baseNodeViews = gv.nodes.ToList().Select(x=>(IBaseNodeView)x);
                    var node = baseNodeViews.First(x=>x.GetNodeData().id==runtimeNodeGraph.CurrentNode().id);
                    Debug.Log(node.GetNodeData().id);
                    var nodeView = (Node)node;
                    _highlightedNode = nodeView;
                    _highlightedNode.AddToClassList("highlightNode");
                    _highlightedNode.style.borderBottomWidth 
                        = _highlightedNode.style.borderLeftWidth
                            = _highlightedNode.style.borderRightWidth 
                                = _highlightedNode.style.borderTopWidth = 2;
                }
            };

            button2.clicked += () => {
                var graphView = GetFirstAncestorOfType<IBaseDataGraphView>();
                var runtimeNodeGraph = graphView.GetRuntimeNodeGraph();
                label.text = runtimeNodeGraph.CurrentNode().id;
                if (graphView is GraphView gv){
                    if (_highlightedNode != null){
                        _highlightedNode.RemoveFromClassList("highlightNode");
                        _highlightedNode.style.borderBottomWidth = _highlightedNode.style.borderLeftWidth =
                            _highlightedNode.style.borderRightWidth = _highlightedNode.style.borderTopWidth = 0;
                    }
                    runtimeNodeGraph.ResetState();
                }
            };

        }
    }
}