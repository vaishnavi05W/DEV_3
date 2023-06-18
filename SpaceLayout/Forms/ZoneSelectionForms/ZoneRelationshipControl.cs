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

namespace SpaceLayout.Forms.ZoneForms
{
    public partial class ZoneRelationshipControl : UserControl
    {
        public DataTable dtZoneRelationSource = new DataTable();
        Form parentFrm;

        public NDrawingView Ndv;
        public NDrawingDocument Ndd;

        public ZoneRelationshipControl()
        {
            InitializeComponent();
            this.Load += IS_Load;
        }

        private void IS_Load(object sender, EventArgs e)
        {
            parentFrm = this.ParentForm;
            Ndv = parentFrm.Controls.Find("nDrawingView1", true).FirstOrDefault() as NDrawingView;
            if (Ndv != null)
            {
                Ndd = Ndv.Document;
            }

            this.dgvZoneRelationship.ReadOnly = true;
            this.dgvZoneRelationship.AllowUserToAddRows = false;
            this.dgvZoneRelationship.AllowUserToDeleteRows = false;
            BindGrid();

        }

        private void BindGrid()
        {
            dtZoneRelationSource = new DataTable();
            dtZoneRelationSource.Columns.Add("ID");
            dtZoneRelationSource.Columns.Add("StartGroup");
            dtZoneRelationSource.Columns.Add("EndGroup");
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
                               && line.FromShape.Group != line.ToShape.Group)
                        {
                            DataRow workRow = dtZoneRelationSource.NewRow();
                            string[] axistype = { };
                            if (!string.IsNullOrEmpty(line.Text.ToString()))
                            {
                                axistype = line.Text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                                workRow["ID"] = "";
                                workRow["StartGroup"] = line.FromShape.Group.Name.ToString();
                                workRow["EndGroup"] = line.ToShape.Group.Name.ToString();
                                workRow["Axis"] = line.Tag.ToString();
                                workRow["Type"] = axistype[1];

                                dtZoneRelationSource.Rows.Add(workRow);

                            }
                            else
                            {
                                workRow["ID"] = "";
                                workRow["StartGroup"] = line.FromShape.Group.Name.ToString();
                                workRow["EndGroup"] = line.ToShape.Group.Name.ToString();
                                workRow["Axis"] = line.Tag.ToString();
                                workRow["Type"] = "";

                                dtZoneRelationSource.Rows.Add(workRow);
                            }
                        }
                    }
                }
            }

        }

        private void ExportCSV(object sender, EventArgs args)
        {
            //if(dtSource.row)
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
    }
}
