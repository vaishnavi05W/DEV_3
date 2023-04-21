using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yWorks.Algorithms;

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
        public Color color; // change color of line to define the difference
        yWorks.Graph.IEdge edge;
    }
}



