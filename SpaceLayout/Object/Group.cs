using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceLayout.Object
{
    public class Group
    {
        public string Name { get; set; }
        public List<Zone_Main> Zones { get; set; }
        // Add other properties as needed

        public Group(string name, List<Zone_Main> zones)
        {
            Name = name;
            Zones = zones;
        }
    }
}
