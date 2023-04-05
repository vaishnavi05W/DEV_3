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
using yWorks.Graph;
using SpaceLayout.Object;
using yWorks.Controls;
using yWorks.Geometry;
using yWorks.Graph.Styles;
using yWorks.Graph.LabelModels;
using yWorks.Controls.Input;
using System.Drawing.Design;





namespace SpaceLayout.Forms.ZoneForms
{
    public partial class ZoneSelectionControl : UserControl
    {
        static string DataSourceInputData = StaticCache.DataSourceBasicInfo;
        private DataTable dtSource;
        private IGraph graph;
        private object color;

        public ZoneSelectionControl()
        {
            InitializeComponent();
            this.Load += IS_Load;
        }


        private void IS_Load(object sender, EventArgs e)
        {
            BindGrid();
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
            dtSource.Columns.Add("Relation");
            dtSource.Columns.Add("Category");
            dtSource.Columns.Add("Color");
            dtSource.Columns.Add("Area");
            dtSource.Columns.Add("Width");
            dtSource.Columns.Add("Length");
            dtSource.Columns.Add("Height");
            dtSource.Columns.Add("Floor");
            dtSource.Columns.Add("Ratio");
            dtSource.Columns.Add("Type");

            List<InputData_ModuleEntity> result;
            List<string> temp;
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
                    ds.Tables["Input Data_Module"].Columns.Remove("Column14");
                    ds.Tables["Input Data_Module"].Columns.Remove("Column15");

                    foreach (DataRow dr in ds.Tables["Input Data_Module"].Rows)
                    {
                        dtSource.Rows.Add(dr.ItemArray);
                    }
                }
                Zone zone = new Zone();
                zone.LoadZoneFromDataTable(dtSource);
                List<Zone> zones = zone.GetZones();

                GraphControl graphControl = new GraphControl();
                graph = graphControl.Graph;

                foreach (Zone z in zones)
                {
                    Zone node = (Zone)graph.CreateNode();
                    node.Tag = z;

                    node.Layout = new RectD(0, 0, z.Width, z.Height);
                    node.style = new ShapeNodeStyle
                    {

                        Brush = new SolidBrush(Color.FromName(z.Color)),
                        Pen = new Pen(Color.FromName(z.Color)),


                    };
                 
                }
                this.Controls.Add(graphControl);
                graphControl.Dock = DockStyle.Fill;
            }



            //Bind datatable to Gridview
            if (dtSource.Rows.Count > 0)
            {
                dataGridView1.DataSource = dtSource;
            }
        }



        public DataTable GetDataSource()
        {
            return dtSource;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }


    }
}

