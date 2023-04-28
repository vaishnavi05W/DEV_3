namespace SpaceLayout.Forms
{
    partial class TestForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestForm));
            this.nDrawingView1 = new Nevron.Diagram.WinForm.NDrawingView();
            this.nDrawingDocument1 = new Nevron.Diagram.NDrawingDocument();
            this.SuspendLayout();
            // 
            // nDrawingView1
            // 
            this.nDrawingView1.AllowDrop = true;
            this.nDrawingView1.DesignTimeState = ((Nevron.Diagram.NBinaryState)(resources.GetObject("nDrawingView1.DesignTimeState")));
            this.nDrawingView1.Document = this.nDrawingDocument1;
            this.nDrawingView1.Location = new System.Drawing.Point(50, 16);
            this.nDrawingView1.Name = "nDrawingView1";
            this.nDrawingView1.RenderTechnology = Nevron.GraphicsCore.RenderTechnology.GDIPlus;
            this.nDrawingView1.Size = new System.Drawing.Size(687, 399);
            this.nDrawingView1.TabIndex = 0;
            this.nDrawingView1.Text = "nDrawingView1";
            // 
            // nDrawingDocument1
            // 
            this.nDrawingDocument1.DesignTimeState = ((Nevron.Diagram.NBinaryState)(resources.GetObject("nDrawingDocument1.DesignTimeState")));
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.nDrawingView1);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Nevron.Diagram.WinForm.NDrawingView nDrawingView1;
        private Nevron.Diagram.NDrawingDocument nDrawingDocument1;
    }
}