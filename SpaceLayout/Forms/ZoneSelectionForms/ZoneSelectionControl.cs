using System;
using System.Runtime;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExcelDataReader;
using SpaceLayout.Entity;
using Nevron.Xml;
using Nevron.UI.WinForm.Controls;
using Nevron.Diagram.WinForm;
using Nevron.Diagram;
using Nevron.Diagram.DataStructures;
using Nevron.Diagram.Shapes;
using System.Xml;

using Nevron.Nov.Diagram.Editors;
using Nevron.Nov.Diagram.DrawingTools;
using Nevron.Diagram.Designer;


using Nevron.GraphicsCore;
using System.Drawing.Design;

using Nevron.Dom;
using Nevron.Diagram.Layout;
using SpaceLayout.Object;
using Nevron.Diagram.Batches;
using Nevron.Diagram.ThinWeb;
using Nevron.Diagram.Extensions;
using Nevron.Serialization;
using Nevron.Diagram.Filters;
using Nevron.Chart.Windows;
using Nevron.Chart;
using Nevron.UI;
using System.Xml.Serialization;

namespace SpaceLayout.Forms.ZoneForms
{
    public partial class ZoneSelectionControl : UserControl
    {
        static string DataSourceInputData = StaticCache.DataSourceBasicInfo;
        private DataTable dtSource;

        // public GraphControl chartControl { get; set; }
        public NDrawingView Ndv;
        public NDrawingDocument Ndd;
        public NGraph graph;
        private Dictionary<int, bool> rowNodecreated = new Dictionary<int, bool>();
        private NNode startnode;
        private NNode endnode;

        public NRectangleShape startNode = null;
        public NRectangleShape endNode = null;
        private NPersistencyManager persistencyManager;

        List<Connector_Main> connector;
        List<Zone_Main> zone;

        public ZoneSelectionControl()
        {
            InitializeComponent();
            this.persistencyManager = new NPersistencyManager();

            // drawing = (NDrawingDocument)persistencyManager.LoadDocumentFromFile("c:\\temp\\drawing1.ndx");

            this.Load += IS_Load;

            connector = new List<Connector_Main>();
            zone = new List<Zone_Main>();

            // Add the diagram view to the form
            this.Controls.Add(Ndv);

            //chartControl.ItemClick += chartControl_MouseClick;

        }

        private void ChartControl_NodeCreated(NNodeEventArgs args)
        {
            Console.WriteLine(args.Node.ToString());
        }

        private void IS_Load(object sender, EventArgs e)
        {
            BindGrid();

            Form f = this.ParentForm;
            Ndv = f.Controls.Find("nDrawingView1", true).FirstOrDefault() as NDrawingView;
            if (Ndv != null)
            {
                Ndd = Ndv.Document;
            }
            //Ndv.EventSinkService.NodeMouseDown += EventSinkService_NodeMouseDown;
            //chartControl.Controller.Selection.Changed += chartControl_SelectionChanged;
            //chartControl.Controller.DoubleClick += chartControl_DoubleClick;
            //chartControl.Document.NodeCreated += ChartControl_NodeCreated;
            //chartControl.Document.UndoRedoService.Pause();
            //chartControl.Document.UndoRedoService.ChangesRegistry.Clear();
            //chartControl.Document.UndoRedoService.Resume();
        }

        //private void chartcontrol_MouseDoubleClick(object sender, MouseEventArgs e)
        //{
        //    foreach (var node in chartControl.Selection.SelectedNodes) { 

        //    Console.WriteLine("-----"+node.ToString());


        //    }
        //}

        //private void graphcontrol_SelectionChanged(object sender, EventArgs e)
        //{
        //    // Get the selected nodes
        //    Console.WriteLine(sender.ToString());
        //    Console.WriteLine(e.ToString());
        // //   GraphSelection graphControl = (GraphSelection)sender;
        //  //  IEnumerable<INode> selectedNodes = graphControl.Selection.SelectedNodes;

        //}
        //public class Selection
        //{
        //    public HashSet<INode> Nodes { get; } = new HashSet<INode>();
        //    public HashSet<IEdge> Edges { get; } = new HashSet<IEdge>();
        //}

        //private void Graph_EdgeCreated(object sender, yWorks.Utils.ItemEventArgs<IEdge> e)
        //{
        //   // Connector zone linking


        //}

        private void BindGrid()
        {
            //Check required excel is exists or not
            if (!File.Exists(DataSourceInputData))
            {
                MessageBox.Show("Please make and configure a setting to initialize dbsource file having path " + DataSourceInputData + "." + Environment.NewLine + "Source File have been put at Project's Datasource Folder.");
                return;
            }
            dtSource = new DataTable();
            dtSource.Columns.Add("ID");
            dtSource.Columns.Add("Name");
            // dtSource.Columns.Add("Department");
            dtSource.Columns.Add("Group");
            dtSource.Columns.Add("Relation");
            dtSource.Columns.Add("Category");
            dtSource.Columns.Add("Color");
            dtSource.Columns.Add("Area");
            dtSource.Columns.Add("Width");
            dtSource.Columns.Add("Length");
            dtSource.Columns.Add("Height");
            dtSource.Columns.Add("Floor");
            dtSource.Columns.Add("Ratio");
            dtSource.Columns.Add("Type");

            //List<InputData_ModuleEntity> result;
            //List<string> temp;
            using (var stream = File.Open(DataSourceInputData, FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)

                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // Choose one of either 1 or 2:

                    // 1. Use the reader methods
                    do
                    {
                        while (reader.Read())
                        {
                            // reader.GetDouble(0);
                        }
                    } while (reader.NextResult());

                    // 2. Use the AsDataSet extension method
                    var ds = reader.AsDataSet();

                    // The result of each spreadsheet is in ds.Tables
                    for (int i = 0; i < 2; i++)
                    {
                        ds.Tables["Input Data_Module"].Rows.RemoveAt(i);
                    }

                    ds.Tables["Input Data_Module"].Rows.RemoveAt(0);
                    ds.Tables["Input Data_Module"].Columns.Remove("Column2");
                    ds.Tables["Input Data_Module"].Columns.Remove("Column14");
                    ds.Tables["Input Data_Module"].Columns.Remove("Column15");

                    foreach (DataRow dr in ds.Tables["Input Data_Module"].Rows)
                    {
                        dtSource.Rows.Add(dr.ItemArray);
                    }
                }
            }

            //Bind datatable to Gridview
            if (dtSource.Rows.Count > 0)
            {
                dataGridView1.DataSource = dtSource;

                // Form MainFirstPage = new MainFirstPageControl(dtSource);
                //CreateNodes(dtSource);
            }
        }

        public DataTable GetDataSource()
        {
            return dtSource;
        }
        private Dictionary<int, bool> rowNodeCreated = new Dictionary<int, bool>();
        private NDrawingDocument drawing;


        //public NDrawingDocument Drawing { get => drawing; set => drawing = value; }


        //private void chartControl_MouseClick(object sender, Nevron.Diagram.WinForm.NMouseEventArgs e)
        //{
        //NRectangleShape clickedNode = Ndv.HitTest(e.Location);

        //    if (clickedNode != null && clickedNode is NRectangleShape shapeNode)
        //    {
        //        if (SourceNode == null)
        //        {
        //            SourceNode = shapeNode;
        //        }
        //        else if (TargetNode == null)
        //        {
        //            TargetNode = shapeNode;

        //            NLineShape edge = new NLineShape();
        //            //chartControl.Document..AddChild(edge);
        //            edge.FromShape = SourceNode;
        //            edge.ToShape = TargetNode;

        //            SourceNode = null;
        //            TargetNode = null;
        //        }
        //    }
        //}

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid")
            {
                object value = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (!((DataGridViewComboBoxColumn)dataGridView1.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    ((DataGridViewComboBoxColumn)dataGridView1.Columns[e.ColumnIndex]).Items.Add(value);
                    e.ThrowException = false;
                }
            }
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex != -1)
                {
                    string columnName = this.dataGridView1.Columns[e.ColumnIndex].Name;
                    if ((columnName == "Column1") && (!string.IsNullOrWhiteSpace(this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())))
                    {
                        dataGridView1.ReadOnly = true;

                        if (!rowNodeCreated.ContainsKey(e.RowIndex) || !rowNodeCreated[e.RowIndex])
                        {

                            Ndv.BeginInit();
                            Ndd.BeginInit();




                            NLayer activeLayer = Ndd.ActiveLayer;
                            NGraph graph = new NGraph();
                            var rand = new Random();
                            Zone zone = new Zone();
                            float width = (float)Math.Sqrt(Convert.ToDouble(this.dataGridView1.Rows[e.RowIndex].Cells["Column6"].Value) * 2);
                            float height = (float)(Convert.ToDouble(dataGridView1.CurrentRow.Cells["Column6"].Value) / width);







                            NRectangleF rect = new NRectangleF(0, 0, width, height);
                            NRectangleShape node = new NRectangleShape(rect);
                            node.Tag = zone;
                            node.Id = zone.ID;
                            node.Style = new NStyle
                            {
                                FillStyle = new NColorFillStyle(Color.FromName(this.dataGridView1.Rows[e.RowIndex].Cells["Column13"].Value.ToString())),
                            };

                            string NodeLabelIn = this.dataGridView1.Rows[e.RowIndex].Cells["Column1"].Value.ToString()
                                + System.Environment.NewLine + this.dataGridView1.Rows[e.RowIndex].Cells["Column6"].Value.ToString() + " " + "m\u00b2";
                            //graphcontrol.Graph.AddLabel(node, NodeLabelIn, InteriorLabelModel.NorthWest, defaultLableStyle, new SizeD(width, height));
                            string NodeLabelOut = this.dataGridView1.Rows[e.RowIndex].Cells["Column2"].Value.ToString();
                            //graphcontrol.Graph.AddLabel(node, NodeLabelOut, ExteriorLabelModel.South, defaultLableStyle);
                            node.Text = NodeLabelIn;


                            //node.Ports.GetChildByName("Bottom");
                            //Nevron.Chart.NLabel l = new Nevron.Chart.NLabel();
                            //l.Text = NodeLabelOut;
                            //l.Bounds = new NRectangleF((float)(Convert.ToInt32(node.Bounds.LeftBottom) - 20), (float)(Convert.ToInt32(node.Bounds.RightTop) - 0), 140, 20);
                            //node.Labels.AddChild(l);
                            //Create an NLabel
                            // NLabel label = new NLabel("Hello, world!");
                            //NCustomRangeLabel customRangeLabel = new NCustomRangeLabel();

                            //graph.Nodes.Add(node);
                            // node.Labels


                            //Random rnd = new Random();

                            //int x = rnd.Next((int)Ndv.ViewportOrigin.X, (int)Ndv.ViewportOrigin.X + (int)Ndv.ViewportSize.Width - (int)rect.Width);
                            //int y = rnd.Next((int)Ndv.ViewportOrigin.Y, (int)Ndv.ViewportOrigin.Y + (int)Ndv.ViewportSize.Height - (int)rect.Height);
                            //node.Location = new NPointF(x, y);

                            int maxWidth = Convert.ToInt32(Ndv.Document.Width);
                            int maxHeight = Convert.ToInt32(Ndv.Document.Height);

                            // Generate random position and size for the shape
                            Random rnd = new Random();
                            int x = rnd.Next(maxWidth);
                            int y = rnd.Next(maxHeight);
                            int w = rnd.Next(50, 150);
                            int h = rnd.Next(50, 150);

                            // Ensure that the shape bounds are within the view bounds
                            if (x + width > maxWidth)
                            {
                                x = maxWidth - w;
                            }
                            if (y + height > maxHeight)
                            {
                                y = maxHeight - h;
                            }

                            node.Location = new NPointF(x, y);
                            node.CreateShapeElements(ShapeElementsMask.Ports);
                            NRotatedBoundsPort port1 = new NRotatedBoundsPort(node.UniqueId, ContentAlignment.TopCenter);
                            NRotatedBoundsPort port2 = new NRotatedBoundsPort(node.UniqueId, ContentAlignment.MiddleLeft);
                            NRotatedBoundsPort port3 = new NRotatedBoundsPort(node.UniqueId, ContentAlignment.BottomCenter);
                            NRotatedBoundsPort port4 = new NRotatedBoundsPort(node.UniqueId, ContentAlignment.MiddleRight);
                            node.Ports.AddChild(port1);
                            node.Ports.AddChild(port2);
                            node.Ports.AddChild(port3);
                            node.Ports.AddChild(port4);
                            node.Ports.DefaultInwardPortUniqueId = port1.UniqueId;
                            node.Ports.DefaultInwardPortUniqueId = port2.UniqueId;
                            node.Ports.DefaultInwardPortUniqueId = port3.UniqueId;
                            node.Ports.DefaultInwardPortUniqueId = port4.UniqueId;
                            //NPortCollection ports = new NPortCollection();
                            //node.Ports.AddChild(ports);

                            //NPort InPort = node.Ports.DefaultInwardPort as NPort;
                            // NPort OutPort = node.Ports.DefaultOutwardPort as NPort;
                            //Guid guid = Guid.NewGuid();
                            //NPort port = new NPort(guid);
                            //port.Name = guid.ToString();
                            //ports.AddChild(port);


                            //node.Ports.po
                            activeLayer.AddChild(node);

                            //if (startNode == null)
                            //{
                            //    startNode = node;
                            //}
                            //else if (endNode == null)
                            //{
                            //    endNode = node;
                            //    NLineShape connector = new NLineShape(startNode.PinPoint, endNode.PinPoint);
                            //    connector.Style = new NStyle();
                            //    connector.Style.StrokeStyle = new NStrokeStyle(2, Color.Black);
                            //    activeLayer.AddChild(connector);
                            //    startNode = null;
                            //    endNode = null;
                            //}

                            //double maxX = graphcontrol.ClientSize.Width - width;
                            //double maxY = graphcontrol.ClientSize.Height - height;
                            //double x = rand.NextDouble() * maxX;
                            //double y = rand.NextDouble() * maxY;
                            //graphcontrol.Graph.SetNodeCenter(node, new PointD(x, y));

                            //graphcontrol.FitGraphBounds();
                            rowNodeCreated[e.RowIndex] = true; //make it true to avoid duplicate node
                            Ndv.EndInit();
                            Ndd.EndInit();

                            //Ndv.Document.Click += OnItemClicked;

                        }
                        else
                        {
                            MessageBox.Show("Zone already created for this row.");
                        }

                        {

                            //GraphEditorInputMode graphEditorInputMode = new GraphEditorInputMode();
                            //graphEditorInputMode.AllowCreateNode = false; // restrict node creation by clicking in UI
                            //graphEditorInputMode.CreateEdgeInputMode.Enabled = true;
                            // Ndv.InputMode = graphEditorInputMode;
                            //graphcontrol.InputMode = graphEditorInputMode;
                            //graphcontrol.Graph.EdgeDefaults.Style = edgeStyle;
                            //graphcontrol.FitGraphBounds();
                            // Ndv.MouseClick += OnItemClicked;
                            //Ndv.EventSinkService.NodeMouseDown += EventSinkService_NodeMouseDown;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ZoneConnectorData(string flg) //flg: 1 = save, 2 =  load
        {
            Ndv.Refresh();
            if (flg == "1")
            {
                foreach (NLineShape edge in Ndv.Document.Descendants(NFilters.Shape1D, -1))
                {
                    // Check if the shape is a line and has a source and target shape
                    if (edge is NLineShape line && line.FromShape != null && line.ToShape != null)
                    {
                        if (line.StartPlug.Shape.FromShape is NRectangleShape && line.EndPlug.Shape.ToShape is NRectangleShape)
                        {
                            DataRow drFrom = GetShapeDataFromDataSource((NRectangleShape)line.FromShape);
                            DataRow drTo = GetShapeDataFromDataSource((NRectangleShape)line.ToShape);
                            Zone_Main z1 = GetZoneData(drFrom);
                            Zone_Main z2 = GetZoneData(drTo);

                            connector.Add(GetConnectorData(z1, z2, line));
                        }
                    }
                }

                foreach (NRectangleShape shape in Ndv.Document.Descendants(NFilters.Shape2D, -1))
                {
                    DataRow dr = GetShapeDataFromDataSource(shape);
                    if (dr != null)
                    {
                        zone.Add(GetZoneData(dr));
                    }
                }

            }
            else if (flg == "2")
            {
                foreach (NLineShape edge in Ndv.Document.Descendants(NFilters.Shape1D, -1))
                {
                    // Check if the shape is a line and has a source and target shape
                    if (edge is NLineShape line && line.FromShape != null && line.ToShape != null)
                    {
                        if (line.FromShape is NRectangleShape && line.ToShape is NRectangleShape)
                        {
                            DataRow drFrom = GetShapeDataFromDataSource((NRectangleShape)line.FromShape);
                            DataRow drTo = GetShapeDataFromDataSource((NRectangleShape)line.ToShape);
                            Zone_Main z1 = GetZoneData(drFrom);
                            Zone_Main z2 = GetZoneData(drTo);

                            connector.Add(GetConnectorData(z1, z2, line));
                        }
                    }
                }

                foreach (NRectangleShape shape in Ndv.Document.Descendants(NFilters.Shape2D, -1))
                {
                    DataRow dr = GetShapeDataFromDataSource(shape);
                    if (dr != null)
                    {
                        zone.Add(GetZoneData(dr));
                    }
                }
            }
            else
            {

            }
        }

        private DataRow GetShapeDataFromDataSource(NRectangleShape shape)
        {
            NShape a = shape.FromShape;
            string[] NodeText = shape.Text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            DataRow row = dtSource.AsEnumerable().Where(r => r.Field<string>("ID") == NodeText[0].ToString()).SingleOrDefault();
            return row;
        }
        private Zone_Main GetZoneData(DataRow dr)
        {
            List<string> EndNodes = new List<string>();
            string relation = string.Empty;
            foreach (var data in connector)
            {
                if (data.Zone21 != null)
                {
                    if (Convert.ToInt32(dr[0]) == data.Zone21.ID)
                        EndNodes.Add(data.Zone22.ID.ToString());
                }
                
            }
            relation = String.Join(",", EndNodes.ToArray());

            Zone_Main zone_data = new Zone_Main(Convert.ToInt32(dr[0])
                         , dr[1].ToString()
                         , dr[2].ToString()
                         , relation
                         , dr[4].ToString()
                         , dr[5].ToString()
                         , dr[6] is DBNull ? 0 : Convert.ToDouble(dr[6])
                         , dr[7] is DBNull ? 0 : Convert.ToDouble(dr[7])
                         , dr[8] is DBNull ? 0 : Convert.ToDouble(dr[8])
                         , dr[9] is DBNull ? 0 : Convert.ToDouble(dr[9])
                         , Convert.ToInt32(dr[10])
                         , dr[11] is DBNull ? 0 : Convert.ToDouble(dr[11])
                         , dr[12].ToString());

            return zone_data;
        }

        private Connector_Main GetConnectorData(Zone_Main z1, Zone_Main z2, NLineShape line)
        {
            Connector_Main connector_data = new Connector_Main(z1
                               , z2
                               , 0 //put zero for temporary 
                               , Convert.ToDouble(line.Length)
                               , line.StyleSheet.Style.StrokeStyle.Color
                               );

            return connector_data;
        }

        //savebutton
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {

                //CreateXMLNodes1();

                ZoneConnectorData("1");
                NPersistentDocument document = new NPersistentDocument("My document");
                // NPersistentSection documentSection = new NPersistentSection("DrawingDocument", Ndd);
                //document.Sections.Add(documentSection);

                // Add the drawing document and the drawing view to the section
                NPersistentSection documentSection = new NPersistentSection("DrawingDocument", Ndd);
                document.Sections.Add(documentSection);

                NPersistentSection nodesSection = new NPersistentSection("Graph", null);
                document.Sections.Add(nodesSection);

                //NPersistentSection graphsection = new NPersistentSection("Graph", null);
                //document.Sections.Add(graphsection);

                // create a new section which will store the rect
                // document.Sections.Add(new NPersistentSection("Rectangle", rect));

                // set the document to the manager
                persistencyManager.PersistentDocument = document;


                // save the document to a file
                persistencyManager.SaveToFile("c:\\temp\\mysavefile.cndx", PersistencyFormat.CustomXML, null);
                AppendXMLNodes();
                MessageBox.Show("Save successful");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {
            //testing for load fucntion
            try
            {
                //Ndd = new NDrawingDocument();
                // Create a drawing document
                NDrawingDocument drawing = new NDrawingDocument();

                // create a new persistency manager
                NPersistencyManager persistencyManager = new NPersistencyManager();

                // load a drawing from the XML file
                drawing = persistencyManager.LoadDrawingFromFile("c:\\temp\\mysavefile.cndx");

                // display the drawing
                Ndv.Document = drawing;

                ZoneConnectorData("2");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            // // Create the connector shape

            // NConnectorShape connector = new NConnectorShape();
            //Tool
            // // Set the connector style
            // connector.StyleSheetName = "MyConnectorStyle";
            // connector.StyleSheet.Apply(connector);

            // // Connect the nodes
            // connector.StartPlug.Connect(node1.Ports.GetChildAt(0) as NPort);
            // connector.EndPlug.Connect(node2.Ports.GetChildAt(0) as NPort);

            // // Add the connector to the active layer
            // diagramControl.Document.ActiveLayer.AddChild(connector);

            // // Refresh the view
            // diagramControl.Refresh();

        }


        private void AppendXMLNodes()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("c:\\temp\\mysavefile.cndx");

            XmlElement zroot = xmlDoc.CreateElement("Zone");

            xmlDoc.DocumentElement.AppendChild(zroot);

            XmlElement croot = xmlDoc.CreateElement("Connector");

            xmlDoc.DocumentElement.AppendChild(croot);

            foreach (var result in zone)
            {
                XmlElement ZoneID = xmlDoc.CreateElement("ZoneID");
                ZoneID.InnerText = result.ID.ToString();
                zroot.AppendChild(ZoneID);

                XmlElement ZoneName = xmlDoc.CreateElement("Name");
                ZoneName.InnerText = result.Name.ToString();

                XmlElement Group = xmlDoc.CreateElement("Group");
                Group.InnerText = result.Group.ToString();

                XmlElement Relation = xmlDoc.CreateElement("Relation");
                Relation.InnerText = result.Relation.ToString();

                XmlElement Category = xmlDoc.CreateElement("Category");
                Category.InnerText = result.Category.ToString();

                XmlElement Color = xmlDoc.CreateElement("Color");
                Color.InnerText = result.Color.ToString();

                XmlElement Area = xmlDoc.CreateElement("Area");
                Area.InnerText = result.Area.ToString();

                XmlElement Width = xmlDoc.CreateElement("Width");
                Width.InnerText = result.Width.ToString();

                XmlElement Length = xmlDoc.CreateElement("Length");
                Length.InnerText = result.Length.ToString();

                XmlElement Height = xmlDoc.CreateElement("Height");
                Height.InnerText = result.Height.ToString();

                XmlElement Floor = xmlDoc.CreateElement("Floor");
                Floor.InnerText = result.Floor.ToString();

                XmlElement Ratio = xmlDoc.CreateElement("Ratio");
                Ratio.InnerText = result.Ratio.ToString();

                XmlElement Type = xmlDoc.CreateElement("Type");
                Type.InnerText = result.Type.ToString();


                ZoneID.AppendChild(ZoneName);
                ZoneID.AppendChild(Group);
                ZoneID.AppendChild(Relation);
                ZoneID.AppendChild(Category);
                ZoneID.AppendChild(Color);
                ZoneID.AppendChild(Area);
                ZoneID.AppendChild(Width);
                ZoneID.AppendChild(Length);
                ZoneID.AppendChild(Height);
                ZoneID.AppendChild(Floor);
                ZoneID.AppendChild(Ratio);
                ZoneID.AppendChild(Type);
            }

            //foreach (var result in connector)
            //{
            //    // Create a new XML element for the connector
            //    XmlElement connectorElement = xmlDoc.CreateElement("Connector");

            //    // Add the ID and type attributes to the connector element
            //    connectorElement.SetAttribute("ID", connector.ID.ToString());
            //    connectorElement.SetAttribute("Type", connector.Type.ToString());

            //    // Add the start and end zone elements to the connector element
            //    XmlElement startZoneElement = xmlDoc.CreateElement("StartZone");
            //    startZoneElement.SetAttribute("ID", connector.Zone21.ID.ToString());
            //    startZoneElement.SetAttribute("Name", connector.Zone21.Name);
            //    connectorElement.AppendChild(startZoneElement);

            //    XmlElement endZoneElement = xmlDoc.CreateElement("EndZone");
            //    endZoneElement.SetAttribute("ID", connector.Zone22.ID.ToString());
            //    endZoneElement.SetAttribute("Name", connector.Zone22.Name);
            //    connectorElement.AppendChild(endZoneElement);

            //    // Add the length and color elements to the connector element
            //    XmlElement lengthElement = xmlDoc.CreateElement("Length");
            //    lengthElement.InnerText = connector.Length.ToString();
            //    connectorElement.AppendChild(lengthElement);

            //    XmlElement colorElement = xmlDoc.CreateElement("Color");
            //    colorElement.InnerText = connector.Color.ToString();
            //    connectorElement.AppendChild(colorElement);

            //    // Add the connector element to the XML file
            //    croot.AppendChild(connectorElement);
            //}
            xmlDoc.Save("c:\\temp\\mysavefile.cndx");
            foreach (var result in connector)
            {
                //XmlElement Zone = xmlDoc.CreateElement("ZoneID");
                //ZoneID.InnerText = result.Zone21.ID.ToString();
                //zroot.AppendChild(ZoneID);

                //XmlElement ZoneName = xmlDoc.CreateElement("Name");
                //ZoneName.InnerText = result.Zone21.Name.ToString();

                //    XmlElement Group = xmlDoc.CreateElement("Group");
                //    Group.InnerText = result.Group.ToString();

                XmlElement Relation = xmlDoc.CreateElement("Relation");
                Relation.InnerText = result.Zone21.ToString();

                //    XmlElement Category = xmlDoc.CreateElement("Category");
                //    Category.InnerText = result.Category.ToString();

                //    XmlElement Color = xmlDoc.CreateElement("Color");
                //    Color.InnerText = result.Color.ToString();

                //    XmlElement Area = xmlDoc.CreateElement("Area");
                //    Area.InnerText = result.Area.ToString();

                //    XmlElement Width = xmlDoc.CreateElement("Width");
                //    Width.InnerText = result.Width.ToString();

                //    XmlElement Length = xmlDoc.CreateElement("Length");
                //    Length.InnerText = result.Length.ToString();

                //    XmlElement Height = xmlDoc.CreateElement("Height");
                //    Height.InnerText = result.Height.ToString();

                //    XmlElement Floor = xmlDoc.CreateElement("Floor");
                //    Floor.InnerText = result.Floor.ToString();

                //    XmlElement Ratio = xmlDoc.CreateElement("Ratio");
                //    Ratio.InnerText = result.Ratio.ToString();

                //    XmlElement Type = xmlDoc.CreateElement("Type");
                //    Type.InnerText = result.Type.ToString();

                xmlDoc.Save("c:\\temp\\mysavefile.cndx");
                //    ZoneID.AppendChild(ZoneName);
                //    ZoneID.AppendChild(Group);
                //    ZoneID.AppendChild(Relation);
                //    ZoneID.AppendChild(Category);
                //    ZoneID.AppendChild(Color);
                //    ZoneID.AppendChild(Area);
                //    ZoneID.AppendChild(Width);
                //    ZoneID.AppendChild(Length);
                //    ZoneID.AppendChild(Height);
                //    ZoneID.AppendChild(Floor);
                //    ZoneID.AppendChild(Ratio);
                //    ZoneID.AppendChild(Type);
                //}



            }
            //private void CreateXMLNodes1()
            //{
            //    // Create an XML document
            //    XmlDocument xmlDoc = new XmlDocument();

            //    // Create the root element
            //    XmlElement root = xmlDoc.CreateElement("Zone");
            //    xmlDoc.AppendChild(root);

            //    // Create a child element with an attribute
            //    XmlElement child = xmlDoc.CreateElement("1");
            //    XmlAttribute attr = xmlDoc.CreateAttribute("Name");
            //    attr.Value = "staff study,office";
            //    child.Attributes.Append(attr);

            //    XmlAttribute attr2 = xmlDoc.CreateAttribute("Area");
            //    attr2.Value = "3983.44";
            //    child.Attributes.Append(attr2);

            //    // Add the child element to the root element
            //    root.AppendChild(child);
            //}


            //{
            //   Ndd= persistencyManager.LoadDrawingFromFile( "c:\\temp\\drawing1.ndx");
            //   Ndv.Document = drawing;
            //    //MessageBox.Show("Import");
        }


    }
}
    
