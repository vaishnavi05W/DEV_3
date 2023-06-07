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
        //NGroup group = new NGroup();

        public NRectangleShape startNode = null;
        public NRectangleShape endNode = null;
        private NPersistencyManager persistencyManager;
        private NGroupBox groupPropertiesGroup;
        private NCheckBox autoDestroyCheckBox;
        private NCheckBox canBeEmptyCheckBox;
        private NLayer layer;
        NRectangleShape m_DesiredSizeShape;
        NFlowLayout ZonesLayout;

        List<Connector_Main> connector;
        List<Zone_Main> zone;
        List<Group> groups;
        List<Connector_Group> congroup;
        List<NRectangleShape> existingGroups = new List<NRectangleShape>();

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
            groups = new List<Group>();
            congroup = new List<Connector_Group>();
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
            layer = new NLayer();
            Ndd.Layers.AddChild(layer);
            

            ZonesLayout = new NFlowLayout();
            ZonesLayout.Direction = LayoutDirection.LeftToRight;
            ZonesLayout.ConstrainMode = CellConstrainMode.Ordinal;
            ZonesLayout.HorizontalContentPlacement = ContentPlacement.Far;
            ZonesLayout.VerticalContentPlacement = ContentPlacement.Far;
            ZonesLayout.HorizontalSpacing = 20;
            ZonesLayout.VerticalSpacing = 20;
            ZonesLayout.MaxOrdinal = 3;
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
                NLayoutContext layoutContext = new NLayoutContext();
                layoutContext.GraphAdapter = new NShapeGraphAdapter();
                layoutContext.BodyAdapter = new NShapeBodyAdapter(Ndd);
                layoutContext.BodyContainerAdapter = new NDrawingBodyContainerAdapter(Ndd);
                
                List<string> existingGroups = new List<string>();
                NGroup maingp = new NGroup();
                string groupname = string.Empty;
                foreach (DataRow dr in dtMain.Rows)
                {
                    if (groupname != dr["Group"].ToString())
                    {
                        groupname = dr["Group"].ToString();
                        if (!String.IsNullOrEmpty(groupname))
                        {
                            
                                DataTable dtGroup = dtMain.Select("Group = '" + groupname + "'").CopyToDataTable();
                                if (dtGroup.Rows.Count > 0)
                                {
                                    List<NRectangleShape> zones = GetShape(dtGroup);

                                NGroup newGroup = new NGroup();
                                newGroup.Name = groupname;
                                newGroup = CreateGroupLayout(layer, layoutContext, groupname, dtGroup);
                                existingGroups.Add(newGroup.Name);
                                Ndd.ActiveLayer.AddChild(newGroup);
                            }

                        }
                    }
                }
                NNodeList listedGroup = new NNodeList();
                foreach (var gname in existingGroups)
                {
                    listedGroup.Add(Ndd.ActiveLayer.GetChildByName(gname));
                }

                ZonesLayout.Layout(listedGroup, layoutContext);
                //NFlowLayout flowlayout = new NFlowLayout();
                //flowlayout.Direction = LayoutDirection.LeftToRight;
                //flowlayout.ConstrainMode = CellConstrainMode.Ordinal;
                //flowlayout.HorizontalContentPlacement = ContentPlacement.Far;
                //flowlayout.VerticalContentPlacement = ContentPlacement.Far;
                //flowlayout.HorizontalSpacing = 50;
                //flowlayout.VerticalSpacing = 50;
                //flowlayout.MaxOrdinal = 1;
                //NNodeList g = new NNodeList();
                //foreach (NRectangleShape rectangle in existingGroups)
                //{
                //    g.Add(rectangle);
                //    //group.Shapes.AddChild(rectangle);
                //}


                //NFlowLayout layout = new NFlowLayout();
                //layout.Direction = LayoutDirection.LeftToRight;
                //layout.ConstrainMode = CellConstrainMode.Ordinal;
                //layout.HorizontalContentPlacement = ContentPlacement.Far;
                //layout.VerticalContentPlacement = ContentPlacement.Far;
                //layout.HorizontalSpacing = 20;
                //layout.VerticalSpacing = 20;
                //layout.MaxOrdinal = 2;



                //if (layout != null)
                //{


                //    // layout the shapes
                //    layout.Layout(g, layoutContext);
                //}

                ////Ndd.AutoBoundsMode = AutoBoundsMode.AutoSizeToContent;
            }
            catch (Exception e)
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
            Color color2 = Color.Black;
            
           // minHeightTextBox.Text = document.AutoBoundsMinSize.Height.ToString();


            NGroup group = new NGroup();
            group.Name = dtGroup.Rows[0]["Group"].ToString();
            
            group.Protection = new NAbilities(AbilitiesMask.Select);
            List<NRectangleShape> zones = GetShape(dtGroup);
            //GetShape(dtGroup);
                //foreach (NRectangleShape r in zones)
                //{
                //    group.Shapes.AddChild(r);
                //}
                //CreateDecorators(frame, dtGroup.Rows[0]["Group"].ToString());
                //group.UpdateModelBounds();

                //frame.Style.FillStyle = new NColorFillStyle(color1);
                //group.Shapes.AddChild(frame);
                //CreateGroupPorts(frame);
                //group.Style.FillStyle = new NColorFillStyle(color2);
                //group.Style.StrokeStyle = new NStrokeStyle(color2);
                //frame.SendToBack();
                return group;
        }

        private bool CheckOverlap(object  item1, object item2)
        {
            bool result =  false;
            if(item1 is NGroup group1 && item2 is NGroup group2)
            {
                result =  group1.Bounds.IntersectsWith(group2.Bounds);
            }
            else if (item1 is NRectangleShape zone1 && item2 is NRectangleShape zone2)
            {
                result = zone1.Bounds.IntersectsWith(zone2.Bounds);
            }
            return result;
        }

        private List<NRectangleShape> GetShape(DataTable dtGroup)
        {
            List<NRectangleShape> zones = new List<NRectangleShape>();

            foreach (DataRow dr in dtGroup.Rows)
            {
                
                float width = (float)Math.Sqrt(Convert.ToDouble(dr[7].ToString()) * 2);
                float height = (float)(Convert.ToDouble(dr[7].ToString()) / width);
               
                Color color1 = Color.FromName(dr[6].ToString());
                Color color2 = Color.Black;

                NRectangleShape zone = new NRectangleShape();
                zone = new NRectangleShape(0, 0, width, height);
                zone.Style.FillStyle = new NColorFillStyle(color1);
                zone.Style.StrokeStyle = new NStrokeStyle(color2);
                string NodeLabelIn = dr[0].ToString()
                   + System.Environment.NewLine + dr[7].ToString() + " " + "m\u00b2";
                string NodeLabelOut = dr[1].ToString();
                zone.Text = NodeLabelIn;
                zone.Name = NodeLabelOut;
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
                //Ndd.ActiveLayer.AddChild(zone);
                //zone.BringToFront();

                zones.Add(zone);
            }
            return zones;
           
        }

        private NGroup CreateGroupLayout(NLayer layer,NLayoutContext layoutContext, String groupName,DataTable dtgroup)
        {
            NGroup group = new NGroup();
            if (ZonesLayout != null)
            {
                NRectangleF bounds = new NRectangleF(0,0,1,1);
                NRectangleShape frame = new NRectangleShape(bounds);
                frame.Protection = new NAbilities(AbilitiesMask.Select);
                List<NRectangleShape> zones = GetShape(dtgroup);
                foreach (NRectangleShape r in zones)
                {
                    group.Shapes.AddChild(r);
                }
                NNodeList listedNode = new NNodeList();
                foreach(NRectangleShape n in group.Descendants(NFilters.Shape2D, -1))
                {
                    listedNode.Add(n);
                }

                ZonesLayout.Layout(listedNode, layoutContext);
                group.Name = dtgroup.Rows[0]["Group"].ToString();
                group.UpdateModelBounds();
                //frame.Style.FillStyle = new NColorFillStyle(Color.Gray);
                //group.Style.FillStyle = new NColorFillStyle(Color.Black);
                //group.Style.StrokeStyle = new NStrokeStyle(1,Color.Black);
                //CreateDecorators(frame, group.Name);
                //group.Shapes.AddChild(frame);
                //frame.SendToBack();
            }

            

            return group;
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
           dataGridView1.ReadOnly = false;
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

        private void CreateDecorators(NRectangleShape shape, string decoratorText)
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
                foreach (NLineShape edge in Ndv.Document.Descendants(NFilters.Shape1D, -1)) ////Detecting Line Shapes with 1D 
                {
                    // Check if the shape is a line and has a source and target shape
                    if (edge is NLineShape line && line.FromShape != null && line.ToShape != null) //For Connectors
                    {

                        if (line.FromShape.Group is NGroup && line.ToShape.Group is NGroup
                            && line.FromShape.Group != line.ToShape.Group)
                        {
                            congroup.Add(new Connector_Group(GetGroupDataFromDataSource((NGroup)line.FromShape.Group)
                                , GetGroupDataFromDataSource((NGroup)line.ToShape.Group)
                                , 0
                                , line.Length
                                , line.StyleSheet.Style.StrokeStyle.Color));
                        }
                        else if (line.StartPlug.Shape.FromShape is NRectangleShape && line.EndPlug.Shape.ToShape is NRectangleShape)
                        {
                            //if(String.IsNullOrWhiteSpace(line.FromShape.Group.Name))
                            DataRow drFrom = GetShapeDataFromDataSource((NRectangleShape)line.FromShape);
                            DataRow drTo = GetShapeDataFromDataSource((NRectangleShape)line.ToShape);
                            Zone_Main z1 = GetZoneData(drFrom);
                            Zone_Main z2 = GetZoneData(drTo);

                            connector.Add(GetConnectorData(z1, z2, line));
                        }
                    }
                }
                foreach (var module in Ndv.Document.Descendants(NFilters.Shape2D, -1)) //Detecting Rectangel Shapes with 2D 
                {
                    if (module is NGroup g) //For Groups
                    {
                        groups.Add(GetGroupDataFromDataSource(g));
                    }
                    else if (module is NRectangleShape shape) // For Zones
                    {
                        DataRow dr = GetShapeDataFromDataSource(shape);
                        if (dr != null)
                        {
                            zone.Add(GetZoneData(dr));
                        }
                    }
                }
            }
            else if (flg == "2")
            {
                foreach (NLineShape edge in Ndv.Document.Descendants(NFilters.Shape1D, -1)) ////Detecting Line Shapes with 1D 
                {
                    // Check if the shape is a line and has a source and target shape
                    if (edge is NLineShape line && line.FromShape != null && line.ToShape != null) //For Connectors
                    {

                        if (line.FromShape.Group is NGroup && line.ToShape.Group is NGroup
                            && line.FromShape.Group != line.ToShape.Group)
                        {
                            congroup.Add(new Connector_Group(GetGroupDataFromDataSource((NGroup)line.FromShape.Group)
                                , GetGroupDataFromDataSource((NGroup)line.ToShape.Group)
                                , 0
                                , line.Length
                                , line.StyleSheet.Style.StrokeStyle.Color));
                        }
                        else if (line.StartPlug.Shape.FromShape is NRectangleShape && line.EndPlug.Shape.ToShape is NRectangleShape)
                        {
                            //if(String.IsNullOrWhiteSpace(line.FromShape.Group.Name))
                            DataRow drFrom = GetShapeDataFromDataSource((NRectangleShape)line.FromShape);
                            DataRow drTo = GetShapeDataFromDataSource((NRectangleShape)line.ToShape);
                            Zone_Main z1 = GetZoneData(drFrom);
                            Zone_Main z2 = GetZoneData(drTo);

                            connector.Add(GetConnectorData(z1, z2, line));
                        }
                    }
                }
                foreach (var module in Ndv.Document.Descendants(NFilters.Shape2D, -1)) //Detecting Rectangel Shapes with 2D 
                {
                    if (module is NGroup g) //For Groups
                    {
                        groups.Add(GetGroupDataFromDataSource(g));
                    }
                    else if (module is NRectangleShape shape) // For Zones
                    {
                        DataRow dr = GetShapeDataFromDataSource(shape);
                        if (dr != null)
                        {
                            zone.Add(GetZoneData(dr));
                        }
                    }
                }
            }
         }
        private Group GetGroupDataFromDataSource(NGroup group)
        {
            DataTable dtGroup = dtSource.Select("Group = '" + group.Name + "'").CopyToDataTable();
            List<Zone_Main> zones = new List<Zone_Main>();
            foreach (DataRow dr in dtGroup.Rows) {
                zones.Add(GetZoneData(dr));
            }

            Group group_data = new Group(dtGroup.Rows[0]["Group"].ToString()
                , Color.FromName(dtGroup.Rows[0]["GroupColor"].ToString())
                , Convert.ToDouble(dtGroup.Rows[0]["GroupArea"].ToString())
                , zones
                );
            return group_data;
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
                         , dr[3].ToString()
                         , relation is null ? String.Empty : relation
                         , dr[5] is DBNull ? 0 : Convert.ToDouble(dr[5].ToString())
                         , dr[6].ToString()
                         , dr[7] is DBNull ? 0 : Convert.ToDouble(dr[7])
                         , dr[8] is DBNull ? 0 : Convert.ToDouble(dr[8])
                         , dr[9] is DBNull ? 0 : Convert.ToDouble(dr[9])
                         , dr[10] is DBNull ? 0 : Convert.ToDouble(dr[10])
                         , Convert.ToInt32(dr[11])
                         , dr[12] is DBNull ? 0 : Convert.ToDouble(dr[12])
                         , dr[13].ToString()); ;

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

            XmlElement groot = xmlDoc.CreateElement("Group");
            xmlDoc.DocumentElement.AppendChild(groot);

            XmlElement gcroot = xmlDoc.CreateElement("GroupConnetor");
            xmlDoc.DocumentElement.AppendChild(gcroot);

            foreach (var result in zone)
            {
                XmlElement ZoneID = xmlDoc.CreateElement("ZoneID");
                ZoneID.InnerText = result.ID.ToString();
                zroot.AppendChild(ZoneID);

                XmlElement ZoneName = xmlDoc.CreateElement("Name");
                ZoneName.InnerText = result.Name.ToString();

                XmlElement Group = xmlDoc.CreateElement("Group");
                Group.InnerText = result.Group.ToString();

                XmlElement GroupColor = xmlDoc.CreateElement("GroupColor");
                GroupColor.InnerText = result.GroupColor.ToString();

                XmlElement Relation = xmlDoc.CreateElement("Relation");
                Relation.InnerText = result.Relation.ToString();

                XmlElement GroupArea = xmlDoc.CreateElement("GroupArea");
                GroupArea.InnerText = result.GroupArea.ToString();
                //XmlElement Category = xmlDoc.CreateElement("Category");
                //Category.InnerText = result.Category.ToString();

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
                ZoneID.AppendChild(GroupColor);
                ZoneID.AppendChild(Relation);
                ZoneID.AppendChild(GroupArea);
                ZoneID.AppendChild(Color);
                ZoneID.AppendChild(Area);
                ZoneID.AppendChild(Width);
                ZoneID.AppendChild(Length);
                ZoneID.AppendChild(Height);
                ZoneID.AppendChild(Floor);
                ZoneID.AppendChild(Ratio);
                ZoneID.AppendChild(Type);
            }
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
            foreach (var result in groups)
            {
                XmlElement Group = xmlDoc.CreateElement("Group");
                Group.InnerText = result.Name.ToString();
                groot.AppendChild(Group);

                XmlElement Color = xmlDoc.CreateElement("Color");
                Color.InnerText = result.Color.Name.ToString();

                XmlElement GroupArea = xmlDoc.CreateElement("GroupArea");
                GroupArea.InnerText = result.GroupArea.ToString();

                Group.AppendChild(Color);
                Group.AppendChild(GroupArea);
            }
            xmlDoc.Save("c:\\temp\\mysavefile.cndx");
            foreach (var result in congroup)
            {
                XmlElement Group1 = xmlDoc.CreateElement("Group1");
                Group1.InnerText = result.Group1.Name.ToString();
                //groot.AppendChild(Group1);

                XmlElement Group2 = xmlDoc.CreateElement("Group2");
                Group2.InnerText = result.Group2.Name.ToString();

                XmlElement Type = xmlDoc.CreateElement("Type");
                Type.InnerText = result.Type.ToString();

                XmlElement Length = xmlDoc.CreateElement("Length");
                Length.InnerText = result.Length.ToString();

                XmlElement Color = xmlDoc.CreateElement("Color");
                Color.InnerText = result.Color.Name.ToString();

                gcroot.AppendChild(Group1);
                gcroot.AppendChild(Group2);
                gcroot.AppendChild(Type);
                gcroot.AppendChild(Length);
                gcroot.AppendChild(Color);
            }
            xmlDoc.Save("c:\\temp\\mysavefile.cndx");
        }
    }
}
        
    


    

    
