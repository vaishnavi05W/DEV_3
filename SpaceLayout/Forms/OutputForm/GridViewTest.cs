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

            //dataTable.Columns.Add("Column1", typeof(Button));
            //dataTable.Columns.Add("Column2", typeof(Button));
            //dataTable.Columns.Add("Column3", typeof(Button));
            int buttonCount = 16; // Number of buttons to generate
            int buttonsPerRow = 3;
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
                            Value = "Set Text"
                        };
                    }
                }

                dataGridView1.Rows.Add(dataGridViewRow);
            }
            //    for (int col = 1; col <= buttonsPerRow; col++)
            //{
            //    DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn
            //    {
            //        Name = "Column" + col,
            //        HeaderText = "Column" + col,

            //    };

            //    dataGridView1.Columns.Add(column);
            //}
            
            //for (int row = 0; row < rowCount; row++)
            //{
            //    DataGridViewRow dataGridViewRow = new DataGridViewRow();
            //    dataGridViewRow.CreateCells(dataGridView1);

            //    for (int col = 0; col < buttonsPerRow; col++)
            //    {
            //        int buttonIndex = (row * buttonsPerRow) + col;
            //        if (buttonIndex < buttonCount)
            //        {
            //            dataGridViewRow.Cells[col].Value = "Set Text";
            //        }
            //    }

            //    dataGridView1.Rows.Add(dataGridViewRow);
            //}

            //for (int i = 1; i <= buttonsPerRow; i++)
            //{
            //    dataTable.Columns.Add("Column" + i, typeof(Button));
            //}

            //for (int row = 0; row < rowCount; row++)
            //{
            //    DataRow dataRow = dataTable.NewRow();
            //    for (int col = 0; col < buttonsPerRow; col++)
            //    {
            //        int buttonIndex = (row * buttonsPerRow) + col;
            //        if (buttonIndex < buttonCount)
            //        {
            //            dataRow[col] = new Button();
            //        }
            //    }
            //    dataTable.Rows.Add(dataRow);
            //}
            //for (int s = 0; s < buttonCount; s++)
            //{
            //    dataTable.Rows.Add(new Button(), new Button(), new Button());
            //}
            //int s = 1;
            //do
            //{
            //    int i = 3;
            //    do
            //    {
            //        i--;
            //        s++;
            //    } while (i == 0);

            //} while (s <= 8);

            //dataTable.Rows.Add(new Button() ,new Button() , new Button() );

            //dataGridView1.DataSource = dataTable;

            //foreach (DataGridViewRow row in dataGridView1.Rows)
            //{
            //    foreach (DataGridViewCell dataGridViewCell in row.Cells)
            //    {
            //        if (dataGridViewCell.Value is Button button)
            //        {
            //            button.Text = "Set Text";
            //        }
            //    }
            //}
        }
        private void GridViewTest_Load(object sender, EventArgs e)
        {

        }
    }
}
