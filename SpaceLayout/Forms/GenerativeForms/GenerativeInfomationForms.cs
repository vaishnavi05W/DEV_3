using ExcelDataReader;
using SpaceLayout.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpaceLayout.Object;
using Nevron.Diagram.WinForm;
using Nevron.Diagram;
using Nevron.Diagram.Layout;
using Nevron.Diagram.DataStructures;
using Nevron.GraphicsCore;
using Nevron.Dom;
using Nevron.Diagram.Filters;

namespace SpaceLayout.Forms.GenerativeForms
{
    public partial class GenerativeInfomationForms : UserControl
    {
        public NDrawingView Ndv2;
        public NDrawingDocument Ndd2;
        private NLayer layer2;

        string level = string.Empty;
        List<string> existingFloor = new List<string>();
        List<string> existingZone = new List<string>();
        NFlowLayout ZonesLayout;
        NFlowLayout FloorLayout;

        public DataTable dtZoneRelationship;
        public DataTable dtSourceMain;
        List<string> node = new List<string>();
        private HashSet<Tuple<string, List<string>>> seq = new HashSet<Tuple<string, List<string>>>();
        public GenerativeInfomationForms()
        {
            InitializeComponent();
            this.Load += IS_Load;
        }

        private void IS_Load(object sender, EventArgs args)
        {
            dtSource();
            dt_ZoneRelationship();
            Form f = this.ParentForm;
            Ndv2 = f.Controls.Find("nDrawingView2", true).FirstOrDefault() as NDrawingView;
            if (Ndv2 != null)
            {
                Ndd2 = Ndv2.Document;
            }
            ZonesLayout = new NFlowLayout();
            ZonesLayout.Direction = LayoutDirection.LeftToRight;
            ZonesLayout.ConstrainMode = CellConstrainMode.Ordinal;
            ZonesLayout.HorizontalContentPlacement = ContentPlacement.Near;
            ZonesLayout.VerticalContentPlacement = ContentPlacement.Near;
            ZonesLayout.HorizontalSpacing = 100;
            ZonesLayout.VerticalSpacing = 20;
            ZonesLayout.MaxOrdinal = 3;

            FloorLayout = new NFlowLayout();
            FloorLayout.Direction = LayoutDirection.LeftToRight;
            FloorLayout.ConstrainMode = CellConstrainMode.Ordinal;
            FloorLayout.HorizontalContentPlacement = ContentPlacement.Far;
            FloorLayout.VerticalContentPlacement = ContentPlacement.Far;
            FloorLayout.HorizontalSpacing = 50;
            FloorLayout.VerticalSpacing = 50;
            FloorLayout.MaxOrdinal = 1;

            node = dtSourceMain.AsEnumerable()
                .Select(s => s.Field<string>("ID"))
                .Distinct()
                .ToList();
            Graph g = new Graph(node);
            foreach (DataRow dr in dtZoneRelationship.Rows)
            {
                g.AddEdge(Convert.ToInt32(dr[0]), Convert.ToInt32(dr[1]));
            }
            seq = g.DFS();
            dgvZoneRelationship.Enabled = false;

            Bind_UpperPanel();
        }

        private void Bind_UpperPanel()
        {
            Button btn1 = new Button();
            btn1.Text = "Alt1";
            btn1.Name = "Alt1";
            btn1.ForeColor = Color.Black;
            btn1.Dock = DockStyle.Fill;

            Button btn2 = new Button();
            btn2.Text = "Alt2";
            btn2.Name = "Alt2";
            btn2.ForeColor = Color.Black;
            btn2.Dock = DockStyle.Fill;

            Button btn3 = new Button();
            btn3.Text = "Alt3";
            btn3.Name = "Alt3";
            btn3.ForeColor = Color.Black;
            btn3.Dock = DockStyle.Fill;

            btn1.Click += btnAlt_Clicked;
            btn2.Click += btnAlt_Clicked;
            btn3.Click += btnAlt_Clicked;

            TableLayoutPanel panel = new TableLayoutPanel();
            panel.ColumnCount = 3;
            panel.RowCount = 1;
            panel.Height = 40;
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            panel.Controls.Add(btn1, 0, 0);
            panel.Controls.Add(btn2, 1, 0);
            panel.Controls.Add(btn3, 3, 0);
            //panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 2F));
            //panel.Controls.Add(new Button() { Text = "Alt1", Name = "Alt1",  ForeColor = Color.Black, Dock = DockStyle.Fill }, 0, 0);
            //panel.Controls.Add(new Button() { Text = "Alt2", Name = "Alt2", ForeColor = Color.Black, Dock = DockStyle.Fill }, 1, 0);
            //panel.Controls.Add(new Button() { Text = "Alt3", Name = "Alt3", ForeColor = Color.Black, Dock = DockStyle.Fill }, 3, 0);
            this.tableLayoutPanel1.Controls.Add(panel, 0, 0);
            panel.Dock = DockStyle.Top;
        }

        private void btnAlt_Clicked(object sender, EventArgs e)
        {
            string text = (sender as Button).Text as string;
            if (text.Equals("Alt1"))
            {
                Ndd2.ActiveLayer.RemoveAllChildren();
                Ndd2.SmartRefreshAllViews();
                DrawDiagram("1");
                LowerTable_and_DrawConnector();
            }
            else if (text.Equals("Alt2"))
            {
                Ndd2.ActiveLayer.RemoveAllChildren();
                Ndd2.SmartRefreshAllViews();
                DrawDiagram("2");
                LowerTable_and_DrawConnector();
            }
            else
            {
                Ndd2.ActiveLayer.RemoveAllChildren();
                Ndd2.SmartRefreshAllViews();
                DrawDiagram("3");
                LowerTable_and_DrawConnector();
            }
        }

        private void DgvAlternative(int totalAlt)
        {
            DataGridView dgv = new DataGridView();
            dgv.Name = "dgvAlternatives";
            dgv.BackgroundColor = SystemColors.ControlLightLight;
            dgv.GridColor = SystemColors.Control;
            dgv.ForeColor = SystemColors.ControlText;
            dgv.ColumnHeadersVisible = false;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;

            //dgv.DataBindings = "a";
            DataGridViewRow dgvr = new DataGridViewRow();
            for (int i = 1; i <= 4; i++)
            {
                var ButtonColumn = new DataGridViewButtonColumn();

                dgv.Columns.Add(ButtonColumn);
                ButtonColumn.DefaultCellStyle = new DataGridViewCellStyle()
                {
                    ForeColor = Color.Black
                };
            }

            this.tableLayoutPanel1.Controls.Add(dgv, 0, 0);
            dgv.Dock = DockStyle.Fill;
            dgv.Rows.Add();
            int row = 1;
            int cell = 0;
            for (int i = 1; i <= totalAlt - 4; i++)
            {
                if (i % 4 == 0)
                {
                    dgv.Rows.Add();

                }
            }


            //dgv.CellContentClick += dgv_CellContentClick;
        }

        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (seq != null)
            {
                var a = seq.Where(x => x.Item1.Equals("1"));
                if (a.Any())
                {
                    var b = a.ToList()[0].Item2.ToList();
                    
                }
            }
        }

        private void DrawDiagram(string zoneID)
        {
            Ndd2.ActiveLayer.RemoveAllChildren();
            layer2 = new NLayer();
            Ndd2.Layers.AddChild(layer2);

            NLayoutContext layoutContext = new NLayoutContext();
            layoutContext.GraphAdapter = new NShapeGraphAdapter();
            layoutContext.BodyAdapter = new NShapeBodyAdapter(Ndd2);
            layoutContext.BodyContainerAdapter = new NDrawingBodyContainerAdapter(Ndd2);

            var alt1 = seq.Where(x => x.Item1.Equals(zoneID)).Select(x => x.Item2).ToList();   //add except zones ID at the end of sequence
            var except = node.Except(alt1[0]);
            if (except.Any())
            {
                foreach (var val in except)
                {
                    alt1[0].Add(val.ToString());
                }
            }

            var floor_ = dtSourceMain.AsEnumerable().Select(s => s.Field<string>("floor")).Distinct().ToList();
            if (floor_.Any())
            {
                if (ZonesLayout != null)
                {
                    foreach (var f in floor_)
                    {
                        NGroup level = new NGroup();
                        level.Name = f.ToString() + "f";
                        existingFloor.Add(level.Name);

                        List<NRectangleShape> zones = new List<NRectangleShape>();
                        foreach (var r in alt1[0])
                        {
                            var row = dtSourceMain.Select("ID =" + r.ToString() + " And floor =" + f.ToString()).SingleOrDefault();
                            if(row != null)
                                zones.Add(GetShape(row));
                        }
                        foreach (NRectangleShape z in zones)
                        {
                            level.Shapes.AddChild(z);
                        }
                        Ndd2.ActiveLayer.AddChild(level);
                        NNodeList listedNode = new NNodeList();
                            foreach (NRectangleShape n in level.Descendants(NFilters.Shape2D, -1))
                            {
                                listedNode.Add(n);
                            }
                            ZonesLayout.Layout(listedNode, layoutContext);
                            level.UpdateModelBounds();

                            NRectangleShape frame = new NRectangleShape(level.Bounds.X, level.Bounds.Y, level.Width, level.Height);
                            //CreateDecorators(frame, newGroup.Name);
                            frame.Protection = new NAbilities(AbilitiesMask.Select);
                            frame.Style.FillStyle = new NColorFillStyle(Color.Transparent);
                            frame.Style.StrokeStyle = new NStrokeStyle(Color.Gray);
                            level.Shapes.AddChild(frame);


                            NAbilities protection1 = frame.Protection;
                            protection1.InplaceEdit = true;
                            frame.Protection = protection1;
                            
                    }
                    NNodeList listedFloor = new NNodeList();
                    foreach (string l in existingFloor)
                    {
                        listedFloor.Add(Ndd2.ActiveLayer.GetChildByName(l));
                    }
                    FloorLayout.Layout(listedFloor, layoutContext);
                    Ndd2.SmartRefreshAllViews();
                }
            }
        }

        private NRectangleShape GetShape(DataRow dr)
        {
            float width = 0;
            float height = 0;
            width = (float)Math.Sqrt(Convert.ToDouble(dr[3].ToString()) * 2);
            height = (float)(width / 2);
            Color color1 = Color.FromName(dr[2].ToString());
            Color color2 = Color.Black;

            NRectangleShape zone = new NRectangleShape();
            zone = new NRectangleShape(0, 0, width, height);
            zone.Name = dr[0].ToString();
            existingZone.Add(zone.Name);
            zone.Style.FillStyle = new NColorFillStyle(color1);
            zone.Style.StrokeStyle = new NStrokeStyle(color2);
            string NodeLabelIn = dr[0].ToString()
               + System.Environment.NewLine + dr[3].ToString() + " " + "m\u00b2";
            string NodeLabelOut = dr[1].ToString();
            zone.Text = NodeLabelIn;
            zone.CreateShapeElements(ShapeElementsMask.Ports);
            NDynamicPort port = new NDynamicPort(zone.UniqueId, ContentAlignment.MiddleCenter, DynamicPortGlueMode.GlueToContour);
            zone.Ports.AddChild(port);
            port.Name = "ZonePort";
            zone.Ports.DefaultInwardPortUniqueId = port.UniqueId;
            NAbilities protection = zone.Protection;
            protection.InplaceEdit = true;
            zone.Protection = protection;
            return zone;
        }

        private void LowerTable_and_DrawConnector()
        {
            HashSet<Tuple<string, string>> horizontal = new HashSet<Tuple<string, string>>();
            HashSet<Tuple<string, string>> vertical = new HashSet<Tuple<string, string>>();
            DataTable dtlower = new DataTable();
            dtlower.Columns.Add("Floor");
            dtlower.Columns.Add("Horizontal");
            dtlower.Columns.Add("Vertical");

            if (Ndd2 == null || Ndd2.ActiveLayer == null
                || Ndd2.ActiveLayer.Descendants(NFilters.Shape1D, -1).Count == 0 || Ndd2.ActiveLayer.Descendants(NFilters.Shape2D, -1).Count == 0)
            {
                foreach (DataRow dr in dtZoneRelationship.Rows)
                {
                    var l1 = dtSourceMain.AsEnumerable()
                       .Where(x => x["ID"].ToString() == dr[0].ToString())
                       .Select(s => s["floor"].ToString()).SingleOrDefault();
                    var l2 = dtSourceMain.AsEnumerable()
                     .Where(x => x["ID"].ToString() == dr[1].ToString())
                     .Select(s => s["floor"].ToString()).SingleOrDefault();
                    

                    var startGroup = Ndd2.ActiveLayer.GetChildByName(l1 + "f");
                    var endGroup = Ndd2.ActiveLayer.GetChildByName(l2 + "f");
                    if(startGroup is NGroup SG && endGroup is NGroup EG)
                    {
                        var startNode = SG.Shapes.GetChildByName(dr["StartNode"].ToString());
                        var endNode = EG.Shapes.GetChildByName(dr["EndNode"].ToString());
                        if (startNode is NRectangleShape SN && endNode is NRectangleShape EN)
                        {
                            NLineShape line = new NLineShape();
                            line.StyleSheetName = NDR.NameConnectorsStyleSheet;

                            line.Style.FillStyle = new NColorFillStyle(Color.Black);
                            line.Style.StrokeStyle = new NStrokeStyle(Color.Black);
                            line.Text = dr["Axis"].ToString() + System.Environment.NewLine + dr["Type"].ToString();
                            Ndd2.ActiveLayer.AddChild(line);
                            line.StartPlug.Connect(SN.Ports.GetChildByName("ZonePort", 0) as NPort);
                            line.EndPlug.Connect(EN.Ports.GetChildByName("ZonePort", 0) as NPort);
                        }
                    }
                   

                    if (l1 == l2)
                    {
                        // Add to horizontal list
                        horizontal.Add(Tuple.Create(l1, dr["StartNode"].ToString() + "-" + dr["EndNode"].ToString()));
                    }
                    else
                    {
                        // Add to vertical list
                        vertical.Add(Tuple.Create(l1, dr["StartNode"].ToString() + "-" + dr["EndNode"].ToString()));
                    }
                }


                var floor = dtSourceMain.AsEnumerable().Select(s => s.Field<string>("floor")).Distinct().ToList();
                if (floor.Any())
                {
                    foreach (var f in floor)
                    {
                        var H = horizontal.Where(x => x.Item1.Equals(f)).Select(x => x.Item2).ToList();
                        var V = vertical.Where(x => x.Item1.Equals(f)).Select(x => x.Item2).ToList();
                        DataRow dr = dtlower.NewRow();
                        dr["Floor"] = f + "f";
                        dr["Horizontal"] = string.Join(",", H);
                        dr["Vertical"] = string.Join(",", V);
                        dtlower.Rows.Add(dr);
                    }
                }
                Ndd2.SmartRefreshAllViews();

                dtlower.AcceptChanges();
                dgvZoneRelationship.DataSource = dtlower;
            }
        }

        private void dt_ZoneRelationship()
        {
            string DataSourceInputData = StaticCache.DataSourceZoneRelationShip;
            if (!File.Exists(DataSourceInputData))
            {
                MessageBox.Show("Please make and configure a setting to initialize dbsource file having path " + DataSourceInputData + "." + Environment.NewLine + "Source File have been put at Project's Datasource Folder.");
                return;
            }
            dtZoneRelationship = new DataTable();
            dtZoneRelationship.Columns.Add("StartNode");
            dtZoneRelationship.Columns.Add("EndNode");
            dtZoneRelationship.Columns.Add("Axis");
            dtZoneRelationship.Columns.Add("Type");

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
                    ds.Tables["Input Data_Module"].Rows.RemoveAt(0);
                    foreach (DataRow dr in ds.Tables["Input Data_Module"].Rows)
                    {
                        dtZoneRelationship.Rows.Add(dr.ItemArray);
                    }
                }
                dtZoneRelationship.AcceptChanges();
            }
        }

        private void dtSource()
        {
            string DataSourceInputData = StaticCache.DataSource_Test;
            if (!File.Exists(DataSourceInputData))
            {
                MessageBox.Show("Please make and configure a setting to initialize dbsource file having path " + DataSourceInputData + "." + Environment.NewLine + "Source File have been put at Project's Datasource Folder.");
                return;
            }
            dtSourceMain = new DataTable();
            dtSourceMain.Columns.Add("ID");
            dtSourceMain.Columns.Add("module name");
            dtSourceMain.Columns.Add("color");
            dtSourceMain.Columns.Add("area");
            dtSourceMain.Columns.Add("height");
            dtSourceMain.Columns.Add("floor");

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
                    ds.Tables["Input Data_Module"].Rows.RemoveAt(0);
                    foreach (DataRow dr in ds.Tables["Input Data_Module"].Rows)
                    {
                        dtSourceMain.Rows.Add(dr.ItemArray);
                    }
                }
                dtSourceMain.AcceptChanges();
            }
        }
    }
}
