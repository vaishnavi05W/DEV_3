using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceLayout.Object
{
    public class Zone
    {
        public int guid;
        public int ID;
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
        public string Color;

        public Zone(int guid, string Name, int ID, string Group, string Relation, string Category, double Area, double Width, double Height, double Level, double Ratio, double Type, string Color, string type)
        {

            this.guid = guid;
            this.Name = Name;
            this.ID = ID;
            this.Group = Group;
            this.Relation = Relation;
            this.Category = Category;
            this.Area = Area;
            this.Width = Width;
            this.Height = Height;
            this.Level = Level;
            this.Ratio = Ratio;
            this.Type = type;
            this.Color = Color;
        }
        foreach 

            public List<Zone> VLinkedZone = new List<Zone>(); //Vertically linked zones
            public List<Zone> HLinkedZone = new List<Zone>(); //Horizontally linked zones
            // public RectD RectD = new RectD();
                   
           
    }

    
}