using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceLayout.Forms.ImportForms
{
    public partial class ImportAutoCADForm : Form
    {
        public ImportAutoCADForm()
        {
            InitializeComponent();
            this.Load += IS_Load;
        }

        private void IS_Load(object sender, EventArgs e)
        {
            string FileName = string.Empty;
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.InitialDirectory = @"C:\temp\";
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (!String.IsNullOrWhiteSpace(ofd.FileName) && Path.GetExtension(ofd.FileName).ToLower() == ".dwg")
                {
                    string path = ofd.FileName;
                    //path = vdImportBoundary.BaseControl.ActiveDocument.GetOpenFileNameDlg(0, ofd.FileName, 0) as string;
                    vdImportBoundary.BaseControl.ActiveDocument.Open(path);
                }
                else
                {
                    MessageBox.Show("Please import AutoCAD file!");
                    return;
                }
            }   
        }
    }
}
