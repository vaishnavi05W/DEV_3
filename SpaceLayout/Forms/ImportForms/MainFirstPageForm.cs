using Nevron.Diagram;
using Nevron.Dom;
using System;
using System.Data;
using System.Windows.Forms;
using Nevron.Diagram.WinForm;
using Nevron.GraphicsCore;
using Nevron.UI.WinForm.Controls;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using Nevron.UI.WinForm.Docking;
using System.Collections.Generic;

namespace SpaceLayout.Forms.ZoneForms
{
    public partial class MainFirstPageControl : Form
    {
        //private IGraph graph;
        public static bool ExcelFlg = false;
        public static DataTable dtZoneSelection = null;
        private MouseEventHandler nDrawingView1_MouseWheel;

        public static object CommandBarsManager { get; internal set; }
        public object m_ZoomRect { get; private set; }

        public MainFirstPageControl()
        {
            InitializeComponent();
            this.Load += IS_Load;
            //nDrawingView1.MouseWheel += NDrawingView1_MouseWheel;
        }

        private void IS_Load(object sender, EventArgs e)
        {
            btnPrevious.Visible = false;
            btnNext.Visible = false;

            toolStripButton2.Enabled = false;
            toolStripButton3.Enabled = false;
            toolStripButton4.Enabled = false;

            //this.nDiagramCommandBarsManager1.AllowCustomize = false;
            //foreach (NDockingToolbar tb in this.nDiagramCommandBarsManager1.Toolbars)
            //{
            //    tb.dele
            //    tb.AllowHide = false;
            //    tb.HasPendantCommand = false;
            //}

            //Array list = this.nDiagramCommandBarsManager1.CommandManager.GetAllCommands().ToArray();
            //Nevron.Diagram.WinForm.Commands.NDiagramCommand cmd = list[0]
            //this.nDiagramCommandBarsManager1.Commander.Commands.Remove();
            // begin view init
            nDrawingView1.BeginInit();

            // display the document in the view
            nDrawingView1.Document = nDrawingDocument1;

            // do not show ports
            nDrawingView1.GlobalVisibility.ShowPorts = true;

            // hide the grid
            nDrawingView1.Grid.Visible = false;

            nDrawingView1.HorizontalRuler.Visible = false;
            nDrawingView1.VerticalRuler.Visible = false;
           
            // fit the document in the viewport 
            //nDrawingView1.ViewLayout = ViewLayout.StretchToWidth;
            
            // apply padding to the document bounds
            nDrawingView1.DocumentPadding = new Nevron.Diagram.NMargins(10);

            // init document
            nDrawingDocument1.BeginInit();


            // end nDrawingDocument1 init
            nDrawingDocument1.EndInit();

            //end view init
            nDrawingView1.EndInit();

            // get the NDiagramCommandBarsManager instance
        }

       
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            LoadRightPanel(2);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            LoadRightPanel(1);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            LoadRightPanel(3);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {

        }

        public void LoadRightPanel(int flg)
        {
            if (flg == 1)
            {
                if (tableLayoutPanel2.Controls.Count > 1)
                    tableLayoutPanel2.Controls.Remove(tableLayoutPanel2.GetControlFromPosition(0, 1));

                var ImportDataFrom = new ImportDataForm();
                tableLayoutPanel2.Controls.Add(ImportDataFrom, 0, 1);
                ImportDataFrom.Dock = DockStyle.Fill;
                btnNext.Visible = true;
                btnPrevious.Visible = false;
            }
            else if (flg == 2)
            {
                if (tableLayoutPanel2.Controls.Count > 1)
                    tableLayoutPanel2.Controls.Remove(tableLayoutPanel2.GetControlFromPosition(0, 1));

                var ZoneSelectionControl = new ZoneSelectionControl();
                tableLayoutPanel2.Controls.Add(ZoneSelectionControl, 0, 1);
                ZoneSelectionControl.Dock = DockStyle.Fill;
                btnNext.Visible = true;
                btnPrevious.Visible = true;
            }
            else if (flg == 3)
            {
                if (tableLayoutPanel2.Controls.Count > 1)
                    tableLayoutPanel2.Controls.Remove(tableLayoutPanel2.GetControlFromPosition(0, 1));

                var ZoneRelation = new ZoneRelationshipControl();
                tableLayoutPanel2.Controls.Add(ZoneRelation, 0, 1);
                ZoneRelation.Dock = DockStyle.Fill;
                btnNext.Visible = false;
                btnPrevious.Visible = true;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            var contrName = tableLayoutPanel2.GetControlFromPosition(0, 1).Name;
            if (contrName == "ImportDataForm" )
            {
                if(ExcelFlg == true)
                {
                    LoadRightPanel(2);
                    toolStripButton2.Enabled = true;
                    toolStripButton3.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Please click the 'Import' button to import the data source first.");
                    toolStripButton2.Enabled = false;
                    toolStripButton3.Enabled = false;
                    return;
                }
            }
            else if (contrName == "ZoneSelectionControl")
            {
                LoadRightPanel(3);
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            var contrName = tableLayoutPanel2.GetControlFromPosition(0, 1).Name;

            if (contrName == "ZoneSelectionControl")
            {
                LoadRightPanel(1);
                toolStripButton2.Enabled = false;
                toolStripButton3.Enabled = false;
                ExcelFlg = false;
            }
            else if (contrName == "ZoneRelationshipControl")
            {
                LoadRightPanel(2);
            }
        }

        private void ImportData()
        {
         
        }

        
        private void NDrawingView1_MouseWheel(object sender, MouseEventArgs e)
        {
            // Zooming logic
            float zoom = nDrawingView1.Document.ActiveLayer.MaxShowZoomFactor;

            // Check the direction of the mouse wheel
            if (e.Delta > 0)
            {
                // Zoom in
                zoom += 0.1f;
            }
            else if (e.Delta < 0)
            {
                // Zoom out
                zoom -= 0.1f;
            }

            // Set the updated zoom factor
            nDrawingView1.Document.ActiveLayer.MaxShowZoomFactor = zoom;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
