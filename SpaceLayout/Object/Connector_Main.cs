using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Nevron.GraphicsCore;
using Nevron.Diagram;

namespace SpaceLayout.Object
{
    public class Connector_Main
    {
        public const int TYPE_VERITCAL = 0;
        public const int TYPE_HORIZONTAL = 1;

        public Zone_Main Zone21 { get; set; }//start node
        public Zone_Main Zone22 { get; set; } // end node
        
      
        public int Type { get; set; } // vertical,horizontal
        public double Length { get; set; }//width

        public Color Color { get; set; }

        

        public Color color { get; set; } // change color of line to define the difference
        public object ID { get; internal set; }

        //yWorks.Graph.IEdge edge;
        public NRoutableConnector connector;
        private Zone_Main z1;
        private Zone_Main z2;
        private int v1;
        private double v2;

        public Connector_Main(Zone_Main zone21, Zone_Main zone22, int type, double length, Color color)
        {
            this.Zone21 = zone21;
            this.Zone22 = zone22;
            this.Type = type;
            this.Length = length;
            this.Color = color;
           

            //connector = new NRoutableConnector();
            //connector.StyleSheetName = NDR.NameConnectorsStyleSheet;
            //connector.ConnectorType = RoutableConnectorType.DynamicPolyline;
            //connector.Style.StrokeStyle = new NStrokeStyle(1f, color);
            //connector.Style.StartArrowheadStyle.Shape = ArrowheadShape.None;
            //connector.Style.EndArrowheadStyle.Shape = ArrowheadShape.None;
        }

       
    }
}
