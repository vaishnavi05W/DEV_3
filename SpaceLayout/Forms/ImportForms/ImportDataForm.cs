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
using SpaceLayout.Forms.ImportForms;
using System.IO;

namespace SpaceLayout.Forms.ZoneForms
{
    public partial class ImportDataForm : UserControl
    {
        public bool ExcelFlg = false;
        public bool CADflg = false;
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
            CADflg = true;
           
            ImportAutoCADForm f = new ImportAutoCADForm();
            f.ShowDialog();
            //// Create a drawing document
            //NDrawingDocument drawing = new NDrawingDocument();

            //// create a new persistency manager
            //NPersistencyManager persistencyManager = new NPersistencyManager();

            //// load a drawing from the XML file
            //drawing = persistencyManager.LoadDrawingFromFile(ofd.FileName);

            //// display the drawing
            //Ndv.Document = drawing;

            //ZoneConnectorData("2");
               

            
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
