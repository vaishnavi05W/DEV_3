using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceLayout.Forms.GenerativeForms
{
    public partial class GenerativeInfomationForms : UserControl
    {
        public GenerativeInfomationForms()
        {
            InitializeComponent();
            this.Load += IS_Load;
        }

        private void IS_Load(object sender, EventArgs args)
        {
            for(int i = 1; i < 2; i++)
            {
                Button btn = new Button();
                btn.Text = "Alt"+ i.ToString();
                //btn.BackColor = Color.White;
                btn.ForeColor = Color.Navy;
                this.tableLayoutPanel1.Controls.Add(btn, 0, 0);
            }
            
        }

        private void dgvZoneRelationship_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
