using Nevron.Diagram;
using Nevron.Diagram.Filters;
using Nevron.Diagram.WinForm;
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
using SpaceLayout.Forms.GenerativeForms;

namespace SpaceLayout.Forms.ZoneForms
{
    public partial class ZoneRelationshipControl : UserControl
    {
        public DataTable dtZoneRelationSource = new DataTable();
        public DataTable dtSource = new DataTable();
        Form parentFrm = null;

        public NDrawingView Ndv = null;
        public NDrawingDocument Ndd = null;
        public TabPage tabMain = null;
        public TabPage tabGenerative = null;
        public TabControl tabControl1 = null;
        public TableLayoutPanel rightPanel = null;
        public ZoneRelationshipControl(DataTable dtDataSource)
        {
            InitializeComponent();
            this.Load += IS_Load;
            dtSource = dtDataSource;
        }

        private void IS_Load(object sender, EventArgs e)
        {
            parentFrm = this.ParentForm;
            Ndv = parentFrm.Controls.Find("nDrawingView1", true).FirstOrDefault() as NDrawingView;
            if (Ndv != null)
            {
                Ndd = Ndv.Document;
            }

            tabMain = parentFrm.Controls.Find("tabMain", true).FirstOrDefault() as TabPage;
            tabGenerative = parentFrm.Controls.Find("tabGenerative", true).FirstOrDefault() as TabPage;
            tabControl1 = parentFrm.Controls.Find("tabControl1", true).FirstOrDefault() as TabControl;
            rightPanel = parentFrm.Controls.Find("tableLayoutPanel2", true).FirstOrDefault() as TableLayoutPanel;
            this.dgvZoneRelationship.ReadOnly = true;
            this.dgvZoneRelationship.AllowUserToAddRows = false;
            this.dgvZoneRelationship.AllowUserToDeleteRows = false;
            BindGrid();
            this.btnGenerative.Click += btnGenerative_Clicked;
        }

        private void BindGrid()
        {
            dtZoneRelationSource = new DataTable();
            dtZoneRelationSource.Columns.Add("StartNode");
            dtZoneRelationSource.Columns.Add("EndNode");
            dtZoneRelationSource.Columns.Add("Axis");
            dtZoneRelationSource.Columns.Add("Type");

            Get_GroupConnectorData();

            if (dtZoneRelationSource.Rows.Count > 0)
                dgvZoneRelationship.DataSource = dtZoneRelationSource;
        }

        private void Get_GroupConnectorData()
        {
            if (Ndd != null || Ndd.ActiveLayer != null
                    || Ndd.ActiveLayer.Descendants(NFilters.Shape1D, -1).Count != 0 || Ndd.ActiveLayer.Descendants(NFilters.Shape2D, -1).Count != 0)
            {
                foreach (NLineShape edge in Ndv.Document.Descendants(NFilters.Shape1D, -1)) ////Detecting Line Shapes with 1D 
                {
                    // Check if the shape is a line and has a source and target shape
                    if (edge is NLineShape line && line.FromShape != null && line.ToShape != null) //For Connectors
                    {
                        if (line.FromShape.Group is NGroup && line.ToShape.Group is NGroup
                               && line.FromShape.Group != line.ToShape.Group && line.FromShape.Name == "RectangleShape"
                               && line.ToShape.Name == "RectangleShape")
                        {
                            DataRow workRow = dtZoneRelationSource.NewRow();
                            string[] axistype = { };
                            if (!string.IsNullOrEmpty(line.Text.ToString()))
                            {
                                axistype = line.Text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                                workRow["StartNode"] = line.FromShape.Group.Name.ToString();
                                workRow["EndNode"] = line.ToShape.Group.Name.ToString();
                                workRow["Axis"] = axistype[0] is null ? string.Empty : axistype[0];
                                workRow["Type"] = axistype[1] is null ? string.Empty : axistype[1];

                                dtZoneRelationSource.Rows.Add(workRow);

                            }
                            else
                            {
                                workRow["StartNode"] = line.FromShape.Group.Name.ToString();
                                workRow["EndNode"] = line.ToShape.Group.Name.ToString();
                                workRow["Axis"] = axistype[0] is null? string.Empty : axistype[0];
                                workRow["Type"] = "";

                                dtZoneRelationSource.Rows.Add(workRow);
                            }
                        }
                        else if (line.StartPlug.Shape.FromShape is NRectangleShape && line.EndPlug.Shape.ToShape is NRectangleShape)
                        {
                            DataRow workRow = dtZoneRelationSource.NewRow();
                            string[] axistype = { };
                            if (!string.IsNullOrEmpty(line.Text.ToString()))
                            {
                                axistype = line.Text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                                workRow["StartNode"] = line.FromShape.Text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)[0].ToString();
                                workRow["EndNode"] = line.ToShape.Text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)[0].ToString();
                                workRow["Axis"] = axistype[0] is null ? string.Empty : axistype[0];
                                workRow["Type"] = axistype[1] is null ? string.Empty : axistype[1];

                                dtZoneRelationSource.Rows.Add(workRow);

                            }
                            else
                            {
                                workRow["StartNode"] = line.FromShape.Text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)[0].ToString();
                                workRow["EndNode"] = line.ToShape.Text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)[0].ToString();
                                workRow["Axis"] = axistype[0] is null ? string.Empty : axistype[0];
                                workRow["Type"] = "";

                                dtZoneRelationSource.Rows.Add(workRow);
                            }
                        }
                    }
                }
            }

        }

        private void ToCSV(DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
            //headers    
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);

            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }

        private void btnGenerative_Clicked(object sender, EventArgs args)
        {
           // if(dgvZoneRelationship.Rows.Count > 0)
            {
                tabControl1.SelectedTab = tabGenerative;
                rightPanel.Controls.Remove(rightPanel.GetControlFromPosition(0, 1));
                var GenerativeInfomationForms = new GenerativeInfomationForms(dtSource, dtZoneRelationSource);
                rightPanel.Controls.Add(GenerativeInfomationForms, 0, 1);
                GenerativeInfomationForms.Dock = DockStyle.Fill;
            }

            
        }
    }
}
