namespace consoleXstream.Menu
{
    partial class ShowMenu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected new void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tmrMenu = new System.Windows.Forms.Timer(this.components);
            this.imgDisplay = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgDisplay)).BeginInit();
            //this.SuspendLayout();
            // 
            // tmrMenu
            // 
            this.tmrMenu.Interval = 1;
            this.tmrMenu.Tick += new System.EventHandler(tmrMenu_Tick);
            // 
            // imgDisplay
            // 
            this.imgDisplay.Location = new System.Drawing.Point(51, 31);
            this.imgDisplay.Name = "imgDisplay";
            this.imgDisplay.Size = new System.Drawing.Size(100, 50);
            this.imgDisplay.TabIndex = 0;
            this.imgDisplay.TabStop = false;
            this.imgDisplay.Click += new System.EventHandler(this.imgDisplay_Click);
            this.imgDisplay.MouseEnter += new System.EventHandler(this.imgDisplay_MouseEnter);
            this.imgDisplay.MouseLeave += new System.EventHandler(this.imgDisplay_MouseLeave);
            this.imgDisplay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.imgDisplay_MouseMove);
            // 
            // frmMenu
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(723, 451);
            this.Controls.Add(this.imgDisplay);
            this.Name = "FrmMenu";
            this.Text = "frmMenu";
            this.Load += new System.EventHandler(this.frmMenu_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imgDisplay)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer tmrMenu;
        private System.Windows.Forms.PictureBox imgDisplay;
    }
}