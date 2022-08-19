using System.Linq;
using NUnit.Framework;
using TNode.TNodeCore.Runtime.Models;
using TNodeCore.Editor.Tools.NodeCreator;
using TNodeCore.Runtime;
using TNodeCore.Runtime.Attributes;
using TNodeCore.Runtime.Attributes.Ports;
using TNodeCore.Runtime.Models;
using TNodeCore.Runtime.RuntimeModels;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Tests{
    public class StaticGraphTest{
        internal class GraphDataForTest:GraphData{
            
        }
        [GraphUsage(typeof(GraphDataForTest))]
        internal class TestNode : NodeData{
            [Input] public int Input{ get; set; }
            [Output] public int Output{ get; set; }
        }
        [GraphUsage(typeof(GraphDataForTest))]
        internal class TestConditionalNode : ConditionalNode{
            public bool TestCondition = false;
            [Output]
            public TransitionCondition Output(){
                return new TransitionCondition(){
                    Condition =  TestCondition
                };
            }
        }
        
        [Test]
        public void TestStaticGraphCreation(){
            GraphData graphData = ScriptableObject.CreateInstance<GraphData>();
            var node = NodeCreator.InstantiateNodeData<TestNode>();
            graphData.NodeDictionary.Add(node.id,node);
            Assert.AreEqual(1,graphData.NodeDictionary.Count);
            
            var staticGraph = new StaticGraph(graphData.NodeDictionary.Values.ToList(),graphData.NodeLinks);
            Assert.NotNull(staticGraph);
            Assert.AreEqual(1,staticGraph.GetRuntimeNodes().Count);
        }

        [Test]
        public void TestStaticGraphAccess(){
            GraphData graphData = ScriptableObject.CreateInstance<GraphData>();
            var node = NodeCreator.InstantiateNodeData<TestNode>();
            graphData.NodeDictionary.Add(node.id,node);
          
            
            var staticGraph = new StaticGraph(graphData.NodeDictionary.Values.ToList(),graphData.NodeLinks);

            
            Assert.AreEqual(staticGraph.GetRuntimeNodes().First(),staticGraph.CurrentRuntimeNode());
        }

        [Test]
        public void TestStaticGraphBfs(){
            GraphData graphData = ScriptableObject.CreateInstance<GraphData>();
            var node1 = NodeCreator.InstantiateNodeData<TestNode>();
            var node2 = NodeCreator.InstantiateNodeData<TestNode>();
            var node3 = NodeCreator.InstantiateNodeData<TestNode>();
            var node4 = NodeCreator.InstantiateNodeData<TestNode>();
            graphData.NodeDictionary.Add(node1.id,node1);
            graphData.NodeDictionary.Add(node2.id,node2);
            graphData.NodeDictionary.Add(node3.id,node3);
            graphData.NodeDictionary.Add(node4.id,node4);
            //Link node1 to node2
            graphData.NodeLinks.Add(new NodeLink(new PortInfo{
                portEntryName = "Input",
                nodeDataId = node2.id
            },new PortInfo{
                portEntryName = "Output",
                nodeDataId = node1.id
            }));
            //Link node2 to node4
            graphData.NodeLinks.Add(new NodeLink(new PortInfo{
                portEntryName = "Input",
                nodeDataId = node4.id
            },new PortInfo{
                portEntryName = "Output",
                nodeDataId = node2.id
            }));
            //LINK NODE4 TO NODE3
            graphData.NodeLinks.Add(new NodeLink(new PortInfo{
                portEntryName = "Input",
                nodeDataId = node3.id
            },new PortInfo{
                portEntryName = "Output",
                nodeDataId = node4.id
            }));
            
            
            
            


            var staticGraph = new StaticGraph(graphData.NodeDictionary.Values.ToList(),graphData.NodeLinks);
            
            Assert.AreEqual(node1,staticGraph.CurrentNode());
            staticGraph.MoveNext();
            Assert.AreEqual(node2,staticGraph.CurrentNode());
            staticGraph.MoveNext();
            Assert.AreEqual(node4,staticGraph.CurrentNode());
            staticGraph.MoveNext();
            Assert.AreEqual(node3,staticGraph.CurrentNode());
            
        }

        [Test]
        public void TestStaticConditionalBfs(){
            GraphData graphData = ScriptableObject.CreateInstance<GraphData>();
            var node1 = NodeCreator.InstantiateNodeData<TestNode>();
            var node2 = NodeCreator.InstantiateNodeData<TestNode>();
            var node3 = NodeCreator.InstantiateNodeData<TestConditionalNode>();
            var node4 = NodeCreator.InstantiateNodeData<TestConditionalNode>();
            var node5 = NodeCreator.InstantiateNodeData<TestNode>();
            var node6 = NodeCreator.InstantiateNodeData<TestNode>();
            
            
            graphData.NodeDictionary.Add(node1.id,node1);
            graphData.NodeDictionary.Add(node2.id,node2);
            graphData.NodeDictionary.Add(node3.id,node3);
            graphData.NodeDictionary.Add(node4.id,node4);
            graphData.NodeDictionary.Add(node5.id,node5);
            graphData.NodeDictionary.Add(node6.id,node6);
            
            
     
            //Link node1 to node2
            graphData.NodeLinks.Add(new NodeLink(new PortInfo{
                portEntryName = "Input",
                nodeDataId = node2.id
            },new PortInfo{
                portEntryName = "Output",
                nodeDataId = node1.id
            }));
            
            //Link node2 to node3
            graphData.NodeLinks.Add(new NodeLink(new PortInfo{
                portEntryName = "In",
                nodeDataId = node3.id
            },new PortInfo{
                portEntryName = "Output",
                nodeDataId = node2.id
            }));
            //Link node3 to node6
            graphData.NodeLinks.Add(new NodeLink(new PortInfo{
                portEntryName = "Input",
                nodeDataId = node6.id
            },new PortInfo{
                portEntryName = "Output",
                nodeDataId = node3.id
            }));

            node3.TestCondition = true;
            
            var staticGraph = new StaticGraph(graphData.NodeDictionary.Values.ToList(),graphData.NodeLinks);
            Assert.AreEqual(node1,staticGraph.CurrentNode());
            staticGraph.MoveNext();
            Assert.AreEqual(node4,staticGraph.CurrentNode());
            staticGraph.MoveNext();
            Assert.AreEqual(node5,staticGraph.CurrentNode());
            staticGraph.MoveNext();
            Assert.AreEqual(node2,staticGraph.CurrentNode());
            staticGraph.MoveNext();
            Assert.AreEqual(node3,staticGraph.CurrentNode());
            staticGraph.MoveNext();
            Assert.AreEqual(node6,staticGraph.CurrentNode());
            
            node3.TestCondition = false;
            
            var staticGraph2 = new StaticGraph(graphData.NodeDictionary.Values.ToList(),graphData.NodeLinks);
            Assert.AreEqual(node1,staticGraph2.CurrentNode());
            staticGraph2.MoveNext();
            Assert.AreEqual(node4,staticGraph2.CurrentNode());
            staticGraph2.MoveNext();
            Assert.AreEqual(node5,staticGraph2.CurrentNode());
            staticGraph2.MoveNext();
            Assert.AreEqual(node2,staticGraph2.CurrentNode());
            staticGraph2.MoveNext();
            Assert.AreEqual(node3,staticGraph2.CurrentNode());
            staticGraph2.MoveNext();
            Assert.AreNotEqual(node6,staticGraph2.CurrentNode());
        }
    }
}