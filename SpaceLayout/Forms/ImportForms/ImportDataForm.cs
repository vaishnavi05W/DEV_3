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
        public bool ExcelFlg = false;
        public ImportDataForm()
        {
            InitializeComponent();
            this.Load += IS_Load;
        }

        private void IS_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            {


            }
        }

        public bool GetExcelFlg()
        {
            return ExcelFlg;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ExcelFlg = true;
            MainFirstPageControl.ExcelFlg = ExcelFlg;
        }

    }
}
