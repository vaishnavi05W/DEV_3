
namespace SpaceLayout.Forms.GenerativeForms
{
    partial class GenerativeInfomationForms
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dgvZoneRelationship = new System.Windows.Forms.DataGridView();
            this.colFloor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHorizontal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVertical = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvZoneRelationship)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.dgvZoneRelationship, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 57.52212F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 42.47787F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(360, 610);
            this.tableLayoutPanel1.TabIndex = 1;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // dgvZoneRelationship
            // 
            this.dgvZoneRelationship.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgvZoneRelationship.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvZoneRelationship.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFloor,
            this.colHorizontal,
            this.colVertical});
            this.dgvZoneRelationship.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvZoneRelationship.GridColor = System.Drawing.SystemColors.Control;
            this.dgvZoneRelationship.Location = new System.Drawing.Point(3, 353);
            this.dgvZoneRelationship.Name = "dgvZoneRelationship";
            this.dgvZoneRelationship.RowHeadersWidth = 51;
            this.dgvZoneRelationship.Size = new System.Drawing.Size(354, 254);
            this.dgvZoneRelationship.TabIndex = 3;
            this.dgvZoneRelationship.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvZoneRelationship_CellContentClick);
            // 
            // colFloor
            // 
            this.colFloor.HeaderText = "Floor";
            this.colFloor.MinimumWidth = 6;
            this.colFloor.Name = "colFloor";
            this.colFloor.Width = 125;
            // 
            // colHorizontal
            // 
            this.colHorizontal.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colHorizontal.DataPropertyName = "Horizontal";
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.colHorizontal.DefaultCellStyle = dataGridViewCellStyle1;
            this.colHorizontal.FillWeight = 111.5168F;
            this.colHorizontal.HeaderText = "Horizontal";
            this.colHorizontal.MinimumWidth = 6;
            this.colHorizontal.Name = "colHorizontal";
            // 
            // colVertical
            // 
            this.colVertical.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colVertical.DataPropertyName = "Vertical";
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.colVertical.DefaultCellStyle = dataGridViewCellStyle2;
            this.colVertical.FillWeight = 111.5168F;
            this.colVertical.HeaderText = "Vertical";
            this.colVertical.MinimumWidth = 6;
            this.colVertical.Name = "colVertical";
            // 
            // GenerativeInfomationForms
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "GenerativeInfomationForms";
            this.Size = new System.Drawing.Size(360, 610);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvZoneRelationship)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView dgvZoneRelationship;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFloor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHorizontal;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVertical;
    }
}
