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
        public int floor1Area = 0;
        public int floor2Area = 0;
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
        List<(List<(int, int)>, List<(int, int)>)> FinalResult = new List<(List<(int, int)>, List<(int, int)>)>();

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
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.ColumnHeadersVisible = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.BorderStyle = BorderStyle.None;

            dgvZoneRelationship.Enabled = false;

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
                floor1Area = 16786;
                floor2Area = 10108;

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
                if(nodeAreas.Count > 0)
                {
                    List<(List<(int, int)>, List<(int, int)>)> permutations = GeneratePermutations(floor1Area, floor2Area, nodeAreas);
                    if (permutations != null)
                    {
                        List<(List<(int, int)>, List<(int, int)>)> filterLevels = GenerateWithLevels(permutations);
                        if (filterLevels != null)
                        {
                            FinalResult = GenerateWithVertical(filterLevels);
                            if (FinalResult != null)
                            {
                                Bind_UpperPanel(FinalResult);
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("There is no connection record!");
                return;
            }
        }

        private void Bind_UpperPanel(List<(List<(int, int)>, List<(int, int)>)> result)
        {
           
            if(result.Count > 0)
            {
                int buttonCount = result.Count(); // Number of buttons to generate
                int buttonsPerRow = 4;
                int rowCount = (int)Math.Ceiling((double)buttonCount / buttonsPerRow);
                for (int row = 0; row < rowCount; row++)
                {
                    DataGridViewRow dataGridViewRow = new DataGridViewRow();
                    dataGridViewRow.CreateCells(dataGridView1);

                    for (int col = 0; col < buttonsPerRow; col++)
                    {
                        int buttonIndex = (row * buttonsPerRow) + col;
                        if (buttonIndex < buttonCount)
                        {
                            Button button = new Button();
                            button.Text = "Set Text";
                            dataGridViewRow.Cells[col] = new DataGridViewButtonCell()
                            {
                                Value = "Alt" + (col+1).ToString()
                            };
                            button.Click += btnAlt_Clicked;
                        }
                    }

                    dataGridView1.Rows.Add(dataGridViewRow);
                }
            }
           
        }

        private void btnAlt_Clicked(object sender, EventArgs e)
        {
            //(List<(int, int)>, List<(int, int)>) result = new(List<(int, int)>, List<(int, int)>);
            string text = (sender as Button).Text as string;
            if (!string.IsNullOrWhiteSpace(text))
            {
                var seq = text.Remove(0, 3);
                var result = FinalResult.ElementAt(Convert.ToInt32(seq));
                Ndd2.ActiveLayer.RemoveAllChildren();
                existingFloor = new List<string>();
                Ndd2.SmartRefreshAllViews();
                //DrawDiagram(seq); //Draw at diagram
                LowerTable_and_DrawConnector(result); // bind data at Lower table
            }
                
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

        private void LowerTable_and_DrawConnector((List<(int, int)>, List<(int, int)>) result)
        {
            HashSet<Tuple<string, string>> horizontal = new HashSet<Tuple<string, string>>();
            HashSet<Tuple<string, string>> vertical = new HashSet<Tuple<string, string>>();
            DataTable dtlower = new DataTable();
            dtlower.Columns.Add("Floor");
            dtlower.Columns.Add("Horizontal");
            dtlower.Columns.Add("Vertical");
            dtlower.Columns.Add("TotalArea");
            dtlower.Columns.Add("ExtraArea");

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
                        dr["TotalArea"] =  (floor1Area - result.Item1.Sum(x=>x.Item1)).ToString();
                        dr["ExtraArea"] = (floor2Area - result.Item2.Sum(x => x.Item1)).ToString();
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
            var totalFloor = dtSourceMain.Select("Floor <> ''").Select(r => r[9].ToString()).Distinct().ToList();
            List<(List<(int, int)>, List<(int, int)>)> validResult = validPermutations;

            if (totalFloor.Any())
            {
                foreach (var n in node)  //select sequences that contain all Zones
                {
                    validResult = validResult.Where(x =>
                        x.Item1.Select(y => y.Item2).Union(x.Item2.Select(y => y.Item2)).Contains(Convert.ToInt32(n))
                    ).ToList();
                }

                foreach (string f in totalFloor)
                {
                    if (f.Equals("1"))
                    {
                        List<int> selectedZones = dtSourceMain.Select("Floor = '" + f + "'")
                            .Select(r => Convert.ToInt32(r[0].ToString())).Distinct().ToList();

                        validResult = validResult.Where(x =>
                            selectedZones.All(zone => x.Item1.Select(y => y.Item2).Contains(zone))
                        ).ToList();
                    }
                    else if (f.Equals("2"))
                    {
                        List<int> selectedZones = dtSourceMain.Select("Floor = '" + f + "'")
                            .Select(r => Convert.ToInt32(r[0].ToString())).Distinct().ToList();

                        validResult = validResult.Where(x =>
                            selectedZones.All(zone => x.Item2.Select(y => y.Item2).Contains(zone))
                        ).ToList();
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
                            //foreach (var f in totalFloor)
                            //{
                            //    if (f.Equals("1"))
                            //    {
                            //        List<int> selectedZones = vertical.Where(x => x.Item1.Equals(f)).Select(y => Convert.ToInt32(y.Item2)).ToList();
                            //        foreach (int r in selectedZones)
                            //        {
                            //            validResult = new List<(List<(int, int)>, List<(int, int)>)>(validResult.Where(x => x.Item1.Select(y => y.Item2).Contains(r)));
                            //        }
                            //        // validResult = new List<(List<(int, int)>, List<(int, int)>)>(validResult.Where(x=>x.Item1.Except(selectedZones)));
                            //    }
                            //    else if (f.Equals("2"))
                            //    {
                            //        List<int> selectedZones = vertical.Where(x => x.Item1.Equals(f)).Select(y => Convert.ToInt32(y.Item2)).ToList();
                            //        foreach (int r in selectedZones)
                            //        {
                            //            validResult = new List<(List<(int, int)>, List<(int, int)>)>(validResult.Where(x => x.Item2.Select(y => y.Item2).Contains(r)));
                            //        }
                            //        //validResult = new List<(List<(int, int)>, List<(int, int)>)>(validResult.Where(x => x.Item1.Select(y => y.Item1.Any())));
                            //    }

                            //}
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
                                        var floorValue = result.First();

                                        if (floorValue == "1")
                                        {
                                            validResult = new List<(List<(int, int)>, List<(int, int)>)>(validResult.Where(x => x.Item1.Select(y => y.Item2).Contains(Convert.ToInt32(i))));
                                        }
                                        else if (floorValue == "2")
                                        {
                                            validResult = new List<(List<(int, int)>, List<(int, int)>)>(validResult.Where(x => x.Item2.Select(y => y.Item2).Contains(Convert.ToInt32(i))));
                                        }
                                        else
                                        {
                                            // Filter permutations where node 'i' is in the same floor as the connected node
                                            validResult = new List<(List<(int, int)>, List<(int, int)>)>(validResult.Where(x =>
                                                (x.Item1.Select(y => y.Item2).Contains(Convert.ToInt32(i)) && x.Item1.Select(y => y.Item1).Contains(Convert.ToInt32(result.First()))) ||
                                                (x.Item2.Select(y => y.Item2).Contains(Convert.ToInt32(i)) && x.Item2.Select(y => y.Item1).Contains(Convert.ToInt32(result.First())))
                                            ));
                                        }
                                    }
                                }
                            }
                        }
                        
                    }
                    return validResult;
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
