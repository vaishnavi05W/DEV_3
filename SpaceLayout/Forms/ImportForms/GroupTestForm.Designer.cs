
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
            this.nDrawingView1 = new Nevron.Diagram.WinForm.NDrawingView();
            this.Nfd = new Nevron.Diagram.NDrawingDocument();
            this.SuspendLayout();
            // 
            // nDrawingView1
            // 
            this.nDrawingView1.AllowDrop = true;
            this.nDrawingView1.DesignTimeState = ((Nevron.Diagram.NBinaryState)(resources.GetObject("nDrawingView1.DesignTimeState")));
            this.nDrawingView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nDrawingView1.Document = null;
            this.nDrawingView1.Location = new System.Drawing.Point(0, 0);
            this.nDrawingView1.Name = "nDrawingView1";
            this.nDrawingView1.RenderTechnology = Nevron.GraphicsCore.RenderTechnology.GDIPlus;
            this.nDrawingView1.Size = new System.Drawing.Size(800, 450);
            this.nDrawingView1.TabIndex = 0;
            this.nDrawingView1.Text = "nDrawingView1";
            // 
            // Nfd
            // 
            this.Nfd.DesignTimeState = ((Nevron.Diagram.NBinaryState)(resources.GetObject("Nfd.DesignTimeState")));
            // 
            // GroupTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.nDrawingView1);
            this.Name = "GroupTestForm";
            this.Text = "GroupTestForm";
            this.Load += new System.EventHandler(this.GroupTestForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Nevron.Diagram.WinForm.NDrawingView nDrawingView1;
        private Nevron.Diagram.NDrawingDocument Nfd;
    }
}