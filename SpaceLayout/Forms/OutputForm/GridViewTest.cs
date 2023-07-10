using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceLayout.Forms.OutputForm
{
    public partial class GridViewTest : Form
    {
        public GridViewTest()
        {
            InitializeComponent();
            this.Load += IS_Load;
        }

        private void IS_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("Column1", typeof(Button));
            dataTable.Columns.Add("Column2", typeof(Button));
            dataTable.Columns.Add("Column3", typeof(Button));
            int s = 1;
            do
            {
                int i = 3;
                do
                {
                    i--;
                    s++;
                } while (i == 0);

            } while (s <= 8);
            
            dataTable.Rows.Add(new Button() ,new Button() , new Button() );
            
            dataGridView1.DataSource = dataTable;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataGridViewCell cell = row.Cells[0];//Column Index
                cell.Value = "Set Text";
            }
        }
    }
}
