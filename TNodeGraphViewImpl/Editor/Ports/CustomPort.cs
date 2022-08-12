using System;
using System.Collections.Generic;
using System.Linq;
using TNodeCore.Editor.NodeGraphView;
using TNodeCore.Runtime.Models;
using TNodeGraphViewImpl.Editor.NodeViews;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace TNodeGraphViewImpl.Editor.Ports{
    public class CustomPort:UnityEditor.Experimental.GraphView.Port{
        public CustomPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type){
          m_EdgeConnector = new EdgeConnector<Edge>(new CustomEdgeConnectorListener());
          this.AddManipulator(m_EdgeConnector);
        }
    }

    public class CustomEdgeConnectorListener : IEdgeConnectorListener{
      private GraphViewChange m_GraphViewChange;
      private List<Edge> m_EdgesToCreate;
      private List<GraphElement> m_EdgesToDelete;

      public CustomEdgeConnectorListener(){
        this.m_EdgesToCreate = new List<Edge>();
        this.m_EdgesToDelete = new List<GraphElement>();
        this.m_GraphViewChange.edgesToCreate = this.m_EdgesToCreate;
      }

      public NodeLink FromEdgeToNodeLink(GraphData graphData, Edge edge){
        var inputNode = edge.input.node;
        var outputNode = edge.output.node;
        var links = graphData.NodeLinks.Where(x=>x.inPort.portEntryName==edge.input.name && x.outPort.portEntryName==edge.output.name);
        return links.FirstOrDefault();
      }

      public NodeLink MakeNodeLink(Edge edge){
        var inputNode = edge.input.node as IBaseNodeView;
        if (inputNode == null) return null;
        var outputNode = edge.output.node as IBaseNodeView;
        if (outputNode == null) return null;
        var link = new NodeLink(new PortInfo{
          nodeDataId = inputNode.GetNodeData().id,
          portEntryName = edge.input.name

        }, new PortInfo(){
          nodeDataId = outputNode.GetNodeData().id,
          portEntryName = edge.output.name
        });
        return link;
      }
      public void OnDropOutsidePort(Edge edge, UnityEngine.Vector2 position){
       
      }
      public void OnDrop(UnityEditor.Experimental.GraphView.GraphView graphView, Edge edge){
        this.m_EdgesToCreate.Clear();
        this.m_EdgesToCreate.Add(edge);
        this.m_EdgesToDelete.Clear();
        if (edge.input.capacity == Port.Capacity.Single){
          foreach (Edge connection in edge.input.connections){
            if (connection != edge)
              this.m_EdgesToDelete.Add((GraphElement) connection);
          }
        }

        if (edge.output.capacity == Port.Capacity.Single){
          foreach (Edge connection in edge.output.connections){
            if (connection != edge)
              this.m_EdgesToDelete.Add(connection);
          }
        }

        var baseDataGraphView = (IBaseDataGraphView) graphView;
        if (m_EdgesToDelete.Count > 0){
          graphView.DeleteElements(this.m_EdgesToDelete);
          baseDataGraphView.RemoveLink(FromEdgeToNodeLink(baseDataGraphView.GetGraphData(), edge));
        }
        
        List<Edge> edgesToCreate = this.m_EdgesToCreate;
        if (graphView.graphViewChanged != null)
          edgesToCreate = graphView.graphViewChanged(this.m_GraphViewChange).edgesToCreate;
        foreach (Edge edge1 in edgesToCreate){
          graphView.AddElement(edge1);
          edge.input.Connect(edge1);
          edge.output.Connect(edge1);
          baseDataGraphView.AddLink(MakeNodeLink(edge1));
        }
        
      }
    }

}