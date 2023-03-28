using System;
using System.Windows.Forms;
using VectorDraw.Professional.Constants;
using VectorDraw.Geometry;
using VectorDraw.Actions;
using VectorDraw.Advanced;
using vdControls;
using VectorDraw.Professional.vdCollections;
using VectorDraw.Professional.vdFigures;
using VectorDraw.Professional.vdObjects;
using VectorDraw.Professional.vdPrimaries;
using VectorDraw.Professional.Control;
using VectorDraw.Render;

namespace SpaceLayout
{
    public partial class SpaceSyntax : Form
    {
        public SpaceSyntax()
        {
            InitializeComponent();
            // this.vectorDrawBaseControl1.BaseControl.AfterAddItem += BaseControl_AfterAddItem;
            // this.vectorDrawBaseControl1.BaseControl.AddItem += BaseControl_AddItem;
            this.vdFramedControl1.BaseControl.AfterAddItem += BaseControl_AfterAddItem;
            this.vdFramedControl1.BaseControl.AddItem += BaseControl_AddItem;
        }

        private void BaseControl_AddItem(object obj, ref bool Cancel)
        {
            throw new NotImplementedException();
        }

        private void BaseControl_AfterAddItem(object obj)
        {
            throw new NotImplementedException();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Next_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Previous_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }
        static string docpath;

        private void button1_Click(object sender, EventArgs e)
        {

           



        }

       

        private void SpaceSyntax_Load(object sender, EventArgs e)
        {

        }
        private void OpenLayersDialog()
        {
            VectorDraw.Professional.Dialogs.LayersDialog.Show
           (vdFramedControl1.BaseControl.ActiveDocument);

        }

        private void vdFramedControl1_Load_1(object sender, EventArgs e)
        {
            vdFramedControl1.BaseControl.ActiveDocument.OnAfterOpenDocument += new VectorDraw.Professional.vdObjects.vdDocument.AfterOpenDocument(ActiveDocument_OnAfterOpenDocument);
            vdFramedControl1.BaseControl.ActiveDocument.Model.ZoomExtents();
            vdFramedControl1.BaseControl.ActiveDocument.Redraw(true);
        }
        
        private void ActiveDocument_OnAfterOpenDocument(object sender)
        {
           // // Show the activelayouts' entities count and hide the UCS axis icon.
           // MessageBox.Show("In this drawing (activelayout) are : " +
           //vdFramedControl1.BaseControl.ActiveDocument.ActiveLayOut.Entities.Count + " entities \r\nUCS Axis will be 
           //hidden.");


           // vdFramedControl1.BaseControl.ActiveDocument.ActiveLayOut.ShowUCSAxis = false;

        }
        

        private void button2_Click(object sender, EventArgs e)
        {
            string fname;
            InitializeComponent();
            object ret = vdFramedControl1.BaseControl.ActiveDocument.GetOpenFileNameDlg(0, "", 0);
            if (ret == null) return;

            docpath = ret as string;
            fname = (string)ret;

            bool success = vdFramedControl1.BaseControl.ActiveDocument.Open(fname);
            if (!success) vdFramedControl1.BaseControl.ActiveDocument.Redraw(true); 

        }

        
    }



}
