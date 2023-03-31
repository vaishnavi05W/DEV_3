using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceLayout.Object;

namespace SpaceLayout
{
    public class Project
    {
        public string name;
        double id;
        public List<Zone> zoneList = new List<Zone>();
        public List<Connector> connectorList = new List<Connector>();
    }
}
