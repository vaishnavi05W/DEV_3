using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceLayout.Object
{
    public class Zone
    {
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

        public Zone(string Name, int ID, string Group, string Relation, string Category, double Area, double Width, double Height, double Level, double Ratio, double Type, string Color, string type)
        {
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
        



    }

    
}