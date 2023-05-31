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
using Nevron.Dom;
using Nevron.Diagram.Filters;
using Nevron.Diagram.Batches;
using Nevron.Diagram.Shapes;
using System.Xml;
using Nevron.Diagram.Templates;
using Nevron.Diagram.WinForm.Commands;
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
using Nevron.Diagram.Layout;
using NSelection = Nevron.Diagram.WinForm.NSelection;

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
        NGroup group = new NGroup();

        public NRectangleShape startNode = null;
        public NRectangleShape endNode = null;
        private NPersistencyManager persistencyManager;
        private NGroupBox groupPropertiesGroup;
        private NCheckBox autoDestroyCheckBox;
        private NCheckBox canBeEmptyCheckBox;
        List<Connector_Main> connector;
        List<Zone_Main> zone;
      
        List<NGroup> existingGroups = new List<NGroup>();

        public ZoneSelectionControl()
        {
            InitializeComponent();
            this.persistencyManager = new NPersistencyManager();
            this.groupPropertiesGroup = new NGroupBox();
            this.autoDestroyCheckBox = new NCheckBox();
            this.canBeEmptyCheckBox = new NCheckBox(); this.groupPropertiesGroup.SuspendLayout();
            this.SuspendLayout();

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
            Form f = this.ParentForm;
            Ndv = f.Controls.Find("nDrawingView1", true).FirstOrDefault() as NDrawingView;
            if (Ndv != null)
            {
                Ndd = Ndv.Document;
            }
            BindGrid();
        }

        

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
            //dtSource.Columns.Add("Department");
            dtSource.Columns.Add("Group");
            dtSource.Columns.Add("GroupColor");
            dtSource.Columns.Add("Relation");
            dtSource.Columns.Add("GroupArea");
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
                MainFunction(dtSource);
                // Form MainFirstPage = new MainFirstPageControl(dtSource);
                //CreateNodes(dtSource);
            }
        }

        public void MainFunction(DataTable dtMain)
        {
            try
            {
                List<NGroup> existingGroups = new List<NGroup>();
                NGroup maingp = new NGroup();
                string groupname = string.Empty;
                foreach (DataRow dr in dtMain.Rows)
                {
                    if (groupname != dr["Group"].ToString())
                    {
                        groupname = dr["Group"].ToString();
                        if (!String.IsNullOrEmpty(groupname))
                        {
                            NGroup existingGroup = existingGroups.FirstOrDefault(g => g.Name == groupname);
                            if (existingGroup != null)
                            {
                                AddRowsToGroup(existingGroup, dtMain.Select("Group = '" + groupname+ "'"));
                            }
                            else
                            {
                                DataTable dtGroup = dtMain.Select("Group = '" + groupname + "'").CopyToDataTable();
                                if (dtGroup.Rows.Count > 0)
                                {
                                    NGroup newGroup = GetGroup(dtGroup, existingGroups);
                                    newGroup.Name = groupname;
                                    existingGroups.Add(newGroup);
                                    // Create a new list for existing groups
                                    maingp = GetGroup(dtGroup, existingGroups);
                                    maingp.Name = groupname;
                                    Ndd.ActiveLayer.AddChild(newGroup);
                                }
                            }
                        
                            

                        }
                    }
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
           
        }
        private void AddRowsToGroup(NGroup group, DataRow[] rows)
        {
            // Add the DataTable rows to the existing group
            foreach (DataRow row in rows)
            {
                // Add your logic to process and add rows to the group
            }
        }

        private NGroup GetGroup(DataTable dtGroup, List<NGroup> existingGroups)//ForGroup
        {
           

            float width = (float)Math.Sqrt(Convert.ToDouble(dtGroup.Rows[0]["GroupArea"].ToString()) * 2) * (float)2.5;
                float height = (float)(Convert.ToDouble(dtGroup.Rows[0]["GroupArea"].ToString()) / width) * (float)4;
                Random random = new Random();

            float x = (float)random.NextDouble() * Ndv.Document.Bounds.X;
            float y = (float)random.NextDouble() * Ndv.Document.Bounds.Y;


            Color color1 = Color.FromName(dtGroup.Rows[0]["GroupColor"].ToString());
                Color color2 = Color.Black;

                NGroup group = new NGroup();
                NRectangleF bounds = new NRectangleF(x, y, width, height);
                NRectangleShape frame = new NRectangleShape(bounds);
                frame.Protection = new NAbilities(AbilitiesMask.Select);
                List<NRectangleShape> zones = GetShape(bounds, dtGroup);
                foreach (NRectangleShape r in zones)
                {
                    group.Shapes.AddChild(r);
                }
                CreateDecorators(frame, dtGroup.Rows[0]["Group"].ToString());
                group.UpdateModelBounds();

                frame.Style.FillStyle = new NColorFillStyle(color1);
                group.Shapes.AddChild(frame);
                CreateGroupPorts(frame);
                group.Style.FillStyle = new NColorFillStyle(color2);
                group.Style.StrokeStyle = new NStrokeStyle(color2);
                frame.SendToBack();

            bool overlapping = false;
            foreach (NGroup existingGroup in existingGroups)
            {
                if (CheckOverlap(group, existingGroup))
                {
                    overlapping = true;
                        x = (float)random.NextDouble() * Ndv.Document.Bounds.X;
                    y = (float)random.NextDouble() * Ndv.Document.Bounds.Y;
                    bounds = new NRectangleF(x, y, width, height);
                    frame.Bounds = bounds;
                    group.UpdateModelBounds();
                    break;
                }
            }

            // If overlapping, recursively call the GetGroup method again
            if (overlapping)
            {
                return GetGroup(dtGroup, existingGroups);
            }
            else
            {

                return group;
            }


        }
        private bool CheckOverlap(NGroup group1, NGroup group2)
        {
            return group1.Bounds.IntersectsWith(group2.Bounds);
        }

        private List<NRectangleShape> GetShape(NRectangleF bounds,DataTable dtGroup)
        {
            List<NRectangleShape> zones = new List<NRectangleShape>();
            foreach (DataRow dr in dtGroup.Rows)
            {
                NRectangleShape zone = new NRectangleShape();
                float width = (float)Math.Sqrt(Convert.ToDouble(dr[7].ToString()) * 2);
                float height = (float)(Convert.ToDouble(dr[7].ToString()) / width);
                Random rnd = new Random();
                int maxWidth = Convert.ToInt32(Math.Sqrt(Convert.ToDouble(dr[5].ToString()) * 2));
                int maxHeight = Convert.ToInt32(Convert.ToDouble(dr[5].ToString()) / maxWidth);

                // Generate random position and size for the shape

                //int x = rnd.Next(0, maxWidth);
                //int y = rnd.Next(0, maxHeight);
                float x = (float)rnd.NextDouble() * maxWidth;
                float y = (float)rnd.NextDouble() * maxHeight;
                //int w = rnd.Next(20, 40);
                //int h = rnd.Next(20, 40);

                //// Ensure that the shape bounds are within the view bounds
                //if (x + width > maxWidth)
                //{
                //    x = maxWidth - w;
                //}
                //if (y + height > maxHeight)
                //{
                //    y = maxHeight - h;
                //}

                string NodeLabelIn = dr[0].ToString()
                   + System.Environment.NewLine + dr[7].ToString() + " " + "m\u00b2";
                string NodeLabelOut = dr[1].ToString();
                zone.Text = NodeLabelIn;
                Color color1 = Color.FromName(dr[6].ToString());
                Color color2 = Color.Black;

                zone = new NRectangleShape(bounds.X + 10 , bounds.Y + 10 , width, height);
                zone.Style.FillStyle = new NColorFillStyle(color1);
                zone.Style.StrokeStyle = new NStrokeStyle(color2);
                //zone.Location = new NPointF(x, y);
                zone.CreateShapeElements(ShapeElementsMask.Ports);
                NRotatedBoundsPort port1 = new NRotatedBoundsPort(zone.UniqueId, ContentAlignment.TopCenter);
                NRotatedBoundsPort port2 = new NRotatedBoundsPort(zone.UniqueId, ContentAlignment.MiddleLeft);
                NRotatedBoundsPort port3 = new NRotatedBoundsPort(zone.UniqueId, ContentAlignment.BottomCenter);
                NRotatedBoundsPort port4 = new NRotatedBoundsPort(zone.UniqueId, ContentAlignment.MiddleRight);
                zone.Ports.AddChild(port1);
                zone.Ports.AddChild(port2);
                zone.Ports.AddChild(port4);
                zone.Ports.AddChild(port3);
                zone.Ports.DefaultInwardPortUniqueId = port1.UniqueId;
                zone.Ports.DefaultInwardPortUniqueId = port2.UniqueId;
                zone.Ports.DefaultInwardPortUniqueId = port3.UniqueId;
                zone.Ports.DefaultInwardPortUniqueId = port4.UniqueId;
                
                zones.Add(zone);
            }
            return zones;
        }

        private Dictionary<int, bool> rowNodeCreated = new Dictionary<int, bool>();
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
        private void UpdateControlsState()
       {
            if (Ndv.Selection.NodesCount != 1)
            {
                groupPropertiesGroup.Enabled = false;
                return;
            }
            NGroup group = (Ndv.Selection.AnchorNode as NGroup);
                 if (group == null)
                 {
                           groupPropertiesGroup.Enabled = false;
                            return;
                 }
                 groupPropertiesGroup.Enabled = true;
            canBeEmptyCheckBox.Checked = group.CanBeEmpty;
            autoDestroyCheckBox.Checked = group.AutoDestroy;
       }
        private void SelectNodesForGrouping(List<NShape> nodes)
        {
            // Assuming Ndd is the diagram instance and view is the view instance
            NSelection selection = Ndv.Selection;
            selection.Nodes.Clear();
            foreach (NShape shape in nodes)
            {
                selection.Nodes.Add(shape);
            }

            Ndv.Refresh();
        }


        private NGroup CreateGroup(DataTable dtData)
        {
            //float width = (float)Math.Sqrt(Convert.ToDouble(dtData.Rows[0]["GroupArea"].ToString()) * 2);
            //float height = (float)(Convert.ToDouble(dtData.Rows[0]["GroupArea"].ToString()) / width);
            
            NGroup group = new NGroup();
            group.Name = dtData.Rows[0]["Group"].ToString();
           
            List<NRectangleShape> lstshape = CreateShapes(dtData);
            foreach (NRectangleShape shape in lstshape)
            {
                group.Shapes.AddChild(shape);
            }
            //group.Shapes.AddChild(CreateShapes(dtData));
           string color = dtData.Rows[0]["GroupColor"].ToString();
            ////int maxWidth = Convert.ToInt32(Ndv.Document.Width);
            ////int maxHeight = Convert.ToInt32(Ndv.Document.Height);
            //float width = (float)Math.Sqrt(Convert.ToDouble(dtData.Rows[0]["GroupArea"].ToString()) * 2);
            //float height = (float)(Convert.ToDouble(dtData.Rows[0]["GroupArea"].ToString()) / width);
        
            CreateDecorators(group, group.Name);
            //CreateGroupPorts(group);
            //AddColorsAndRectangleShape(group, color, width, height);
            //CreateShape(group, dtData);
           // group.Style.FillStyle = new NColorFillStyle(Color.FromName(color));
            ////group.SetBounds(new NRectangleF(0, 0, width, height));

            //group.UpdateModelBounds();
            //group.AutoUpdateModelBounds = true;
            ////ApplyProtections(group, true, true);
            return group;
        }

        public List<NRectangleShape> CreateShapes(DataTable dtShape)
        {
            List<NRectangleShape> shapes = new List<NRectangleShape>();
            foreach(DataRow dr in dtShape.Rows)
            {
                var rand = new Random();
                //Zone zone = new Zone();
                float width = (float)Math.Sqrt(Convert.ToDouble(dr[7].ToString()) * 2);
                float height = (float)(Convert.ToDouble(dr[7].ToString()) / width);
                //NRectangleF rect = new NRectangleF(0, 0, width, height);
                NRectangleShape node = new NRectangleShape(0, 0, width, height);
                //node.Tag = zone;
                //node.Id = zone.ID;
                node.Style.FillStyle = new NColorFillStyle(Color.FromName(dr[6].ToString()));
                string NodeLabelIn = dr[0].ToString()
                    + System.Environment.NewLine + dr[7].ToString() + " " + "m\u00b2";
                //graphcontrol.Graph.AddLabel(node, NodeLabelIn, InteriorLabelModel.NorthWest, defaultLableStyle, new SizeD(width, height));
                string NodeLabelOut = dr[1].ToString();
                //graphcontrol.Graph.AddLabel(node, NodeLabelOut, ExteriorLabelModel.South, defaultLableStyle);
                node.Text = NodeLabelIn;

                int maxWidth = Convert.ToInt32(Math.Sqrt(Convert.ToDouble(dr[5].ToString()) * 2));
                int maxHeight = Convert.ToInt32(Convert.ToDouble(dr[5].ToString()) / maxWidth);

                // Generate random position and size for the shape
                Random rnd = new Random();
                int x = rnd.Next(0, maxWidth);
                int y = rnd.Next(0, maxHeight);
                //float x = (float)rnd.NextDouble() * maxWidth;
                // float y = (float)rnd.NextDouble() * maxHeight;
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

                shapes.Add(node);
            }
            return shapes;
        }
        
        private void CreateGroupPorts(NRectangleShape group)
        {
            group.CreateShapeElements(ShapeElementsMask.Ports);
            NRotatedBoundsPort port = new NRotatedBoundsPort(new NContentAlignment(ContentAlignment.TopCenter));
            NRotatedBoundsPort port1 = new NRotatedBoundsPort(new NContentAlignment(ContentAlignment.BottomCenter));
            NRotatedBoundsPort port2 = new NRotatedBoundsPort(new NContentAlignment(ContentAlignment.MiddleLeft));
            NRotatedBoundsPort port3 = new NRotatedBoundsPort(new NContentAlignment(ContentAlignment.MiddleRight));
            port.Name = "port";
            port1.Name = "port1";
            port2.Name = "port2";
            port3.Name = "port3";
            group.Ports.AddChild(port);
            group.Ports.AddChild(port1);
            group.Ports.AddChild(port2);
            group.Ports.AddChild(port3);
        }

        private void AddColorsAndRectangleShape(NGroup group, string color, float width, float height)
        {
            // Assuming you have predefined colors and rectangle shape dimensions
            NColorFillStyle Fill = new NColorFillStyle(Color.FromName(color));
            NStrokeStyle stroke = new NStrokeStyle(Color.Black);
            //float width = 100;
            //float height = 50;

            NRectangleShape rect = new NRectangleShape(0, 0, (float)width, (float)height);
            rect.Style.FillStyle = Fill;
            rect.Style.StrokeStyle = stroke;
            
            group.Shapes.AddChild(rect);
        }

        private void CreateDecorators(NShape shape, string decoratorText)
		{
			// Create the decorators
			shape.CreateShapeElements(ShapeElementsMask.Decorators);

			// Create a frame decorator
			// We want the user to be able to select the shape when the frame is hit
			NFrameDecorator frameDecorator = new NFrameDecorator();
			frameDecorator.ShapeHitTestable = true;
			frameDecorator.Header.Margins = new Nevron.Diagram.NMargins(20, 0, 0, 0);
			frameDecorator.Header.Text = decoratorText;
			shape.Decorators.AddChild(frameDecorator);

			// Create an expand/collapse decorator
			NExpandCollapseDecorator decorator = new NExpandCollapseDecorator();
			shape.Decorators.AddChild(decorator);
		}

        private void ApplyProtections(NShape shape, bool trackersEdit, bool move)
		{
			NAbilities protection = shape.Protection;
			protection.TrackersEdit = trackersEdit;
			protection.MoveX = move;
			protection.MoveY = move;
			shape.Protection = protection;
		}

        private void CreateShape(NGroup group, DataTable dtData)
        {
            NRectangleShape node;
            Random rnd = new Random();
            foreach (DataRow dr in dtData.Rows)
            {
                //var rand = new Random();
                //Zone zone = new Zone();
                float width = (float)Math.Sqrt(Convert.ToDouble(dr[7].ToString()) * 2);
                float height = (float)(Convert.ToDouble(dr[7].ToString()) / width);
                NRectangleF rect = new NRectangleF(0, 0, width, height);
                 node = new NRectangleShape(rect);
                //node.Tag = zone;
                //node.Id = zone.ID;
                node.Style.FillStyle = new NColorFillStyle(Color.FromName(dr[6].ToString()));
                string NodeLabelIn = dr[0].ToString()
                    + System.Environment.NewLine + dr[7].ToString() + " " + "m\u00b2";
                //graphcontrol.Graph.AddLabel(node, NodeLabelIn, InteriorLabelModel.NorthWest, defaultLableStyle, new SizeD(width, height));
                string NodeLabelOut = dr[1].ToString();
                //graphcontrol.Graph.AddLabel(node, NodeLabelOut, ExteriorLabelModel.South, defaultLableStyle);
                node.Text = NodeLabelIn;

                int maxWidth = Convert.ToInt32(Math.Sqrt(Convert.ToDouble(dr[5].ToString()) * 2));
                int maxHeight = Convert.ToInt32(Convert.ToDouble(dr[5].ToString()) / maxWidth);

                //int x = 0, y = 0,w =0, h= 0;

                //bool overlapping = true;
                //while (overlapping)
                //{
                //    x = rnd.Next(0, maxWidth);
                //    y = rnd.Next(0, maxHeight);
                //    w = rnd.Next(50, 150);
                //    h = rnd.Next(50, 150);


                //    node.Bounds = new NRectangleF(x, y, node.Bounds.Width, node.Bounds.Height);

                //    bool intersecting = false;
                //    // Check if the new shape intersects with any existing shape in the group
                //    foreach (var shape in group.Shapes)
                //    {

                //        if (shape is NRectangleShape existingShape && existingShape.Bounds.IntersectsWith(node.Bounds))

                //            intersecting = true;
                //            break;
                //    }
                //    // If no intersection, exit the loop
                //    if (!intersecting)
                //    {
                //        overlapping = false;
                //    }
                //}

                // Generate random position and size for the shape
               
                int x = rnd.Next(0, maxWidth);
                int y = rnd.Next(0, maxHeight);
                //float x = (float)rnd.NextDouble() * maxWidth;
                // float y = (float)rnd.NextDouble() * maxHeight;
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
                group.Shapes.AddChild(node);
            }
           
        }

        private void ZoneConnectorData(string flg) //flg: 1 = save, 2 =  load
        {
            //Ndv.Refresh();
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
                

                // Add the drawing document and the drawing view to the section
                NPersistentSection documentSection = new NPersistentSection("DrawingDocument", Ndd);
                document.Sections.Add(documentSection);

                NPersistentSection nodesSection = new NPersistentSection("Graph", null);
                document.Sections.Add(nodesSection);
                // set the document to the manager
                persistencyManager.PersistentDocument = document;


                // save the document to a file
                persistencyManager.SaveToFile("c:\\temp\\Drawingfile.cndx", PersistencyFormat.CustomXML, null);
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
                string FileName = string.Empty;
                OpenFileDialog ofd = new OpenFileDialog();

                ofd.InitialDirectory = @"C:\temp\";
                ofd.RestoreDirectory = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (!String.IsNullOrWhiteSpace(ofd.FileName) && Path.GetExtension(ofd.FileName) == ".cndx")
                    {
                        // Create a drawing document
                        NDrawingDocument drawing = new NDrawingDocument();

                        // create a new persistency manager
                        NPersistencyManager persistencyManager = new NPersistencyManager();

                        // load a drawing from the XML file
                        drawing = persistencyManager.LoadDrawingFromFile(ofd.FileName);

                        // display the drawing
                        Ndv.Document = drawing;

                        ZoneConnectorData("2");
                    }
                    else
                    {
                        MessageBox.Show("Please import cndx file!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void AppendXMLNodes()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("c:\\temp\\Drawingfile.cndx");

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
                XmlElement Zone21 = xmlDoc.CreateElement("ConnectorZone21");
                Zone21.InnerText = result.Zone21.ID.ToString();
                croot.AppendChild(Zone21);

                XmlElement ZoneName = xmlDoc.CreateElement("Name");
                ZoneName.InnerText = result.Zone21.Name.ToString();

                XmlElement Group = xmlDoc.CreateElement("Group");
                Group.InnerText = result.Zone21.Group.ToString();

                XmlElement Relation = xmlDoc.CreateElement("Relation");
                Relation.InnerText = result.Zone21.ToString();

                Zone21.AppendChild(ZoneName);
                Zone21.AppendChild(Group);
                Zone21.AppendChild(Relation);
            }

            xmlDoc.Save("c:\\temp\\mysavefile.cndx");
            foreach (var result in connector)
            {

                XmlElement Zone22 = xmlDoc.CreateElement("ConnectorZone22");
                Zone22.InnerText = result.Zone22.ID.ToString();
                croot.AppendChild(Zone22);

                XmlElement ZoneName1 = xmlDoc.CreateElement("Name");
                ZoneName1.InnerText = result.Zone22.Name.ToString();

                XmlElement Group1 = xmlDoc.CreateElement("Group");
                Group1.InnerText = result.Zone22.Group.ToString();

                XmlElement Relation1 = xmlDoc.CreateElement("Relation");
                Relation1.InnerText = result.Zone22.ToString();


                Zone22.AppendChild(ZoneName1);
                Zone22.AppendChild(Group1);
                Zone22.AppendChild(Relation1);

            }
            xmlDoc.Save("c:\\temp\\mysavefile.cndx");
            //XmlElement Category = xmlDoc.CreateElement("Category");
            //Category.InnerText = result.Category.ToString();

            //XmlElement Color = xmlDoc.CreateElement("Color");
            //Color.InnerText = result.Color.ToString();

            //XmlElement Area = xmlDoc.CreateElement("Area");
            //Area.InnerText = result.Area.ToString();

            //XmlElement Width = xmlDoc.CreateElement("Width");
            //Width.InnerText = result.Width.ToString();

            //XmlElement Length = xmlDoc.CreateElement("Length");
            //Length.InnerText = result.Length.ToString();

            //XmlElement Height = xmlDoc.CreateElement("Height");
            //Height.InnerText = result.Height.ToString();

            //XmlElement Floor = xmlDoc.CreateElement("Floor");
            //Floor.InnerText = result.Floor.ToString();

            //XmlElement Ratio = xmlDoc.CreateElement("Ratio");
            //Ratio.InnerText = result.Ratio.ToString();

            //XmlElement Type = xmlDoc.CreateElement("Type");
            //Type.InnerText = result.Type.ToString();





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

       

        //private void button1_Click(object sender, EventArgs e)
        //{


        //    // Get the active view
        //    NDrawingDocument document = MainFirstPageControl.CommandBarsManager.Document as NDrawingDocument;

        //    if (document.ActiveLayer.Selection.Nodes.Count == 2)
        //    {
        //        // Get the first selected shape
        //        NShape shape1 = document.ActiveView.Selection.Nodes[0] as NShape;

        //        // Get the second selected shape
        //        NShape shape2 = document.ActiveView.Selection.Nodes[1] as NShape;

        //        // Create a connector line
        //        NLineShape connector = new NLineShape(shape1.PinPoint, shape2.PinPoint);

        //        // Add the connector line to the active layer
        //        document.ActiveLayer.AddChild(connector);

        //        // Refresh the view
        //        document.ActiveView.Refresh();
        //    }
        //    else
        //    {
        //        // Show an error message if two shapes are not selected
        //        MessageBox.Show("Please select two shapes to create a connector line.");
        //    }
        //}

        //private void button2_Click(object sender, EventArgs e)
        //{
        //        // Get the active drawing document
        //        NDrawingDocument document = MainFirstPageControl.Document as NDrawingDocument;

        //        // Check if the document and active layer are valid
        //        if (document != null && document.ActiveLayer != null)
        //        {
        //            NNodeList nodes = document.ActiveLayer.Children(NFilters.Shape2D);
        //            if (nodes.Count != 0)
        //            {
        //                // Create an instance of NImageExporter
        //                NImageExporter imageExporter = new NImageExporter(document);
        //                imageExporter.KnownBoundsTable.Add("All nodes", NFunctions.ComputeNodesBounds(nodes, null));


        //            {
        //                // Specify the desired image format and save the image
        //                string imagePath = "path/to/save/image.jpg";
        //                imageExporter.SaveImage(imagePath, ENImageFormat.Jpeg);

        //                MessageBox.Show("Image exported successfully.");
        //            }


        //            // Show the image exporter dialog
        //            imageExporter.ShowDialog();
        //            }
        //            else
        //            {
        //                // Show an error message if there are no nodes in the active layer
        //                MessageBox.Show("No nodes found in the active layer.");
        //            }
        //        }
        //        else
        //        {
        //            // Show an error message if the active drawing document or active layer is not available
        //            MessageBox.Show("No active drawing document or active layer found.");
        //        }


    }
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
    


    

    
