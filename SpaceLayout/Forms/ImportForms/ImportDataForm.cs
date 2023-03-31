using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vdControls;
using VectorDraw.Geometry;
using VectorDraw.Professional.Constants;
using VectorDraw.Professional.vdCollections;
using VectorDraw.Professional.vdFigures;
using VectorDraw.Professional.vdObjects;
using VectorDraw.Professional.vdPrimaries;
using VectorDraw.Actions;

namespace SpaceLayout.Forms.ZoneForms
{
    public partial class ImportDataForm : UserControl
    {
        static bool ExcelFlg = false;
        public ImportDataForm()
        {
            InitializeComponent();
        }

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            {
                

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ExcelFlg = true;
        }

        private void Next_Click(object sender, EventArgs e)
        {
            if(ExcelFlg)
            {
                this.Parent.Controls.Remove(this);

                //ToolStripButton btn = this.ParentForm.Controls.Find("toolStripButton2", true).FirstOrDefault() as ToolStripButton;
                //this.Parent.Controls.Add(ZoneSelectionControl);
                //var tbtn = this.tableLayoutPanel2.Controls.Find("toolStrip1", true);

                var ZoneSelectionControl = new ZoneSelectionControl();
                this.Parent.Controls.Add(ZoneSelectionControl);
                ZoneSelectionControl.Dock = DockStyle.Fill;
            }
        }
    }
}
