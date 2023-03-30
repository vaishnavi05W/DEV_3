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
    public partial class MainFirstPageControl : Form
    {
        public MainFirstPageControl()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (tableLayoutPanel2.Controls.Count > 1)
                tableLayoutPanel2.Controls.RemoveAt(1);

            var ZoneSelectionControl = new ZoneSelectionControl();
            tableLayoutPanel2.Controls.Add(ZoneSelectionControl);
            ZoneSelectionControl.Dock = DockStyle.Fill;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (tableLayoutPanel2.Controls.Count > 1)
                tableLayoutPanel2.Controls.RemoveAt(1);

            var ImportDataFrom = new ImportDataForm();
            tableLayoutPanel2.Controls.Add(ImportDataFrom);
            ImportDataFrom.Dock = DockStyle.Fill;
        }
    }
}
