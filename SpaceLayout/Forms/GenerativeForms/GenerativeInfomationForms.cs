using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        }



        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
      

        private void dtZoneReationship(DataTable dt)
        {
            dtZoneRelationship = new DataTable();
            dtZoneRelationship.Columns.Add("StartNode");
            dtZoneRelationship.Columns.Add("EndNode");
            dtZoneRelationship.Columns.Add("Axis");
            dtZoneRelationship.Columns.Add("Type");

            dtZoneRelationship = dt.Copy();
            dtZoneRelationship.AcceptChanges();
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
    }
}
