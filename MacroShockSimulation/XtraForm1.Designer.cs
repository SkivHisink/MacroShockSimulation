namespace MacroShockSimulation
{
    partial class MacroShockSimulator
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
            this.xtraUserControl11 = new MacroShockSimulation.XtraUserControl1();
            this.SuspendLayout();
            // 
            // xtraUserControl11
            // 
            this.xtraUserControl11.Location = new System.Drawing.Point(12, 12);
            this.xtraUserControl11.Name = "xtraUserControl11";
            this.xtraUserControl11.Size = new System.Drawing.Size(1222, 1017);
            this.xtraUserControl11.TabIndex = 0;
            // 
            // MacroShockSimulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1234, 1025);
            this.Controls.Add(this.xtraUserControl11);
            this.MaximumSize = new System.Drawing.Size(1236, 1072);
            this.MinimumSize = new System.Drawing.Size(1236, 1072);
            this.Name = "MacroShockSimulator";
            this.Text = "MacroShockSimulator";
            this.ResumeLayout(false);

        }

        #endregion

        private XtraUserControl1 xtraUserControl11;
    }
}