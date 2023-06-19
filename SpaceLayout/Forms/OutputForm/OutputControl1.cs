using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office;
using System.IO;
using System.Diagnostics;
using Microsoft.Office.Interop;
using Excel = Microsoft.Office.Interop.Excel;

namespace SpaceLayout.Forms.ZoneForms
{
    public partial class OutputControl1 : UserControl
    {
        private DataTable dtExcelExport = new DataTable();
        public OutputControl1(DataTable dtZoneRelationshipControl)
        {
            InitializeComponent();
            this.Load += IS_Load;
            dtExcelExport = dtZoneRelationshipControl;
        }

        private void IS_Load(object sender, EventArgs e)
        {
            btnExport.Click += btnExport_Clicked;
        }

        private void btnExport_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (checkedListBox1.CheckedItems.Count > 0)
                {
                    for (int x = 0; x < checkedListBox1.CheckedItems.Count; x++)
                    {
                        if(checkedListBox1.CheckedItems[x].ToString() == "관계도 .xlsx")
                        {
                            if (dtExcelExport.Rows.Count > 0)
                            {
                                string folderPath = "C:\\temp\\";
                                if (!Directory.Exists(folderPath))
                                {
                                    Directory.CreateDirectory(folderPath);
                                }
                                SaveFileDialog savedialog = new SaveFileDialog();
                                savedialog.Filter = "Excel Files|*.xlsx;";
                                savedialog.Title = "Save";
                                savedialog.FileName = "관계도";
                                savedialog.InitialDirectory = folderPath;

                                savedialog.RestoreDirectory = true;

                                if (savedialog.ShowDialog() == DialogResult.OK)
                                {
                                    if (Path.GetExtension(savedialog.FileName).Contains(".xlsx"))
                                    {
                                        if(ExportDataTableToExcel(dtExcelExport, savedialog.FileName))
                                        {
                                            DialogResult result = MessageBox.Show("Save Successful", "", MessageBoxButtons.OK);
                                            if (result == DialogResult.OK)
                                            {
                                                Process.Start(Path.GetDirectoryName(savedialog.FileName));
                                            }
                                            else
                                            {
                                                Process.Start(Path.GetDirectoryName(savedialog.FileName));
                                            }
                                            
                                        }
                                            
                                    }
                                }
                            }
                        }
                    }
                   
                }
                else
                {
                    MessageBox.Show("Please check at least one item.");
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

       
        public static bool ExportDataTableToExcel(DataTable dt, string filepath)
        {
            if (File.Exists(filepath))
            {
                try
                {

                    using (Stream stream = new FileStream(filepath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        // Here you can copy your file
                        // then rename the copied file
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("File is in use!! Close it and try again");
                    return false;
                }
            }
              
            Excel.Application oXL;
            Excel.Workbook oWB;
            Excel.Worksheet oSheet;
            Excel.Range oRange;

            try
            {
                // Start Excel and get Application object. 
                oXL = new Excel.Application();

                // Set some properties 
                oXL.Visible = true;
                oXL.DisplayAlerts = false;

                // Get a new workbook. 
                oWB = oXL.Workbooks.Add(System.Reflection.Missing.Value);

                // Get the Active sheet 
                oSheet = (Excel.Worksheet)oWB.ActiveSheet;
                oSheet.Name = "Data";

                int rowCount = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    rowCount += 1;
                    for (int i = 1; i < dt.Columns.Count + 1; i++)
                    {
                        // Add the header the first time through 
                        if (rowCount == 2)
                        {
                            oSheet.Cells[1, i] = dt.Columns[i - 1].ColumnName;
                        }
                        oSheet.Cells[rowCount, i] = dr[i - 1].ToString();
                    }
                }

                // Save the sheet and close 
                oSheet = null;
                oRange = null;
                oWB.SaveCopyAs(filepath);
                oWB = null;
                oXL.Quit();
        }
            catch
            {
                throw;
            }
            finally
            {
                // Clean up 
                // NOTE: When in release mode, this does the trick 
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }

            return true;
        }
    }
}
