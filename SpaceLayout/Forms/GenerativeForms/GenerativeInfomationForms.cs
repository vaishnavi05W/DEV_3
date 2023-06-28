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

namespace SpaceLayout.Forms.GenerativeForms
{
    public partial class GenerativeInfomationForms : UserControl
    {
        public DataTable dtZoneRelationship;
        public DataTable dtSourceMain;
        public GenerativeInfomationForms()
        {
            InitializeComponent();
            this.Load += IS_Load;
        }

        private void IS_Load(object sender, EventArgs args)
        {
            dtSource();
            dtZoneReationship();
            //for (int i = 1; i < 2; i++)
            //{
            //    Button btn = new Button();
            //    btn.Text = "Alt" + i.ToString();
            //    //btn.BackColor = Color.White;
            //    btn.ForeColor = Color.Navy;
            //    this.tableLayoutPanel1.Controls.Add(btn, 0, 0);
            //}

            DataGridView dgv = new DataGridView();
            dgv.Name = "dgvAlternatives";
            dgv.BackgroundColor = SystemColors.ControlLightLight;
            dgv.GridColor = SystemColors.Control;
            dgv.ForeColor = SystemColors.ControlText;
            dgv.ColumnHeadersVisible = false;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            //dgv.DefaultCellStyle = new DataGridViewCellStyle()
            //{

            //};
            DataGridViewRow dgvr = new DataGridViewRow();
            for (int i = 1; i <= 4; i++)
            {
                var ButtonColumn = new DataGridViewButtonColumn();

                //  dgv.Rows.Add(ButtonColumn);
                dgv.Columns.Add(ButtonColumn);
                //ButtonColumn.Text = "Alt" + i.ToString();
                //ButtonColumn.UseColumnTextForButtonValue = false;
                ButtonColumn.DefaultCellStyle = new DataGridViewCellStyle()
                {
                    ForeColor = Color.Black
                };
            }

            this.tableLayoutPanel1.Controls.Add(dgv, 0, 0);
            dgv.Dock = DockStyle.Fill;
            ////dgv.Rows.Add(dgvr);
            //DataGridViewButtonCell btn = new DataGridViewButtonCell();
            //btn.te
            dgv.Rows.Add();
            int row = 1;
            int cell = 0;
            for (int i = 1; i <= 20 - 4; i++)
            {
                if (i % 4 == 0)
                {
                    dgv.Rows.Add();

                }
            }


            dgv.CellContentClick += dgv_CellContentClick;

            //Test_topology();
            var node = dtSourceMain.AsEnumerable()
                .Select(s => Convert.ToInt32(s.Field<string>("ID")))
                .Distinct()
                .ToList();

            HashSet<Tuple<int, int>> connection = new HashSet<Tuple<int, int>>();
            foreach (DataRow dr in dtZoneRelationship.Rows)
            {
                connection.Add(Tuple.Create(Convert.ToInt32(dr[0]), Convert.ToInt32(dr[1])));
            }

            var ret = TopologicalSort(new HashSet<int>(node), connection);

        }

        private void Test_topology()
        {
            int vertices = 5;
            int[,] edges = new int[,] { { 3, 2 }, { 3, 0 }, { 2, 0 }, { 2, 1 },{ 1, 0} };
            // Console.WriteLine(edges.GetLength(0));
            List<int> result = SortGraph(vertices, edges);
           
        }

        public static List<int> SortGraph(int vertices, int[,] edges)
        {
            // 0. Initialize Sorted List
            List<int> sortedOrder = new List<int>();
            if (vertices <= 0)
            {
                return sortedOrder;
            }

            // 1. Initialize the Graph (O(V))

            Dictionary<int, List<int>> graph = new Dictionary<int, List<int>>(); // key = node, values = list of it's adjacent nodes
            Dictionary<int, int> inDegrees = new Dictionary<int, int>(); // key = vertex, value = number of incoming edges

            for (int i = 0; i < vertices; i++)
            {
                inDegrees.Add(i, 0);
                graph.Add(i, new List<int>());
            }

            // 2. Build the Graph (O(E))

            for (int i = 0; i < edges.GetLength(0); i++)
            {
                int parent = edges[i, 0]; // left node of directed edge
                int child = edges[i, 1]; // right node of directed edge

                graph[parent].Add(child); // put the child into it's parent's adjacency list
                inDegrees[child] += 1;
            }

            // 3. Find all Sources and add to Queue

            Queue<int> sources = new Queue<int>();

            foreach (var entry in inDegrees)
            {
                if (entry.Value == 0)
                {
                    sources.Enqueue(entry.Key);
                }
            }

            // 4. Sort

            // For each Source, add it to Sorted Order
            while (sources.Count > 0)
            {
                int source = sources.Dequeue();
                sortedOrder.Add(source);

                // Subtract one from all of it's children's inDegrees value
                foreach (int child in graph[source])
                {
                    inDegrees[child] -= 1;

                    // If a child's in-degree becomes zero, add to sources queue
                    if (inDegrees[child] == 0)
                    {
                        sources.Enqueue(child);
                    }
                }
            }

            // 5. Check for Cycles
            // Check if there is a topological sort by seeing if the graph has a cycle
            if (sortedOrder.Count != vertices)
            {
                return new List<int>();
            }

            return sortedOrder;

        }

        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private List<T> TopologicalSort<T>(HashSet<T> nodes, HashSet<Tuple<T, T>> edges) where T : IEquatable<T>
        {
            // Empty list that will contain the sorted elements
            var L = new List<T>();

            // Set of all nodes with no incoming edges
            var S = new HashSet<T>(nodes.Where(n => edges.All(e => e.Item2.Equals(n) == false)));

            // while S is non-empty do
            while (S.Any())
            {

                //  remove a node n from S
                var n = S.First();
                S.Remove(n);

                // add n to tail of L
                L.Add(n);

                // for each node m with an edge e from n to m do
                foreach (var e in edges.Where(e => e.Item1.Equals(n)).ToList())
                {
                    var m = e.Item2;

                    // remove edge e from the graph
                    edges.Remove(e);

                    // if m has no other incoming edges then
                    if (edges.All(me => me.Item2.Equals(m) == false))
                    {
                        // insert m into S
                        S.Add(m);
                    }
                }
            }

            // if graph has edges then
            if (edges.Any())
            {
                // return error (graph has at least one cycle)
                return null;
            }
            else
            {
                // return L (a topologically sorted order)
                return L;
            }
        }

        private void dtZoneReationship()
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
