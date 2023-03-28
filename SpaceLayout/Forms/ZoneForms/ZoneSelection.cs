using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceLayout.Forms
{
    public partial class ZoneCreation : UserControl
    {
       // static string DataSourceInputData =Entity.StaticCache.DataSourceInputData;

        public ZoneCreation()
        {
            InitializeComponent();
            this.Load += IS_Load;

        }
        //DataTable dtSource;
        private void IS_Load(object sender, EventArgs e)
        {
            BindGrid();
        }
        private void BindGrid()
        {
            //if (!File.Exists(DatasourceInputData))
            {
                //MessageBox.Show("Please make and configure a setting to initialize dbsource file having path " + DataSourceInputData + "." + Environment.NewLine + "Source File have been put at Project's Datasource Folder.");
                return;
            }
            //dtSource = new DataTable();
            //dtSource.Columns.Add("Col1");
            //dtSource.Columns.Add("Col2");
            //dtSource.Columns.Add("Col3");
            //dtSource.Columns.Add("Col4");
            //dtSource.Columns.Add("Col5");
            //dtSource.Columns.Add("Col6");
            //dtSource.Columns.Add("Col7");
            //dtSource.Columns.Add("Col8");



        }

        private void Zonecreation_Load(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
    
}
