using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceLayout.Object
{
    public class Zone_Main
    {
        public Zone_Main(int ID, string Name, string Group,string Relation, string Category,string Color, double Area, double Width, double Lenght, double Height, int Floor, double Ratio, string Type)
        {
            this.ID = ID;
            this.Name = Name;
            this.Group = Group;
            this.Relation = Relation;
            this.Category = Category;
            this.Color = Color;
            this.Area = Area;
            this.Width = Width;
            this.Length = Lenght;
            this.Height = Height;
            this.Floor = Floor;
            this.Ratio = Ratio;
            this.Type = Type;
            //this.Tag = Tag;
            //this.Labels = Labels;
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public string Relation { get; set; }
        public string Category { get; set; }
        public string Color { get; set; }
        public double Area { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public double Height { get; set; }
        public int Floor { get; set; }
        public double Ratio { get; set; }
        public string Type { get; set; }
        //public Zone Tag { get; internal set; }
        //public object Labels { get; internal set; }
    }
}
