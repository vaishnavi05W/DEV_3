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
    public partial class OutputControl1 : UserControl
    {
        public OutputControl1(DataTable dtZoneRelationshipControl)
        {
            InitializeComponent();
            this.Load += IS_Load;
        }

        private void IS_Load(object sender, EventArgs e)
        {

        }
    }
}
