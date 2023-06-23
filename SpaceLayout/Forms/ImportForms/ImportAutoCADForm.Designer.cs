namespace SpaceLayout.Forms.ImportForms
{
    partial class ImportAutoCADForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.vdImportBoundary = new vdControls.vdFramedControl();
            this.SuspendLayout();
            // 
            // vdImportBoundary
            // 
            this.vdImportBoundary.AccessibleRole = System.Windows.Forms.AccessibleRole.Pane;
            this.vdImportBoundary.DisplayPolarCoord = false;
            this.vdImportBoundary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vdImportBoundary.HistoryLines = ((uint)(4u));
            this.vdImportBoundary.LoadCommandstxt = true;
            this.vdImportBoundary.LoadMenutxt = true;
            this.vdImportBoundary.Location = new System.Drawing.Point(0, 0);
            this.vdImportBoundary.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.vdImportBoundary.Name = "vdImportBoundary";
            this.vdImportBoundary.PropertyGridWidth = ((uint)(300u));
            this.vdImportBoundary.Size = new System.Drawing.Size(968, 742);
            this.vdImportBoundary.TabIndex = 0;
            // 
            // ImportAutoCADForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(968, 742);
            this.Controls.Add(this.vdImportBoundary);
            this.Name = "ImportAutoCADForm";
            this.Text = "ImportBoundary";
            this.ResumeLayout(false);

        }

        #endregion

        private vdControls.vdFramedControl vdImportBoundary;
    }
}