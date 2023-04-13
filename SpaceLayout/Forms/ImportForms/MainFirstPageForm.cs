﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using yWorks.Controls.Input;
using yWorks.Controls;
using SpaceLayout.Entity;
using yWorks.Graph;
using SpaceLayout.Object;
using yWorks.Geometry;
using yWorks.Graph.Styles;
using yWorks.Graph.LabelModels;

namespace SpaceLayout.Forms.ZoneForms
{
    public partial class MainFirstPageControl : Form
    {
        private IGraph graph;
        public static bool ExcelFlg = false;
        public static DataTable dtZoneSelection = null;

        public MainFirstPageControl()
        {
            InitializeComponent();
            this.Load += IS_Load;
        }

        private void IS_Load(object sender, EventArgs e)
        {
            CrateModuleDatatable();
            btnPrevious.Visible = false;
            btnNext.Visible = false;

            toolStripButton2.Enabled = false;
            toolStripButton3.Enabled = false;
        }

        private void CrateModuleDatatable()
        {
            dtZoneSelection = new DataTable();
            dtZoneSelection.Columns.Add("ID");
            dtZoneSelection.Columns.Add("Name");
            //dtZoneSelection.Columns.Add("Department");
            dtZoneSelection.Columns.Add("Group");
            dtZoneSelection.Columns.Add("Relation");
            dtZoneSelection.Columns.Add("Category");
            dtZoneSelection.Columns.Add("Area");
            dtZoneSelection.Columns.Add("Width");
            dtZoneSelection.Columns.Add("Length");
            dtZoneSelection.Columns.Add("Height");
            dtZoneSelection.Columns.Add("Floor");
            dtZoneSelection.Columns.Add("Ratio");
            dtZoneSelection.Columns.Add("Type");
            dtZoneSelection.Columns.Add("Color");
        }
       
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            LoadRightPanel(2);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            LoadRightPanel(1);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            LoadRightPanel(3);
        }

        public void LoadRightPanel(int flg)
        {
            if (flg == 1)
            {
                if (tableLayoutPanel2.Controls.Count > 1)
                    tableLayoutPanel2.Controls.Remove(tableLayoutPanel2.GetControlFromPosition(0, 1));

                var ImportDataFrom = new ImportDataForm();
                tableLayoutPanel2.Controls.Add(ImportDataFrom, 0, 1);
                ImportDataFrom.Dock = DockStyle.Fill;
                btnNext.Visible = true;
                btnPrevious.Visible = false;
            }
            else if (flg == 2)
            {
                if (tableLayoutPanel2.Controls.Count > 1)
                    tableLayoutPanel2.Controls.Remove(tableLayoutPanel2.GetControlFromPosition(0, 1));

                var ZoneSelectionControl = new ZoneSelectionControl();
                tableLayoutPanel2.Controls.Add(ZoneSelectionControl, 0, 1);
                ZoneSelectionControl.Dock = DockStyle.Fill;
                btnNext.Visible = true;
                btnPrevious.Visible = true;
            }
            else if (flg == 3)
            {
                if (tableLayoutPanel2.Controls.Count > 1)
                    tableLayoutPanel2.Controls.Remove(tableLayoutPanel2.GetControlFromPosition(0, 1));

                var ZoneRelation = new ZoneRelationshipControl();
                tableLayoutPanel2.Controls.Add(ZoneRelation, 0, 1);
                ZoneRelation.Dock = DockStyle.Fill;
                btnNext.Visible = false;
                btnPrevious.Visible = true;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            var contrName = tableLayoutPanel2.GetControlFromPosition(0, 1).Name;
            if (contrName == "ImportDataForm" )
            {
                if(ExcelFlg == true)
                {
                    LoadRightPanel(2);
                    toolStripButton2.Enabled = true;
                    toolStripButton3.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Please click the 'Imoprt' button to import the data source first.");
                    toolStripButton2.Enabled = false;
                    toolStripButton3.Enabled = false;
                    return;
                }
            }
            else if (contrName == "ZoneSelectionControl")
            {
                LoadRightPanel(3);
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            var contrName = tableLayoutPanel2.GetControlFromPosition(0, 1).Name;

            if (contrName == "ZoneSelectionControl")
            {
                LoadRightPanel(1);
                toolStripButton2.Enabled = false;
                toolStripButton3.Enabled = false;
                ExcelFlg = false;
            }
            else if (contrName == "ZoneRelationshipControl")
            {
                LoadRightPanel(2);
            }
        }
    }
}
