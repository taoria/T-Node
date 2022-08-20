using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TNodeCore.Editor.Tools.NodeCreator;
using TNodeCore.Runtime;
using TNodeCore.Runtime.Attributes;
using TNodeCore.Runtime.Attributes.Ports;
using TNodeCore.Runtime.Models;
using TNodeGraphViewImpl.Editor.NodeGraphView;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests{
    public class NodePortTest
    {
        internal class GraphDataForTest:GraphData{
            
        }
        [GraphUsage(typeof(GraphDataForTest))]
        internal class PortNameNodeData:NodeData{
            [Input(Group = true)]
            public List<string> Inputs{
                get => _inputs;
                set => _inputs = value;
            }

            private List<string> _inputs = new List<string>{"Input1", "Input2"};
        }
        [Test]
        public void TestWithPortName(){
            var graphView = new BaseDataGraphView<GraphData>();
            var node = NodeCreator.InstantiateNodeData<PortNameNodeData>();
            graphView.AddTNode(node,new Rect());
            Assert.AreEqual(graphView.nodes.ToList().Count, 1);
            Assert.AreEqual(graphView.ports.ToList().Count, 2);
            Debug.Log(graphView.ports.ToList().Aggregate("",(total,port)=> total + port.name));
        }

        [Test]
        public void TestWithRuntimeNodeAccessPort(){
            var node = NodeCreator.InstantiateNodeData<PortNameNodeData>();
            RuntimeNode runtimeNode = new RuntimeNode(node);
            runtimeNode.SetInput("Inputs:0", "Hello");
            Assert.AreEqual("Hello",node.Inputs[0]);
        }


        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NodeViewTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
