using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nevron.Diagram;
using Nevron.Filters;
using Nevron.GraphicsCore;


namespace SpaceLayout.Forms.ZoneForms
{
    public partial class ContenetPlacementForm : Form
    {
        public string placementWeight = "";
        public ContenetPlacementForm(string title)
        {
            InitializeComponent();
            this.label1.Text = title.ToString();
            this.Load += IS_Load;
        }

        private void  IS_Load(object sender, EventArgs args)
        {
            
            this.ChkFit.CheckedChanged += Chk_Checked;
            this.chkNear.CheckedChanged += Chk_Checked;
            this.chkFar.CheckedChanged += Chk_Checked;
            this.btnApply.Click += btnApply_Clicked;
        }

        private void Chk_Checked(object sender, EventArgs args)
        {
            CheckBox chk = new CheckBox();
            chk = sender as CheckBox;
            
            if(chk.Checked == true && chk == ChkFit)
            {
                chkNear.Checked = false;
                chkFar.Checked = false;

                chkNear.Enabled = false;
                chkFar.Enabled = false;
            }
            else if(chk.Checked == true && chk == chkNear)
            {
                ChkFit.Checked = false;
                chkFar.Checked = false;

                ChkFit.Enabled = false;
                chkFar.Enabled = false;
            }
            else if (chk.Checked == true && chk == chkFar)
            {
                ChkFit.Checked = false;
                chkNear.Checked = false;

                ChkFit.Enabled = false;
                chkNear.Enabled = false;
            }
            else
            {
                ChkFit.Checked = false;
                chkNear.Checked = false;
                chkFar.Checked = false;

                ChkFit.Enabled = true;
                chkNear.Enabled = true;
                chkFar.Enabled = true;
            }
        }

        private void btnApply_Clicked(object sender, EventArgs args)
        {
            if(ChkFit.Checked == true)
            {
                placementWeight = "Fit";
                this.Close();
            }
            else if( chkNear.Checked == true) 
            {
                placementWeight = "Near";
                this.Close();
            }
            else if(chkFar.Checked == true)
            {
                placementWeight = "Far";
                this.Close();
            }
            else
            {
                MessageBox.Show("Please choose one of the Content Placement weight.");
            }
        }
    }
}
