using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using yWorks.Graph;
using yWorks.Geometry;
using yWorks.Controls;
using System.Data;
using VectorDraw.Serialize.Properties;
using ExcelDataReader;
using SpaceLayout.Entity;
using SpaceLayout.DataSource;
using yWorks.Graph.Styles;

namespace SpaceLayout.Object
{
    public class Zone
    {
        private List<Zone> ZonesList;
        internal ShapeNodeStyle Style;
        public List<Connector> Connectors;
        public INode Node;


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
        public Zone Tag { get; internal set; }
        public RectD Layout { get; internal set; }
        public object Labels { get; internal set; }



        public void LoadZoneFromDataTable(DataTable dtSource)
        {
            List<Zone> zones = new List<Zone>();
            foreach (DataRow row in dtSource.Rows)
            {
                Zone zone = new Zone();
                zone.ID = Convert.ToInt32(row["ID"]);
                zone.Name = row["Name"].ToString();
                zone.Group = row["Group"].ToString();
                zone.Relation = row["Relation"].ToString();
                zone.Category = row["Category"].ToString();
                zone.Color = row["Color"].ToString();
                zone.Area = Convert.ToDouble(row["Area"]);
                zone.Width = Convert.ToDouble(row["Width"]);
                zone.Length = Convert.ToDouble(row["Length"]);
                zone.Height = Convert.ToDouble(row["Height"]);
                zone.Floor = Convert.ToInt32(row["Floor"]);
                zone.Ratio = Convert.ToDouble(row["Ratio"]);
                zone.Type = row["Type"].ToString();
                zones.Add(zone);
            }
            
        }
        public List<Zone> GetZones()
        {
            return ZonesList;
        }
    }  
}
    
    
    //public class Zone
    //{
    //    public int guid;
    //    public int ID { get; set; }
    //    public List<string> Names { get; set; }
    //    public List<Zone> Groups { get; set; }
    //    public List<Zone> Relation { get; set; }
    //    public string Category { get; set; }
    //    public double Area { get; set; }
    //    public double Width { get; set; }
    //    public double Length { get; set; }
    //    public double Height { get; set; }
    //    public double Level { get; set; }
    //    public double Ratio { get; set; }
    //    public string Type { get; set; }
    //    public string Color { get; set; }
    //}
    //List <Zone> zones = new List<Zone> ();
    //foreach (DataRow row in dtSource.Rows)
    //    {
    //    Zone Zone = new Zone();
    //    Zone.ID = Convert.ConvertToInt32(row[0]["ID"]);
    //   zone.Name = Row["Name"].ToString();




        
        //public Zone(int guid, List<string> Names, int ID, List<Zone> Groups, List <Zone> Relation, string Category, double Area, double Width, double Height, double Level, double Ratio, double Type, string Color, string type)
        //{

        //    this.guid = guid;
        //    this.Names = new List<string>();
        //    this.ID = ID;
        //    this.Groups = new List<Zone>();
        //    this.Relation = new List<Zone>();
        //    this.Category = Category;
        //    this.Area = Area;
        //    this.Width = Width;
        //    this.Height = Height;
        //    this.Level = Level;
        //    this.Ratio = Ratio;
        //    this.Type = type;
        //    this.Color = Color;

           
        //}
       

        //    public List<Zone> VLinkedZone = new List<Zone>(); //Vertically linked zones
        //    public List<Zone> HLinkedZone = new List<Zone>(); //Horizontally linked zones
        //    private List<string> groups;
        //// public RectD RectD = new RectD();






