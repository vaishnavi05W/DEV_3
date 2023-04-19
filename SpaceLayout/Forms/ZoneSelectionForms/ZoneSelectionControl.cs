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
        public GraphControl graphcontrol;
        public INode SourceNode = null;
        public INode TargetNode = null;

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
           // dtSource.Columns.Add("Department");
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
            }

            //Bind datatable to Gridview
            if (dtSource.Rows.Count > 0)
            {
                dataGridView1.DataSource = dtSource;

                // Form MainFirstPage = new MainFirstPageControl(dtSource);
                //CreateNodes(dtSource);
            }
        }

        public DataTable GetDataSource()
        {
            return dtSource;
        }
        private Dictionary<int, bool> rowNodeCreated = new Dictionary<int, bool>();

        public void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.ReadOnly = true;
            try
            {
                Form f = this.ParentForm;
                graphcontrol = f.Controls.Find("graphControl1", true).FirstOrDefault() as GraphControl;
                if (!rowNodeCreated.ContainsKey(e.RowIndex) || !rowNodeCreated[e.RowIndex])
                {
                   

                    var rand = new Random();
                    Zone zone = new Zone();
                    double width = Math.Sqrt(Convert.ToDouble(this.dataGridView1.Rows[e.RowIndex].Cells["Column6"].Value) * 2);
                    double height = (Convert.ToDouble(dataGridView1.CurrentRow.Cells["Column6"].Value) / width);
                    var node = graphcontrol.Graph.CreateNode();
                    graphcontrol.Graph.SetNodeLayout(node, new RectD(0, 0, width, height));


                    //var style = new ShapeNodeStyle
                    //{
                    //    Brush =  new SolidColorBrush // Blue with alpha 255

                    //    Pen = new Pen(Color.FromName(this.dataGridView1.Rows[e.RowIndex].Cells["Column13"].Value.ToString())),
                    //};

                    var NodeStyle = new ShapeNodeStyle
                    {
                        Brush = new SolidBrush(Color.FromName(this.dataGridView1.Rows[e.RowIndex].Cells["Column13"].Value.ToString())),
                        Pen = new Pen(Color.FromName(this.dataGridView1.Rows[e.RowIndex].Cells["Column13"].Value.ToString())),
                    };
                    graphcontrol.Graph.SetStyle(node, NodeStyle);

                    var defaultLableStyle = new DefaultLabelStyle
                    {
                        TextBrush = Brushes.LightGray,
                    };
                    string NodeLabel = this.dataGridView1.Rows[e.RowIndex].Cells["Column1"].Value.ToString() + System.Environment.NewLine + this.dataGridView1.Rows[e.RowIndex].Cells["Column6"].Value.ToString() + " " + "m\u00b2";
                    graphcontrol.Graph.AddLabel(node, NodeLabel, InteriorLabelModel.NorthWest, defaultLableStyle, new SizeD(width, height));

                    double maxX = graphcontrol.ClientSize.Width - width;
                    double maxY = graphcontrol.ClientSize.Height - height;
                    double x = rand.NextDouble() * maxX;
                    double y = rand.NextDouble() * maxY;
                    graphcontrol.Graph.SetNodeCenter(node, new PointD(x, y));

                    graphcontrol.FitGraphBounds();
                    rowNodeCreated[e.RowIndex] = true; //make it true to avoid duplicate node
                }

                else
                {
                    MessageBox.Show("Zone already created for this row.");

                   



                }
                {

                    var graphEditorInputMode = new GraphEditorInputMode();
                    graphEditorInputMode.AllowCreateNode = false; // restrict node creation by clicking in UI
                    graphEditorInputMode.CreateEdgeInputMode.Enabled = true;
                    graphcontrol.InputMode = graphEditorInputMode;

                   // graphEditorInputMode.ClickableItems = GraphItemTypes.Node;
                    //graphEditorInputMode.MarqueeSelectableItems = GraphItemTypes.Node;
                   

                    //var grapEditorInputMode = new GraphEditorInputMode();
                    //graphcontrol.InputMode = grapEditorInputMode;

                    //grapEditorInputMode.ClickableItems = GraphItemTypes.Node;

                    //graphEditorInputMode.MarqueeSelectableItems = GraphItemTypes.Node;

                

                    var edgeStyle = new PolylineEdgeStyle
                    {
                        Pen = new Pen(Brushes.Black, 2),
                        TargetArrow = new Arrow { Brush = Brushes.Black, Type = ArrowType.Default }
                    };
                    graphcontrol.Graph.EdgeDefaults.Style = edgeStyle;
                    graphcontrol.FitGraphBounds();
                   graphEditorInputMode.ItemClicked += OnItemClicked;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
       


        private void OnItemClicked(object sender, ItemClickedEventArgs<IModelItem> e)
        {
            {
                if (e.Item is INode node)
                {
                  
                    if (SourceNode == null)
                    {
                        SourceNode = node;

                    }
                    else if (TargetNode == null)
                    {
                        TargetNode = node;
                        var edge = graphcontrol.Graph.CreateEdge(SourceNode, TargetNode);
                        graphcontrol.Selection.Clear();
                        SourceNode = null;
                        TargetNode = null;
                    }
                }
            };
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid")
            {
                object value = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (!((DataGridViewComboBoxColumn)dataGridView1.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    ((DataGridViewComboBoxColumn)dataGridView1.Columns[e.ColumnIndex]).Items.Add(value);
                    e.ThrowException = false;
                }
            }
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }

}

