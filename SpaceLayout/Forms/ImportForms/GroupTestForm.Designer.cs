
namespace SpaceLayout.Forms.ZoneForms
{
    partial class GroupTestForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GroupTestForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.nDrawingDocumentTest = new Nevron.Diagram.NDrawingDocument();
            this.nDrawingView1 = new Nevron.Diagram.WinForm.NDrawingView();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 202F));
            this.tableLayoutPanel1.Controls.Add(this.nDrawingView1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(914, 696);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // nDrawingDocumentTest
            // 
            this.nDrawingDocumentTest.DesignTimeState = ((Nevron.Diagram.NBinaryState)(resources.GetObject("nDrawingDocumentTest.DesignTimeState")));
            // 
            // nDrawingView1
            // 
            this.nDrawingView1.AllowDrop = true;
            this.nDrawingView1.DesignTimeState = ((Nevron.Diagram.NBinaryState)(resources.GetObject("nDrawingView1.DesignTimeState")));
            this.nDrawingView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nDrawingView1.Document = this.nDrawingDocumentTest;
            this.nDrawingView1.Location = new System.Drawing.Point(3, 3);
            this.nDrawingView1.Name = "nDrawingView1";
            this.nDrawingView1.RenderTechnology = Nevron.GraphicsCore.RenderTechnology.GDIPlus;
            this.nDrawingView1.Size = new System.Drawing.Size(706, 690);
            this.nDrawingView1.TabIndex = 0;
            this.nDrawingView1.Text = "nDrawingView1";
            // 
            // GroupTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(914, 696);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "GroupTestForm";
            this.Text = "GroupTestForm";
            this.Load += new System.EventHandler(this.GroupTestForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Nevron.Diagram.NDrawingDocument nDrawingDocumentTest;
        private Nevron.Diagram.WinForm.NDrawingView nDrawingView1;
    }
}