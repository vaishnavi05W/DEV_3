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

namespace SpaceLayout.Forms.GenerativeForms
{
    public partial class GenerativeInfomationForms : UserControl
    {
        public DataTable dtZoneRelationship;
        public DataTable dtSourceMain;
        List<int> node = new List<int>();
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
            
            node = dtSourceMain.AsEnumerable()
                .Select(s => Convert.ToInt32(s.Field<string>("ID")))
                .Distinct()
                .ToList();
            Graph g = new Graph(node);
            //HashSet<Tuple<int, int>> connection = new HashSet<Tuple<int, int>>();
            foreach (DataRow dr in dtZoneRelationship.Rows)
            {
                g.AddEdge(Convert.ToInt32(dr[0]), Convert.ToInt32(dr[1]));
                //connection.Add(Tuple.Create(Convert.ToInt32(dr[0]), Convert.ToInt32(dr[1])));
            }
            seq = g.DFS();
            //var ret = GetAllTopologicalSorts(new HashSet<int>(node), connection);
            Bind_LowerTable();
            //DgvAlternative(seq.Count);
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

        private void DrawDiagram()
        {

        }
      
        private void Bind_LowerTable()
        {
            List<string> horizontal = new List<string>();
            List<string> vertical = new List<string>();
            DataTable dtlower = new DataTable();
            dtlower.Columns.Add("Floor");
            dtlower.Columns.Add("Horizontal");
            dtlower.Columns.Add("Vertical");

            var floor = dtSourceMain.AsEnumerable().Select(s => s.Field<string>("floor")).Distinct().ToList();
            if (floor.Any())
            {
                foreach(var f in floor)
                {
                    DataRow dr = dtlower.NewRow();
                    dr["Floor"] = f + "f";
                    dr["Horizontal"] = "";
                    dr["Vertical"] = "";
                    dtlower.Rows.Add(dr);
                }
            }
           // string a = 1 + "-" + 2;
            //foreach (DataRow dr in dtZoneRelationship.Rows)
            //{
            //    var l1 = dtSourceMain.AsEnumerable().Where(x => x. = dr[0].ToString()).Select(s => s.Field<string>("floor"));

            //}
            dtlower.AcceptChanges();
            dgvZoneRelationship.DataSource = dtlower;
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
