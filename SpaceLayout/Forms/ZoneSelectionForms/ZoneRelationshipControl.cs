using Nevron.Diagram;
using Nevron.Diagram.Filters;
using Nevron.Diagram.WinForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceLayout.Forms.ZoneForms
{
    public partial class ZoneRelationshipControl : UserControl
    {
        DataTable dtSource;
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
            dtSource = new DataTable();
            dtSource.Columns.Add("ID");
            dtSource.Columns.Add("StartNode");
            dtSource.Columns.Add("EndNode");
            dtSource.Columns.Add("Axis");
            dtSource.Columns.Add("Type");

            Get_GroupConnectorData();

            if(dtSource.Rows.Count > 0)
                dgvZoneRelationship.DataSource = dtSource;
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
                            DataRow workRow = dtSource.NewRow();
                            string[] axistype = { };
                            if (!string.IsNullOrEmpty(line.Text.ToString()))
                            {
                                axistype = line.Text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                                workRow["ID"] = "";
                                workRow["StartNode"] = line.FromShape.Group.Name.ToString();
                                workRow["EndNode"] = line.ToShape.Group.Name.ToString();
                                workRow["Axis"] = line.Tag.ToString();
                                workRow["Type"] = axistype[1];

                                dtSource.Rows.Add(workRow);
                                //congroup.Add(new Connector_Group(GetGroupDataFromDataSource((NGroup)line.FromShape.Group)
                                //    , GetGroupDataFromDataSource((NGroup)line.ToShape.Group)
                                //    , line.Tag.ToString()
                                //    , axistype[1].ToString()
                                //    , line.Length));

                            }
                            else
                            {
                                workRow["ID"] = "";
                                workRow["StartGroup"] = line.FromShape.Group.Name.ToString();
                                workRow["EndGroup"] = line.ToShape.Group.Name.ToString();
                                workRow["Axis"] = line.Tag.ToString();
                                workRow["Type"] ="";

                                dtSource.Rows.Add(workRow);
                                //congroup.Add(new Connector_Group(GetGroupDataFromDataSource((NGroup)line.FromShape.Group)
                                //    , GetGroupDataFromDataSource((NGroup)line.ToShape.Group)
                                //    , line.Tag.ToString()
                                //    , string.Empty
                                //    , line.Length));
                            }
                        }
                    }
                }
            }
            
        }
    }
}
