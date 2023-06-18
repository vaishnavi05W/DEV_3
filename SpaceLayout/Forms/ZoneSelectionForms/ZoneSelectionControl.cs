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
        public NGroup startgroup = null;
        public NGroup endgroup = null;
        public NRectangleShape startNode = null;
        public NRectangleShape endNode = null;
        private NPersistencyManager persistencyManager;
        private NGroupBox groupPropertiesGroup;
        private NCheckBox autoDestroyCheckBox;
        private NCheckBox canBeEmptyCheckBox;
        private NLayer layer;
        
        NFlowLayout ZonesLayout;
        NFlowLayout GroupsLayout;
        NFlowLayout FloorLayout;
        List<Connector_Main> connector;
        List<Zone_Main> zone;
        List<Group> groups;
        List<Connector_Group> congroup;
        // List<NRectangleShape> existingGroups = new List<NRectangleShape>();
        
        IDictionary<string, string> existingGroups;
        public ZoneSelectionControl()
        {
            InitializeComponent();
            this.persistencyManager = new NPersistencyManager();
            this.groupPropertiesGroup = new NGroupBox();
            this.autoDestroyCheckBox = new NCheckBox();
            this.canBeEmptyCheckBox = new NCheckBox(); this.groupPropertiesGroup.SuspendLayout();
            this.SuspendLayout();

            this.Load += IS_Load;

            connector = new List<Connector_Main>();
            zone = new List<Zone_Main>();
            groups = new List<Group>();
            congroup = new List<Connector_Group>();
            // Add the diagram view to the form
            this.Controls.Add(Ndv);
        }

        private void ChartControl_NodeCreated(NNodeEventArgs args)
        {
            Console.WriteLine(args.Node.ToString());
        }

        private void IS_Load(object sender, EventArgs e)
        {
            Form f = this.ParentForm;
            //TableLayoutPanel tablelayout = f.Controls.Find("tableLayoutPanel1", true).FirstOrDefault() as TableLayoutPanel;
            ToolStrip btnFile =f.Controls[4] as ToolStrip;
            ((System.Windows.Forms.ToolStripDropDownItem)btnFile.Items[0]).DropDown.Items[0].Click -= BtnSave_Click;
            ((System.Windows.Forms.ToolStripDropDownItem)btnFile.Items[0]).DropDown.Items[1].Click -= BtnLoad_Click;

            ((System.Windows.Forms.ToolStripDropDownItem)btnFile.Items[0]).DropDown.Items[0].Click += BtnSave_Click;
            ((System.Windows.Forms.ToolStripDropDownItem)btnFile.Items[0]).DropDown.Items[1].Click += BtnLoad_Click;

            Ndv = f.Controls.Find("nDrawingView1", true).FirstOrDefault() as NDrawingView;
            if (Ndv != null)
            {
                Ndd = Ndv.Document;
            }

            ZonesLayout = new NFlowLayout();
            ZonesLayout.Direction = LayoutDirection.LeftToRight;
            ZonesLayout.ConstrainMode = CellConstrainMode.Ordinal;
            ZonesLayout.HorizontalContentPlacement = ContentPlacement.Near;
            ZonesLayout.VerticalContentPlacement = ContentPlacement.Near;
            ZonesLayout.HorizontalSpacing = 20;
            ZonesLayout.VerticalSpacing = 20;
            ZonesLayout.MaxOrdinal = 3;

            GroupsLayout = new NFlowLayout();
            GroupsLayout.Direction = LayoutDirection.LeftToRight;
            GroupsLayout.ConstrainMode = CellConstrainMode.Ordinal;
            GroupsLayout.HorizontalContentPlacement = ContentPlacement.Far;
            GroupsLayout.VerticalContentPlacement = ContentPlacement.Far;
            GroupsLayout.HorizontalSpacing = 50;
            GroupsLayout.VerticalSpacing = 50;
            GroupsLayout.MaxOrdinal = 3;

            FloorLayout = new NFlowLayout();
            FloorLayout.Direction = LayoutDirection.LeftToRight;
            FloorLayout.ConstrainMode = CellConstrainMode.Ordinal;
            FloorLayout.HorizontalContentPlacement = ContentPlacement.Far;
            FloorLayout.VerticalContentPlacement = ContentPlacement.Far;
            FloorLayout.HorizontalSpacing = 50;
            FloorLayout.VerticalSpacing = 50;
            FloorLayout.MaxOrdinal = 1;
            BindRatioCombo();
            BindGrid();


            //this.dataGridView1.ReadOnly = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.cboZonesRatio.Visible = false;
            //this.cboZonesRatio.SelectedValueChanged += ZonesRatio_SelectedValueChanged;
            //this.dataGridView1.CellFormatting += dataGridView1_CellFormatting;
            dataGridView1.CellValueChanged += new DataGridViewCellEventHandler(dataGridView1_CellValueChanged);
            dataGridView1.CurrentCellDirtyStateChanged += new EventHandler(dataGridView1_CurrentCellDirtyStateChanged);

            btnHorizontal.Click += Horizontal_Clicked;
            btnVertical.Click += Vertical_Clicked;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (Ndv.Document.ActiveLayer != null)
                    Export();
                else MessageBox.Show("Diagram View is empty!");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                Import();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Connect_Horizontal(NNodeEventArgs args)
        {
            var module = args.Node;
            List<string> GroupRecord = new List<string>();
            GroupRecord = dtSource.AsEnumerable().Select(r => r.Field<string>("Group")).Distinct().ToList();
            if (module is NGroup group && GroupRecord.Contains(group.Name))
            {
                if (startgroup == null)
                    startgroup = group;
                else if(endgroup == null)
                {
                    endgroup = group;

                    NLineShape line = new NLineShape();
                    line.StyleSheetName = NDR.NameConnectorsStyleSheet;
                    
                    line.Style.FillStyle = new NColorFillStyle(Color.Black);
                    line.Style.StrokeStyle = new NStrokeStyle(Color.Black);
                    Ndd.ActiveLayer.AddChild(line);
                    //line.StartPoint = new NPointF(startgroup.Center.X, startgroup.Center.Y);
                    //line.EndPoint = new NPointF(endgroup.Center.X, endgroup.Center.Y);
                    NShape start = (NShape)startgroup.Shapes.GetChildAt(0);
                    NShape end = (NShape)endgroup.Shapes.GetChildAt(0);
                    line.StartPlug.Connect(start.Ports.GetChildByName("GroupPort", 0) as NPort);
                    line.EndPlug.Connect(end.Ports.GetChildByName("GroupPort", 0) as NPort);
                    line.Tag = "Horizontal";
                    
                    Ndv.Selection.DeselectAll();

                    startgroup = null;
                    endgroup = null;

                    NAbilities protection = line.Protection;
                    protection.InplaceEdit = true;
                    line.Protection = protection;
                    line.DoubleClick += Line_DoubleClick;

                    Ndv.NodeSelected -= Connect_Horizontal;
                }
            }
        }

        private void Line_DoubleClick(NNodeViewEventArgs args)
        {
            var module = args.Node;
            if(module is NLineShape line)
            {
                string title = "";
                if (line.Tag.ToString() == "Horizontal")
                    title = "Horizontal Content Weight";
                else
                    title = "Vertical Content Weight";
                ContenetPlacementForm Contentplacement = new ContenetPlacementForm(title);
                if (Contentplacement.ShowDialog() == DialogResult.Cancel)
                {
                    if (!string.IsNullOrWhiteSpace(Contentplacement.placementWeight))
                    {
                        line.Text = line.Tag.ToString() +System.Environment.NewLine + Contentplacement.placementWeight.ToString();
                        
                        line.Style.TextStyle = new NTextStyle()
                        {
                            Orientation = 180,
                        };
                    }
                }
                Ndv.Selection.DeselectAll();
            }
            
        }

        private void Connect_Vertical(NNodeEventArgs args)
        {
            var module = args.Node;
            List<string> GroupRecord = new List<string>();
            GroupRecord = dtSource.AsEnumerable().Select(r => r.Field<string>("Group")).Distinct().ToList();
            if (module is NGroup group && GroupRecord.Contains(group.Name))
            {
                if (startgroup == null)
                    startgroup = group;
                else if (endgroup == null)
                {
                    endgroup = group;

                    NLineShape line = new NLineShape();
                    line.StyleSheetName = NDR.NameConnectorsStyleSheet;
                    line.Style.FillStyle = new NColorFillStyle(Color.Black);
                    line.Style.StrokeStyle = new NStrokeStyle(Color.Black);
                    Ndd.ActiveLayer.AddChild(line);
                    NShape start = (NShape)startgroup.Shapes.GetChildAt(0);
                    NShape end = (NShape)endgroup.Shapes.GetChildAt(0);
                    line.StartPlug.Connect(start.Ports.GetChildByName("GroupPort", 0) as NPort);
                    line.EndPlug.Connect(end.Ports.GetChildByName("GroupPort", 0) as NPort);
                    line.Tag = "Vertical";

                    Ndv.Selection.DeselectAll();

                    startgroup = null;
                    endgroup = null;

                    NAbilities protection = line.Protection;
                    protection.InplaceEdit = true;
                    line.Protection = protection;
                    line.DoubleClick += Line_DoubleClick;

                    Ndv.NodeSelected -= Connect_Vertical;

                }
            }
        }

        private void BindRatioCombo()
        {
            //cboZonesRatio.DropDownStyle = ComboBoxStyle.DropDownList;
            Dictionary<string, string> comboSource = new Dictionary<string, string>();
            comboSource.Add("1", "1:1");
            comboSource.Add("2", "2:1");
            comboSource.Add("3", "3:2");
            //cboZonesRatio.DataSource = new BindingSource(comboSource, null);
            //cboZonesRatio.DisplayMember = "Value";
            //cboZonesRatio.ValueMember = "Key";
            //cboZonesRatio.SelectedValue = "2";

            DataGridViewComboBoxColumn cboRatio = (DataGridViewComboBoxColumn)dataGridView1.Columns["Column11"];
            cboRatio.DataPropertyName = "Ratio";
            cboRatio.FlatStyle = FlatStyle.Standard;
            cboRatio.DisplayMember = "Value";

            //cboRatio.ValueMember = "Key";
            ((DataGridViewComboBoxColumn)dataGridView1.Columns["Column11"]).DataSource = comboSource.ToList();
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
            //dtSource.Columns.Add("GroupColor");
            dtSource.Columns.Add("Relation");
            //dtSource.Columns.Add("GroupArea");
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
                    ds.Tables["Input Data_Module"].Columns.Remove("Column13");
                    ds.Tables["Input Data_Module"].Columns.Remove("Column14");
                    //ds.Tables["Input Data_Module"].Columns.Remove("Column15");


                    foreach (DataRow dr in ds.Tables["Input Data_Module"].Rows)
                    {
                        dtSource.Rows.Add(dr.ItemArray);

                    }
                }
            }

            foreach (DataRow r in dtSource.Rows)
            {
                r["Ratio"] = "2:1";
            }

            //Bind datatable to Gridview
            if (dtSource.Rows.Count > 0)
            {
                dataGridView1.DataSource = dtSource;
                if(Ndd== null || Ndd.ActiveLayer == null 
                    || Ndd.ActiveLayer.Descendants(NFilters.Shape1D, -1).Count == 0 || Ndd.ActiveLayer.Descendants(NFilters.Shape2D, -1).Count == 0)
                MainFunction(dtSource);
            }
        }
        private void ZonesRatio_SelectedValueChanged(object sender, EventArgs e)
        {
            string ratio = (sender as ComboBox).SelectedValue as string;
            if (ratio == "1")
            {
                Ndd.ActiveLayer.RemoveAllChildren();
                MainFunction(dtSource);
            }
            else if (ratio == "2")
            {
                Ndd.ActiveLayer.RemoveAllChildren();
                MainFunction(dtSource);
            }
            else
            {
                Ndd.ActiveLayer.RemoveAllChildren();
                MainFunction(dtSource);
            }
        }


        void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                // This fires the cell value changed handler below
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            
            DataGridViewComboBoxCell cb = (DataGridViewComboBoxCell)dataGridView1.Rows[e.RowIndex].Cells[10];
            if (cb.Value != null)
            {
                // do stuff
                dtSource.AcceptChanges();
                Ndd.ActiveLayer.RemoveAllChildren();
                MainFunction(dtSource);
                dataGridView1.Invalidate();
            }
        }

        public void MainFunction(DataTable dtMain)
        {
            try
            {
                Ndd.ActiveLayer.RemoveAllChildren();
                layer = new NLayer();
                Ndd.Layers.AddChild(layer);

                NLayoutContext layoutContext = new NLayoutContext();
                layoutContext.GraphAdapter = new NShapeGraphAdapter();
                layoutContext.BodyAdapter = new NShapeBodyAdapter(Ndd);
                layoutContext.BodyContainerAdapter = new NDrawingBodyContainerAdapter(Ndd);

                existingGroups = new Dictionary<string, string>();
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
                                NGroup newGroup = new NGroup();
                                newGroup.Name = groupname;
                                newGroup = CreateGroupLayout(layer, layoutContext, groupname, dtGroup);
                                NRectangleShape frame = new NRectangleShape(newGroup.Bounds.X, newGroup.Bounds.Y, newGroup.Width, newGroup.Height);
                                //CreateDecorators(frame, newGroup.Name);
                                frame.Protection = new NAbilities(AbilitiesMask.Select);
                                frame.Style.FillStyle = new NColorFillStyle(Color.Transparent);
                                frame.Style.StrokeStyle = new NStrokeStyle(Color.Gray);
                                newGroup.Shapes.AddChild(frame);
                                CreateGroupPorts(frame);


                                newGroup.CreateShapeElements(ShapeElementsMask.Labels);
                                NRotatedBoundsLabel label = new NRotatedBoundsLabel(groupname, newGroup.UniqueId, new Nevron.Diagram.NMargins(0, 0, 0, -155)); //add labels to group for Name
                                label.Mode = BoxTextMode.Wrap;
                                newGroup.Labels.DefaultLabelUniqueId = label.UniqueId;
                                newGroup.Labels.AddChild(label);
                                frame.SendToBack();
                                existingGroups.Add(newGroup.Name, dtGroup.Rows[0]["Floor"].ToString());
                                Ndd.ActiveLayer.AddChild(newGroup);

                                NAbilities protection1 = frame.Protection;
                                protection1.InplaceEdit = true;
                                frame.Protection = protection1;

                                NAbilities protection = newGroup.Protection;
                                protection.InplaceEdit = true;
                                newGroup.Protection = protection;
                            }
                        }
                    }
                }

                List<string> level = new List<string>();
                string floor = string.Empty;
                foreach (var gname in existingGroups)
                {
                    //floor = gname.Value;
                    if (gname.Value != floor)
                    {
                        floor = gname.Value;
                        List<string> names = existingGroups.Where(x => x.Value == floor).Select(x => x.Key).ToList();
                        NGroup floorgroups = new NGroup();
                        floorgroups = CreateFloorLayout(layoutContext, names);
                        floorgroups.Name = floor;
                        NRectangleShape frame = new NRectangleShape(floorgroups.Bounds.X, floorgroups.Bounds.Y, floorgroups.Width, floorgroups.Height);
                        frame.Protection = new NAbilities(AbilitiesMask.Select);
                        frame.Style.FillStyle = new NColorFillStyle(Color.Transparent);
                        frame.Style.StrokeStyle = new NStrokeStyle(Color.Black);
                        floorgroups.Shapes.AddChild(frame);

                        //For Label
                        floorgroups.CreateShapeElements(ShapeElementsMask.Labels);
                        //NRotatedBoundsLabel label = new NRotatedBoundsLabel(floor + " floor", floorgroups.UniqueId, new Nevron.Diagram.NMargins(0, 0, -108, -2));
                        NRotatedBoundsLabel label = new NRotatedBoundsLabel(floor, floorgroups.UniqueId, new Nevron.Diagram.NMargins(0, -110, 0, 0));  //add labels to group for floor
                        label.Mode = BoxTextMode.Wrap;
                        floorgroups.Labels.DefaultLabelUniqueId = label.UniqueId;
                        floorgroups.Labels.Protection = new NAbilities(AbilitiesMask.Delete);
                        floorgroups.Labels.AddChild(label);

                        level.Add(floor);
                        Ndd.ActiveLayer.AddChild(floorgroups);

                        NAbilities protection1 = frame.Protection;
                        protection1.InplaceEdit = true;
                        frame.Protection = protection1;

                        NAbilities protection = floorgroups.Protection;
                        protection.InplaceEdit = true;
                        floorgroups.Protection = protection;
                    }
                }

                NNodeList listedFloor = new NNodeList();
                foreach (string l in level)
                {
                    listedFloor.Add(Ndd.ActiveLayer.GetChildByName(l));
                    //Ndd.ActiveLayer.RemoveChild(Ndd.ActiveLayer.GetChildByName(l));
                }
                FloorLayout.Layout(listedFloor, layoutContext);
                ////Ndd.AutoBoundsMode = AutoBoundsMode.AutoSizeToContent;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }

        }

        private bool CheckOverlap(object item1, object item2)
        {
            bool result = false;
            if (item1 is NGroup group1 && item2 is NGroup group2)
            {
                result = group1.Bounds.IntersectsWith(group2.Bounds);
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
            float width = 0;
            float height = 0;

            foreach (DataRow dr in dtGroup.Rows)
            {
                if (dr[10].ToString() == "1:1") //1:1
                {
                    width = (float)Math.Sqrt(Convert.ToDouble(dr[5].ToString()));
                    height = width;
                }
                else if (dr[10].ToString() == "2:1") //2:1
                {
                    width = (float)Math.Sqrt(Convert.ToDouble(dr[5].ToString()) * 2);
                    height = (float)(width / 2);
                }
                else //3:2
                {
                    width = (float)Math.Sqrt(Convert.ToDouble(dr[5].ToString()) * 3 / 2);
                    height = (float)(width * 2 / 3);
                }

                Color color1 = Color.FromName(dr[4].ToString());
                Color color2 = Color.Black;

                NRectangleShape zone = new NRectangleShape();
                zone = new NRectangleShape(0, 0, width, height);
                zone.Id = Convert.ToInt32(dr[0].ToString());
                zone.Style.FillStyle = new NColorFillStyle(color1);
                zone.Style.StrokeStyle = new NStrokeStyle(color2);
                string NodeLabelIn = dr[0].ToString()
                   + System.Environment.NewLine + dr[5].ToString() + " " + "m\u00b2";
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
                NAbilities protection = zone.Protection;
                protection.InplaceEdit = true;
                zone.Protection = protection;
                zones.Add(zone);
            }
            return zones;

        }

        private NGroup CreateFloorLayout(NLayoutContext layoutContext, List<string> groupnames)
        {
            NGroup group = new NGroup();
            NNodeList listedNode = new NNodeList();

            if (groupnames != null)
            {
                foreach (string n in groupnames)
                {
                    group.Shapes.AddChild(Ndd.ActiveLayer.GetChildByName(n));
                    listedNode.Add(Ndd.ActiveLayer.GetChildByName(n));
                    Ndd.ActiveLayer.RemoveChild(Ndd.ActiveLayer.GetChildByName(n));
                }
                GroupsLayout.Layout(listedNode, layoutContext);
                group.UpdateModelBounds();
            }
            return group;
        }

        private NGroup CreateGroupLayout(NLayer layer, NLayoutContext layoutContext, String groupName, DataTable dtgroup)
        {
            NGroup group = new NGroup();
            if (ZonesLayout != null)
            {
                NRectangleF bounds = new NRectangleF(5, 5, 1, 1);
                NRectangleShape frame = new NRectangleShape(bounds);
                frame.Protection = new NAbilities(AbilitiesMask.Select);
                List<NRectangleShape> zones = GetShape(dtgroup);
                foreach (NRectangleShape r in zones)
                {
                    group.Shapes.AddChild(r);
                }
                NNodeList listedNode = new NNodeList();
                foreach (NRectangleShape n in group.Descendants(NFilters.Shape2D, -1))
                {
                    listedNode.Add(n);
                }
                ZonesLayout.Layout(listedNode, layoutContext);
                group.Name = dtgroup.Rows[0]["Group"].ToString();
                group.UpdateModelBounds();

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

        private void CreateGroupPorts(NRectangleShape group)
        {
            group.CreateShapeElements(ShapeElementsMask.Ports);
            //NPort port = new NRotatedBoundsPort(new NContentAlignment(ContentAlignment.MiddleCenter));
            //NRotatedBoundsPort port1 = new NRotatedBoundsPort(new NContentAlignment(ContentAlignment.BottomCenter));
            //NRotatedBoundsPort port2 = new NRotatedBoundsPort(new NContentAlignment(ContentAlignment.MiddleLeft));
            //NRotatedBoundsPort port3 = new NRotatedBoundsPort(new NContentAlignment(ContentAlignment.MiddleRight));
            //port.Name = "GroupPort";
            //port1.Name = "port1";
            //port2.Name = "port2";
            //port3.Name = "port3";
            //group.Ports.AddChild(port);
            //group.Ports.AddChild(port1);
            //group.Ports.AddChild(port2);
            //group.Ports.AddChild(port3);

            NDynamicPort port = new NDynamicPort(group.UniqueId, ContentAlignment.MiddleCenter, DynamicPortGlueMode.GlueToContour);
            group.Ports.AddChild(port);
            port.Name = "GroupPort";
            group.Ports.DefaultInwardPortUniqueId = port.UniqueId;

        }

        private void Horizontal_Clicked(object sender, EventArgs e)
         {
            //do stuff
            //Connector line = new Connector();\
            Ndv.Selection.DeselectAll();
            startgroup = null;
            endgroup = null;
            Ndv.NodeSelected += Connect_Horizontal;
            
        }

        private void Vertical_Clicked(object sender, EventArgs e)
        {
            //do stuff
            Ndv.Selection.DeselectAll();
            startgroup = null;
            endgroup = null;
            Ndv.NodeSelected += Connect_Vertical;
            
        }

        private void ZoneConnectorData(string flg) //flg: 1 = save, 2 =  load
        {
            //Ndv.Refresh();
            List<string> GroupRecord = new List<string>();
            GroupRecord = dtSource.AsEnumerable().Select(r => r.Field<string>("Group")).Distinct().ToList();
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
                            string[] axistype = { };
                            if (!string.IsNullOrEmpty(line.Text.ToString()))
                            {
                                axistype = line.Text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                                congroup.Add(new Connector_Group(GetGroupDataFromDataSource((NGroup)line.FromShape.Group)
                                    , GetGroupDataFromDataSource((NGroup)line.ToShape.Group)
                                    , line.Tag.ToString()
                                    , axistype[1].ToString()
                                    , line.Length));
                            }
                            else
                            {
                                congroup.Add(new Connector_Group(GetGroupDataFromDataSource((NGroup)line.FromShape.Group)
                                    , GetGroupDataFromDataSource((NGroup)line.ToShape.Group)
                                    , line.Tag.ToString()
                                    , string.Empty
                                    , line.Length));
                            }
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
                        if (GroupRecord.Contains(g.Name))
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
                            string[] axistype = { };
                            if (!string.IsNullOrEmpty(line.Text.ToString()))
                            {
                                axistype = line.Text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                                congroup.Add(new Connector_Group(GetGroupDataFromDataSource((NGroup)line.FromShape.Group)
                                    , GetGroupDataFromDataSource((NGroup)line.ToShape.Group)
                                    , line.Tag.ToString()
                                    , axistype[1].ToString()
                                    , line.Length));
                            }
                            else
                            {
                                congroup.Add(new Connector_Group(GetGroupDataFromDataSource((NGroup)line.FromShape.Group)
                                    , GetGroupDataFromDataSource((NGroup)line.ToShape.Group)
                                    , line.Tag.ToString()
                                    , string.Empty
                                    , line.Length));
                            }
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
                        if (GroupRecord.Contains(g.Name))
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
            foreach (DataRow dr in dtGroup.Rows)
            {
                zones.Add(GetZoneData(dr));
            }

            Group group_data = new Group(dtGroup.Rows[0]["Group"].ToString()
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
                         , relation is null ? String.Empty : relation
                         , dr[4].ToString()
                         , dr[5] is DBNull ? 0 : Convert.ToDouble(dr[5])
                         , dr[6] is DBNull ? 0 : Convert.ToDouble(dr[6])
                         , dr[7] is DBNull ? 0 : Convert.ToDouble(dr[7])
                         , dr[8] is DBNull ? 0 : Convert.ToDouble(dr[8])
                         , Convert.ToInt32(dr[9])
                         , dr[10].ToString()
                         , dr[11].ToString()); ;

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
        private void Export()
        {
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


        private void Import()
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

                XmlElement Relation = xmlDoc.CreateElement("Relation");
                Relation.InnerText = result.Relation.ToString();

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
            }
            xmlDoc.Save("c:\\temp\\mysavefile.cndx");
            foreach (var result in congroup)
            {
                XmlElement Group1 = xmlDoc.CreateElement("Group1");
                Group1.InnerText = result.Group1.Name.ToString();
                //groot.AppendChild(Group1);

                XmlElement Group2 = xmlDoc.CreateElement("Group2");
                Group2.InnerText = result.Group2.Name.ToString();

                XmlElement Axis = xmlDoc.CreateElement("Axis");
                Axis.InnerText = result.Axis.ToString();

                XmlElement Type = xmlDoc.CreateElement("Type");
                Type.InnerText = result.Type.ToString();

                XmlElement Length = xmlDoc.CreateElement("Length");
                Length.InnerText = result.Length.ToString();
                
                gcroot.AppendChild(Group1);
                gcroot.AppendChild(Group2);
                gcroot.AppendChild(Axis);
                gcroot.AppendChild(Type);
                gcroot.AppendChild(Length);
            }
            xmlDoc.Save("c:\\temp\\mysavefile.cndx");
        }
    }
}