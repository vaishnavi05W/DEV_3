using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nevron.Diagram;
using Nevron.Diagram.Shapes;
using Nevron.Diagram.Batches;
using Nevron.Diagram.ThinWeb;
using Nevron.GraphicsCore;
//using yWorks.Algorithms;

namespace SpaceLayout.Object
{


    public class Connector
    {
        public  const int TYPE_VERITCAL = 0;
        public  const int TYPE_HORIZONTAL = 1;

        public Zone  Zone21; //start node
        public Zone Zone22; // end node
        public int Type; // vertical,horizontal
        public double Length; //width

        public Color Color { get; }

        public Color color; // change color of line to define the difference
                            //yWorks.Graph.IEdge edge;
        public NRoutableConnector connector;

        public Connector(Zone zone21, Zone zone22, int type, double length, Color color)
        {
            Zone21 = zone21;
            Zone22 = zone22;
            Type = type;
            Length = length;
            Color = color;

            connector = new NRoutableConnector();
            connector.StyleSheetName = NDR.NameConnectorsStyleSheet;
            connector.ConnectorType = RoutableConnectorType.DynamicPolyline;
            connector.Style.StrokeStyle = new NStrokeStyle(1f, color);
            connector.Style.StartArrowheadStyle.Shape = ArrowheadShape.None;
            connector.Style.EndArrowheadStyle.Shape = ArrowheadShape.None;




        }
    }
}



