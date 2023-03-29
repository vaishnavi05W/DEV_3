using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceLayout.Object
{
    class Zone
    {
        public class Zone1
        {
            public string ID;
            public string Name;
            public string Group;
            public string Relation;
            public string Category;
            public double Area;
            public double Width;
            public double Length;
            public double Height;
            public double Level;
            public double Ratio;
            public string Type;
            public string color;

            public List<Zone> zones = new List<Zone>();
            // public RectD RectD = new RectD();



            public void Rectangle(double w, double h)
            {
                Width = w;
                Height = h;
                Area = w * h;

                //        }
                //    }
            }
        }
    }
}