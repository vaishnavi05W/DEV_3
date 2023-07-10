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
using Nevron.Diagram.Extensions;
using Nevron.UI.WinForm.Controls;

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
        NLayoutContext layoutContext;
        NFlowLayout ZonesLayout;
        NFlowLayout FloorLayout;
        private NPersistencyManager persistencyManager;
        private NGroupBox groupPropertiesGroup;
        private NCheckBox autoDestroyCheckBox;
        private NCheckBox canBeEmptyCheckBox;

        public DataTable dtZoneRelationship;
        public DataTable dtSourceMain;
        List<string> node = new List<string>();
        List<int> Nodes_1F = new List<int>();
        List<int> Nodes_2F = new List<int>();
        private HashSet<Tuple<string, List<string>>> seq = new HashSet<Tuple<string, List<string>>>();
        public GenerativeInfomationForms(DataTable dt_Main, DataTable dt_ZoneRelationship)
        {
            InitializeComponent();
            this.persistencyManager = new NPersistencyManager();
            this.groupPropertiesGroup = new NGroupBox();
            this.autoDestroyCheckBox = new NCheckBox();
            this.canBeEmptyCheckBox = new NCheckBox(); this.groupPropertiesGroup.SuspendLayout();
            this.SuspendLayout();
            dtSourceMain = dt_Main;
            dtZoneRelationship = dt_ZoneRelationship;
            this.Load += IS_Load;
        }

        private void IS_Load(object sender, EventArgs args)
        {
            ///dtSource();
           // dt_ZoneRelationship();
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

            layer2 = new NLayer();
            Ndd2.Layers.AddChild(layer2);

            layoutContext = new NLayoutContext();
            layoutContext.GraphAdapter = new NShapeGraphAdapter();
            layoutContext.BodyAdapter = new NShapeBodyAdapter(Ndd2);
            layoutContext.BodyContainerAdapter = new NDrawingBodyContainerAdapter(Ndd2);

            node = dtSourceMain.AsEnumerable()
                .Select(s => s.Field<string>("ID"))
                .Distinct()
                .ToList();

            if (dtSourceMain.Rows.Count > 0)
            {
                int floor1Area = 16786;
                int floor2Area = 10108;

                //Permutation p = new Permutation(node.Count);
                //var result = p.Successor();
                
                //List<double> node_areas = new List<double>();

                //foreach (DataRow dr in dtSourceMain.Rows)
                //{
                //    node_areas.Add(Convert.ToDouble(Convert.ToDouble(dr[5].ToString().Replace(",", ""))));
                //}
                //var res = CalculateValidPermutations(node_areas.ToArray(), Convert.ToDouble(floor1Area), Convert.ToDouble(floor2Area));

                List<(int,int)> nodeAreas = new List<(int, int)>();
                foreach (DataRow dr in dtSourceMain.Rows)
                {
                    nodeAreas.Add((Convert.ToInt32(Convert.ToDouble(dr[5].ToString().Replace(",", ""))), Convert.ToInt32(dr[0].ToString().Replace(",", ""))));
                }

                List<(List<(int, int)>, List<(int, int)>)> permutations = GeneratePermutations(floor1Area, floor2Area, nodeAreas);
                if (permutations != null)
                {

                    List<(List<(int, int)>, List<(int, int)>)> filterLevels = GenerateWithLevels(permutations);
                    if(filterLevels != null)
                    {
                        List<(List<(int, int)>, List<(int, int)>)> filterVertical = GenerateWithVertical(filterLevels);
                    }
                }
                

                dgvZoneRelationship.Enabled = false;

                Bind_UpperPanel();
            }
            else
            {
                MessageBox.Show("There is no connection record!");
                return;
            }
        }

        private void Bind_UpperPanel()
        {
            DataGridView dgvUpper = new DataGridView();
            
            //Button btn1 = new Button();
            //btn1.Text = "Alt1";
            //btn1.Name = "Alt1";
            //btn1.ForeColor = Color.Black;
            //btn1.Dock = DockStyle.Fill;

            //Button btn2 = new Button();
            //btn2.Text = "Alt2";
            //btn2.Name = "Alt2";
            //btn2.ForeColor = Color.Black;
            //btn2.Dock = DockStyle.Fill;

            //Button btn3 = new Button();
            //btn3.Text = "Alt3";
            //btn3.Name = "Alt3";
            //btn3.ForeColor = Color.Black;
            //btn3.Dock = DockStyle.Fill;

            //Button btn4 = new Button();
            //btn4.Text = "Alt4";
            //btn4.Name = "Alt4";
            //btn4.ForeColor = Color.Black;
            //btn4.Dock = DockStyle.Fill;

            //Button btn5 = new Button();
            //btn5.Text = "Alt5";
            //btn5.Name = "Alt5";
            //btn5.ForeColor = Color.Black;
            //btn5.Dock = DockStyle.Fill;

            //Button btn6 = new Button();
            //btn6.Text = "Alt6";
            //btn6.Name = "Alt6";
            //btn6.ForeColor = Color.Black;
            //btn6.Dock = DockStyle.Fill;

            //Button btn7 = new Button();
            //btn7.Text = "Alt7";
            //btn7.Name = "Alt7";
            //btn7.ForeColor = Color.Black;
            //btn7.Dock = DockStyle.Fill;

            //Button btn8 = new Button();
            //btn8.Text = "Alt8";
            //btn8.Name = "Alt8";
            //btn8.ForeColor = Color.Black;
            //btn8.Dock = DockStyle.Fill;

            //Button btn9 = new Button();
            //btn9.Text = "Alt9";
            //btn9.Name = "Alt9";
            //btn9.ForeColor = Color.Black;
            //btn9.Dock = DockStyle.Fill;

            //btn1.Click += btnAlt_Clicked;
            //btn2.Click += btnAlt_Clicked;
            //btn3.Click += btnAlt_Clicked;
            //btn4.Click += btnAlt_Clicked;
            //btn5.Click += btnAlt_Clicked;
            //btn6.Click += btnAlt_Clicked;
            //btn7.Click += btnAlt_Clicked;
            //btn8.Click += btnAlt_Clicked;
            //btn9.Click += btnAlt_Clicked;

            //TableLayoutPanel panel = new TableLayoutPanel();
            //panel.ColumnCount = 3;
            
            //panel.RowCount = 3;
            //panel.Height = 90;
            //panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            //panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            //panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            //panel.Controls.Add(btn1, 0, 0);
            //panel.Controls.Add(btn2, 1, 0);
            //panel.Controls.Add(btn3, 3, 0);
            //panel.Controls.Add(btn4, 0, 1);
            //panel.Controls.Add(btn5, 1, 1);
            //panel.Controls.Add(btn6, 3, 1);
            //panel.Controls.Add(btn7, 0, 2);
            //panel.Controls.Add(btn8, 1, 2);
            //panel.Controls.Add(btn9, 3, 2);
            ////panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 2F));
            ////panel.Controls.Add(new Button() { Text = "Alt1", Name = "Alt1",  ForeColor = Color.Black, Dock = DockStyle.Fill }, 0, 0);
            ////panel.Controls.Add(new Button() { Text = "Alt2", Name = "Alt2", ForeColor = Color.Black, Dock = DockStyle.Fill }, 1, 0);
            ////panel.Controls.Add(new Button() { Text = "Alt3", Name = "Alt3", ForeColor = Color.Black, Dock = DockStyle.Fill }, 3, 0);
            //this.tableLayoutPanel1.Controls.Add(panel, 0, 0);
           // panel.Dock = DockStyle.Top;
        }

        private void btnAlt_Clicked(object sender, EventArgs e)
        {
            string text = (sender as Button).Text as string;
            if (text.Equals("Alt1"))
            {
                Ndd2.ActiveLayer.RemoveAllChildren();
                existingFloor = new List<string>();
                Ndd2.SmartRefreshAllViews();
                DrawDiagram("1");
                LowerTable_and_DrawConnector();
            }
            else if (text.Equals("Alt2"))
            {
                Ndd2.ActiveLayer.RemoveAllChildren();
                existingFloor = new List<string>();
                Ndd2.SmartRefreshAllViews();
                DrawDiagram("2");
                LowerTable_and_DrawConnector();
            }
            else if (text.Equals("Alt3"))
            {
                Ndd2.ActiveLayer.RemoveAllChildren();
                existingFloor = new List<string>();
                Ndd2.SmartRefreshAllViews();
                DrawDiagram("3");
                LowerTable_and_DrawConnector();
            }
            else if (text.Equals("Alt4"))
            {
                Ndd2.ActiveLayer.RemoveAllChildren();
                existingFloor = new List<string>();
                Ndd2.SmartRefreshAllViews();
                DrawDiagram("4");
                LowerTable_and_DrawConnector();
            }
            else if (text.Equals("Alt5"))
            {
                Ndd2.ActiveLayer.RemoveAllChildren();
                existingFloor = new List<string>();
                Ndd2.SmartRefreshAllViews();
                DrawDiagram("5");
                LowerTable_and_DrawConnector();
            }
            else if (text.Equals("Alt6"))
            {
                Ndd2.ActiveLayer.RemoveAllChildren();
                existingFloor = new List<string>();
                Ndd2.SmartRefreshAllViews();
                DrawDiagram("6");
                LowerTable_and_DrawConnector();
            }
            else if (text.Equals("Alt7"))
            {
                Ndd2.ActiveLayer.RemoveAllChildren();
                existingFloor = new List<string>();
                Ndd2.SmartRefreshAllViews();
                DrawDiagram("7");
                LowerTable_and_DrawConnector();
            }
            else if (text.Equals("Alt8"))
            {
                Ndd2.ActiveLayer.RemoveAllChildren();
                existingFloor = new List<string>();
                Ndd2.SmartRefreshAllViews();
                DrawDiagram("8");
                LowerTable_and_DrawConnector();
            }
            else if (text.Equals("Alt9"))
            {
                Ndd2.ActiveLayer.RemoveAllChildren();
                existingFloor = new List<string>();
                Ndd2.SmartRefreshAllViews();
                DrawDiagram("9");
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
                        Ndd2.ActiveLayer.AddChild(level);
                        List<NRectangleShape> zones = new List<NRectangleShape>();
                        foreach (var r in alt1[0])
                        {
                            var row = dtSourceMain.Select("ID =" + r.ToString() + " And floor =" + f.ToString()).SingleOrDefault();
                            if (row != null)
                                zones.Add(GetShape(row));
                        }
                        foreach (NRectangleShape z in zones)
                        {
                            level.Shapes.AddChild(z);
                        }

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
            width = (float)Math.Sqrt(Convert.ToDouble(dr[5].ToString()) * 2);
            height = (float)(width / 2);
            Color color1 = Color.FromName(dr[4].ToString());
            Color color2 = Color.Black;

            NRectangleShape zone = new NRectangleShape();
            zone = new NRectangleShape(0, 0, width, height);
            zone.Name = dr[0].ToString();
            existingZone.Add(zone.Name);
            zone.Style.FillStyle = new NColorFillStyle(color1);
            zone.Style.StrokeStyle = new NStrokeStyle(color2);
            string NodeLabelIn = dr[0].ToString()
               + System.Environment.NewLine + dr[5].ToString() + " " + "m\u00b2";
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
                    if (startGroup is NGroup SG && endGroup is NGroup EG)
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

        //Case-1
        public static List<(List<(int, int)>, List<(int, int)>)> GeneratePermutations(int floor1Area, int floor2Area, List<(int,int)> nodes)
        {
            List<(List<(int, int)>, List<(int, int)>)> validPermutations = new List<(List<(int, int)>, List<(int, int)>)>();

            void Backtrack(List<(int, int)> floor1Nodes, List<(int, int)> floor2Nodes, List<(int, int)> remainingNodes)
            {
                if (remainingNodes.Count == 0)
                {
                    validPermutations.Add((floor1Nodes, floor2Nodes));
                    return;
                }

                (int,int) currentNode = remainingNodes[0];
                List<(int, int)> remaining = new List<(int, int)>(remainingNodes);
                remaining.RemoveAt(0);

                // Try placing the current node on the first floor
                if (currentNode.Item1 <= floor1Area)
                {
                    List<(int, int)> newFloor1Nodes = new List<(int, int)>(floor1Nodes);
                    newFloor1Nodes.Add(currentNode);
                    //floor1Area = floor1Area - currentNode.Item1;
                    List<(int, int)> newFloor2Nodes = new List<(int, int)>(floor2Nodes);
                    Backtrack(newFloor1Nodes, newFloor2Nodes, remaining);
                }

                // Try placing the current node on the second floor
                 if (currentNode.Item1 <= floor2Area)
                {
                    List<(int, int)> newFloor1Nodes = new List<(int, int)>(floor1Nodes);
                    List<(int, int)> newFloor2Nodes = new List<(int, int)>(floor2Nodes);
                    newFloor2Nodes.Add(currentNode);
                    //floor2Area = floor2Area - currentNode.Item1;
                    Backtrack(newFloor1Nodes, newFloor2Nodes, remaining);
                }

                // Try skipping the current node
                List<(int, int)> newFloor1NodesSkip = new List<(int, int)>(floor1Nodes);
                List<(int, int)> newFloor2NodesSkip = new List<(int, int)>(floor2Nodes);
                Backtrack(newFloor1NodesSkip, newFloor2NodesSkip, remaining);
            }

            Backtrack(new List<(int, int)>(), new List<(int, int)>(), nodes);

            validPermutations = new List<(List<(int, int)>, List<(int, int)>)>(validPermutations.Where(x => x.Item1.Select(y => y.Item1).Sum() <= floor1Area && x.Item2.Select(y => y.Item1).Sum() <= floor2Area));
            return validPermutations;
        }

        //Case-2
        private List<(List<(int, int)>, List<(int, int)>)> GenerateWithLevels(List<(List<(int, int)>, List<(int, int)>)> validPermutations)
        {
            //validPermutations.RemoveAll(x => x.Item1.Count() + x.Item2.Count() < dtSourceMain.Rows.Count);
            List<(List<(int, int)>, List<(int, int)>)> validResult = new List<(List<(int, int)>, List<(int, int)>)>();
            validResult = validPermutations;
            var totalFloor = dtSourceMain.Select("Floor <> ''").Select(r => r[9].ToString()).Distinct().ToList();
            if (totalFloor.Any())
            {
                foreach(var n in node)  //select sequences that contains all Zones
                {
                    validResult = new List<(List<(int, int)>, List<(int, int)>)>(validResult.Where(x => x.Item1.Select(y => y.Item2).Union(x.Item2.Select(y => y.Item2)).Contains(Convert.ToInt32(n))));
                }
                

                foreach (string f in totalFloor)
                {

                    if (f.Equals("1"))
                    {
                        List<int> selectedZones = dtSourceMain.Select("Floor = '" + f + "'").Select(r => Convert.ToInt32(r[0].ToString())).Distinct().ToList();
                        foreach (int r in selectedZones)
                        {
                            validResult = new List<(List<(int, int)>, List<(int, int)>)>(validResult.Where(x => x.Item1.Select(y => y.Item2).Contains(r)));
                        }
                        //validResult = new List<(List<(int, int)>, List<(int, int)>)>(validResult.Where(x => x.Item1.Select(y => y.Item2).Intersect(selectedZones).Any()));
                    }
                    else if (f.Equals("2"))
                    {
                        List<int> selectedZones = dtSourceMain.Select("Floor = '" + f + "'").Select(r => Convert.ToInt32(r[0].ToString())).Distinct().ToList();
                        foreach (int r in selectedZones)
                        {
                            validResult = new List<(List<(int, int)>, List<(int, int)>)>(validResult.Where(x => x.Item2.Select(y => y.Item2).Contains(r)));
                        }
                        //validResult = new List<(List<(int, int)>, List<(int, int)>)>(validResult.Where(x => x.Item2.Select(y => y.Item2).Intersect(selectedZones).Any()));
                    }
                }
            }
            return validResult;
        }

        //Case-3
        private List<(List<(int, int)>, List<(int, int)>)> GenerateWithVertical(List<(List<(int, int)>, List<(int, int)>)> validPermutations)
        {
            List<(List<(int, int)>, List<(int, int)>)> validResult = new List<(List<(int, int)>, List<(int, int)>)>();
            try
            {
                validResult = validPermutations;
                if (dtZoneRelationship.Rows.Count > 0)
                {
                    HashSet<Tuple<string, string>> vertical = new HashSet<Tuple<string, string>>();
                    foreach (DataRow r in dtZoneRelationship.Rows)
                    {
                        if (r[2].ToString() == "Vertical") //get data if it's ony vertical
                        {
                            for (int i = 0; i <= 1; i++)
                            {
                                var floor = dtSourceMain.AsEnumerable()
                                    .Where(x => x.Field<string>("ID").Equals(r[i].ToString()))
                                    .Select(y => y.Field<string>("Floor")).SingleOrDefault();
                                if (floor != null)
                                {
                                    vertical.Add(Tuple.Create(floor.ToString(), r[i].ToString()));
                                }
                                else
                                {
                                    vertical.Add(Tuple.Create(string.Empty, r[i].ToString()));
                                }
                            }
                        }

                    }
                    if (vertical.Count() > 0)  //Filter Zones with Vertical connections
                    {
                        var totalFloor = dtSourceMain.Select("Floor <> ''").Select(r => r[9].ToString()).Distinct().ToList();
                        if (totalFloor.Any())
                        {
                            foreach (var f in totalFloor)
                            {
                                if (f.Equals("1"))
                                {
                                    List<int> selectedZones = vertical.Where(x => x.Item1.Equals(f)).Select(y => Convert.ToInt32(y.Item2)).ToList();
                                    foreach (int r in selectedZones)
                                    {
                                        validResult = new List<(List<(int, int)>, List<(int, int)>)>(validResult.Where(x => x.Item1.Select(y => y.Item2).Contains(r)));
                                    }
                                    // validResult = new List<(List<(int, int)>, List<(int, int)>)>(validResult.Where(x=>x.Item1.Except(selectedZones)));
                                }
                                else if (f.Equals("2"))
                                {
                                    List<int> selectedZones = vertical.Where(x => x.Item1.Equals(f)).Select(y => Convert.ToInt32(y.Item2)).ToList();
                                    foreach (int r in selectedZones)
                                    {
                                        validResult = new List<(List<(int, int)>, List<(int, int)>)>(validResult.Where(x => x.Item2.Select(y => y.Item2).Contains(r)));
                                    }
                                    //validResult = new List<(List<(int, int)>, List<(int, int)>)>(validResult.Where(x => x.Item1.Select(y => y.Item1.Any())));
                                }

                            }
                            List<string> noFloorZones = vertical.Where(x => string.IsNullOrWhiteSpace(x.Item1)).Select(y => (y.Item2)).ToList();

                            if (noFloorZones.Any()) //For Zones that has no Floor
                            {
                                foreach (string i in noFloorZones)
                                {
                                    List<string> result = new List<string>();
                                    List<string> a = dtZoneRelationship.AsEnumerable().Where(y => y.Field<string>("EndNode").Equals(i)).Select(x => x.Field<string>("StartNode")).ToList();
                                    if (a.Any())
                                    {
                                        List<string> b = new List<string>();
                                        foreach (string v in a)
                                        {

                                            b.Add(dtSourceMain.AsEnumerable().Where(y => y.Field<string>("ID").Equals(v)).Select(x => x.Field<string>("Floor")).SingleOrDefault());

                                        }
                                        if (b.Any())
                                        {
                                            result = totalFloor.Except(b).ToList();
                                        }
                                    }
                                    if (result.Any())
                                    {
                                        if (result.First().Equals("1"))
                                        {
                                            validResult = new List<(List<(int, int)>, List<(int, int)>)>(validResult.Where(x => x.Item1.Select(y => y.Item2).Contains(Convert.ToInt32(i))));
                                        }
                                        else if (result.First().Equals("2"))
                                        {
                                            validResult = new List<(List<(int, int)>, List<(int, int)>)>(validResult.Where(x => x.Item2.Select(y => y.Item2).Contains(Convert.ToInt32(i))));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return validResult;
        }


        public static long CalculateValidPermutations(double[] roomAreas, double firstFloorArea, double secondFloorArea)
        {
            int totalRooms = roomAreas.Length;

            long numPermutations = 0;

            for (int i = 1; i < (1 << totalRooms); i++)
            {
                double firstFloorTotalArea = 0;
                double secondFloorTotalArea = 0;

                int firstFloorRooms = 0;
                int secondFloorRooms = 0;

                for (int j = 0; j < totalRooms; j++)
                {
                    if (firstFloorTotalArea + roomAreas[j] <= firstFloorArea)
                    {
                        firstFloorTotalArea += roomAreas[j];
                        firstFloorRooms++;
                    }
                    else
                    {
                        secondFloorTotalArea += roomAreas[j];
                        secondFloorRooms++;
                    }
                }

                if (firstFloorTotalArea <= firstFloorArea && secondFloorTotalArea <= secondFloorArea)
                {
                    long permutationsForDistribution = CalculatePermutations(totalRooms, firstFloorRooms, secondFloorRooms);
                    numPermutations += permutationsForDistribution;
                }
            }

            return numPermutations;
        }

        public static long CalculatePermutations(int totalRooms, int firstFloorRooms, int secondFloorRooms)
        {
            long numerator = Factorial(totalRooms);
            long denominator = Factorial(firstFloorRooms) * Factorial(secondFloorRooms);
            long numPermutations = numerator / denominator;

            return numPermutations;
        }

        public static long Factorial(int number)
        {
            long factorial = 1;

            for (int i = 2; i <= number; i++)
            {
                factorial *= i;
            }

            return factorial;
        }

    }

}
