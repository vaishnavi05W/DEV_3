﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceLayout.Object
{
    public class Connector_Group
    {
        public const int TYPE_VERITCAL = 0;
        public const int TYPE_HORIZONTAL = 1;
        public Group Group1 { get; set; } //start group
        public Group Group2 { get; set; } //end group
        public string Axis { get; set; }
        public string Type { get; set; } // vertical,horizontal
        public double Length { get; set; }//width
        public Connector_Group(Group group21, Group group22, string axis, string type, double length)
        {
            this.Group1 = group21;
            this.Group2 = group22;
            this.Axis = axis;
            this.Type = type;
            this.Length = length;
            //connector = new NRoutableConnector();
            //connector.StyleSheetName = NDR.NameConnectorsStyleSheet;
            //connector.ConnectorType = RoutableConnectorType.DynamicPolyline;
            //connector.Style.StrokeStyle = new NStrokeStyle(1f, color);
            //connector.Style.StartArrowheadStyle.Shape = ArrowheadShape.None;
            //connector.Style.EndArrowheadStyle.Shape = ArrowheadShape.None;
        }
    }
}
