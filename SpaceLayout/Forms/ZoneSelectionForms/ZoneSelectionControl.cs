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


using Nevron.Diagram.WinForm;
using Nevron.Diagram;
using Nevron.Diagram.DataStructures;

using Nevron.GraphicsCore;
using System.Drawing.Design;

using Nevron.Dom;
using Nevron.Diagram.Layout;
using SpaceLayout.Object;
using Nevron.Diagram.Batches;
using Nevron.Diagram.ThinWeb;
using Nevron.Diagram.Extensions;
using Nevron.Serialization;

namespace SpaceLayout.Forms.ZoneForms
{
    public partial class ZoneSelectionControl : UserControl
    {
        static string DataSourceInputData = StaticCache.DataSourceBasicInfo;
        private DataTable dtSource;

        // public GraphControl chartControl { get; set; }
        public NDrawingView Ndv;
        public NDrawingDocument Ndd;
        public NGraph graph;

        public NRectangleShape SourceNode = null;
        public NRectangleShape TargetNode = null;
        private NPersistencyManager persistencyManager;

       

        public ZoneSelectionControl()
        {
            InitializeComponent();
            this.persistencyManager = new NPersistencyManager();
           
           // drawing = (NDrawingDocument)persistencyManager.LoadDocumentFromFile("c:\\temp\\drawing1.ndx");
            
            this.Load += IS_Load;
            
         
          

            // Add the diagram view to the form
            this.Controls.Add(Ndv);

            //chartControl.ItemClick += chartControl_MouseClick;

        }

        private void ChartControl_NodeCreated(NNodeEventArgs args)
        {
            Console.WriteLine(args.Node.ToString());
        }

        private void IS_Load(object sender, EventArgs e)
        {
            BindGrid();

            Form f = this.ParentForm;
            Ndv = f.Controls.Find("nDrawingView1", true).FirstOrDefault() as NDrawingView;
            if (Ndv != null)
            {
                Ndd = Ndv.Document;
            }
           
            //chartControl.Controller.Selection.Changed += chartControl_SelectionChanged;
            //chartControl.Controller.DoubleClick += chartControl_DoubleClick;
            //chartControl.Document.NodeCreated += ChartControl_NodeCreated;
            //chartControl.Document.UndoRedoService.Pause();
            //chartControl.Document.UndoRedoService.ChangesRegistry.Clear();
            //chartControl.Document.UndoRedoService.Resume();
        }

        //private void chartcontrol_MouseDoubleClick(object sender, MouseEventArgs e)
        //{
        //    foreach (var node in chartControl.Selection.SelectedNodes) { 

        //    Console.WriteLine("-----"+node.ToString());


        //    }
        //}

        //private void graphcontrol_SelectionChanged(object sender, EventArgs e)
        //{
        //    // Get the selected nodes
        //    Console.WriteLine(sender.ToString());
        //    Console.WriteLine(e.ToString());
        // //   GraphSelection graphControl = (GraphSelection)sender;
        //  //  IEnumerable<INode> selectedNodes = graphControl.Selection.SelectedNodes;

        //}
        //public class Selection
        //{
        //    public HashSet<INode> Nodes { get; } = new HashSet<INode>();
        //    public HashSet<IEdge> Edges { get; } = new HashSet<IEdge>();
        //}

        //private void Graph_EdgeCreated(object sender, yWorks.Utils.ItemEventArgs<IEdge> e)
        //{
        //   // Connector zone linking


        //}

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

            //List<InputData_ModuleEntity> result;
            //List<string> temp;
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
        private NDrawingDocument drawing;

        //public NDrawingDocument Drawing { get => drawing; set => drawing = value; }


        //private void chartControl_MouseClick(object sender, Nevron.Diagram.WinForm.NMouseEventArgs e)
        //{
        //NRectangleShape clickedNode = Ndv.HitTest(e.Location);

        //    if (clickedNode != null && clickedNode is NRectangleShape shapeNode)
        //    {
        //        if (SourceNode == null)
        //        {
        //            SourceNode = shapeNode;
        //        }
        //        else if (TargetNode == null)
        //        {
        //            TargetNode = shapeNode;

        //            NLineShape edge = new NLineShape();
        //            //chartControl.Document..AddChild(edge);
        //            edge.FromShape = SourceNode;
        //            edge.ToShape = TargetNode;

        //            SourceNode = null;
        //            TargetNode = null;
        //        }
        //    }
        //}

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

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1)
            {
                string columnName = this.dataGridView1.Columns[e.ColumnIndex].Name;
                if ((columnName == "Column1") && (!string.IsNullOrWhiteSpace(this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())))
                {
                    dataGridView1.ReadOnly = true;
                    try 
                    {
                        if (!rowNodeCreated.ContainsKey(e.RowIndex) || !rowNodeCreated[e.RowIndex])
                        {

                            Ndv.BeginInit();
                            Ndd.BeginInit();



                          
                            NLayer activeLayer = Ndd.ActiveLayer;
                            NGraph graph = new NGraph();
                            var rand = new Random();
                            Zone zone = new Zone();
                            float width = (float)Math.Sqrt(Convert.ToDouble(this.dataGridView1.Rows[e.RowIndex].Cells["Column6"].Value) * 2);
                            float height = (float)(Convert.ToDouble(dataGridView1.CurrentRow.Cells["Column6"].Value) / width);



                         



                            NRectangleF rect = new NRectangleF(0, 0, width, height);
                            NRectangleShape node = new NRectangleShape(rect);
                            node.Tag = zone;
                            node.Id = zone.ID;
                            node.Style = new NStyle
                            {
                                FillStyle = new NColorFillStyle(Color.FromName(this.dataGridView1.Rows[e.RowIndex].Cells["Column13"].Value.ToString())),
                            };

                            string NodeLabelIn = this.dataGridView1.Rows[e.RowIndex].Cells["Column1"].Value.ToString() + System.Environment.NewLine + this.dataGridView1.Rows[e.RowIndex].Cells["Column6"].Value.ToString() + " " + "m\u00b2";
                            //graphcontrol.Graph.AddLabel(node, NodeLabelIn, InteriorLabelModel.NorthWest, defaultLableStyle, new SizeD(width, height));
                            string NodeLabelOut = this.dataGridView1.Rows[e.RowIndex].Cells["Column2"].Value.ToString();
                            //graphcontrol.Graph.AddLabel(node, NodeLabelOut, ExteriorLabelModel.South, defaultLableStyle);
                            node.Text = NodeLabelIn;
                            


                            //graph.Nodes.Add(node);



                            Random rnd = new Random();
                            
                                int x = rnd.Next((int)Ndv.ViewportOrigin.X, (int)Ndv.ViewportOrigin.X + (int)Ndv.ViewportSize.Width - (int)rect.Width);
                                int y = rnd.Next((int)Ndv.ViewportOrigin.Y, (int)Ndv.ViewportOrigin.Y + (int)Ndv.ViewportSize.Height - (int)rect.Height);
                                node.Location = new NPointF(x, y);



                               
                           
                                activeLayer.AddChild(node);
                             

                            //double maxX = graphcontrol.ClientSize.Width - width;
                            //double maxY = graphcontrol.ClientSize.Height - height;
                            //double x = rand.NextDouble() * maxX;
                            //double y = rand.NextDouble() * maxY;
                            //graphcontrol.Graph.SetNodeCenter(node, new PointD(x, y));

                            //graphcontrol.FitGraphBounds();
                            rowNodeCreated[e.RowIndex] = true; //make it true to avoid duplicate node
                            Ndv.EndInit();
                            Ndd.EndInit();
                        }
                        else
                        {
                            MessageBox.Show("Zone already created for this row.");
                        }
                       
                        {

                            //var graphEditorInputMode = new GraphEditorInputMode();
                            //graphEditorInputMode.AllowCreateNode = false; // restrict node creation by clicking in UI
                            //graphEditorInputMode.CreateEdgeInputMode.Enabled = true;
                            //graphcontrol.InputMode = graphEditorInputMode;
                            //graphcontrol.Graph.EdgeDefaults.Style = edgeStyle;
                            //graphcontrol.FitGraphBounds();
                           // Ndv.MouseClick += OnItemClicked;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }
        //savebutton
        private void button3_Click(object sender, EventArgs e)
        {
            //Ndd.EndUpdate();
            // create a rectangle
            NRectangleShape rect = new NRectangleShape(10, 10, 20, 20);

            // create a new persistent document NPersistentDocument
            NPersistentDocument document = new NPersistentDocument("My document");

            // create a new section which will store the rect
            document.Sections.Add(new NPersistentSection("Rectangle", rect));

            // set the document to the manager
            persistencyManager.PersistentDocument = document;

            // save the document to a file
            persistencyManager.SaveToFile("c:\\temp\\mysavefile.ndx", PersistencyFormat.XML, null);

         
        
        }

        private void button4_Click(object sender, EventArgs e)
        {
           Ndd= persistencyManager.LoadDrawingFromFile( "c:\\temp\\drawing1.ndx");
           Ndv.Document = drawing;
            //MessageBox.Show("Import");
        }

        //private void OnItemClicked(object sender, MouseEventArgs e)
        //{
        //    if(e.Ite)
        //}
}
    }

