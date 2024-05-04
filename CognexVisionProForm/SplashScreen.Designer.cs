namespace CognexVisionProForm
{
    partial class SplashScreen
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
            this.txtArchive = new System.Windows.Forms.Label();
            this.pbLoadApp = new System.Windows.Forms.ProgressBar();
            this.lbLoadProgress = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtArchive
            // 
            this.txtArchive.AutoSize = true;
            this.txtArchive.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtArchive.Location = new System.Drawing.Point(25, 175);
            this.txtArchive.Name = "txtArchive";
            this.txtArchive.Size = new System.Drawing.Size(763, 73);
            this.txtArchive.TabIndex = 13;
            this.txtArchive.Text = "LOADING APPLICATION";
            // 
            // pbLoadApp
            // 
            this.pbLoadApp.Location = new System.Drawing.Point(38, 278);
            this.pbLoadApp.Name = "pbLoadApp";
            this.pbLoadApp.Size = new System.Drawing.Size(750, 23);
            this.pbLoadApp.TabIndex = 14;
            this.pbLoadApp.Value = 100;
            // 
            // lbLoadProgress
            // 
            this.lbLoadProgress.AutoSize = true;
            this.lbLoadProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLoadProgress.Location = new System.Drawing.Point(337, 331);
            this.lbLoadProgress.Name = "lbLoadProgress";
            this.lbLoadProgress.Size = new System.Drawing.Size(139, 15);
            this.lbLoadProgress.TabIndex = 15;
            this.lbLoadProgress.Text = "LOADING APPLICATION";
            this.lbLoadProgress.Click += new System.EventHandler(this.lbLoadProgress_Click);
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.ControlBox = false;
            this.Controls.Add(this.lbLoadProgress);
            this.Controls.Add(this.pbLoadApp);
            this.Controls.Add(this.txtArchive);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 450);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 450);
            this.Name = "SplashScreen";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Splish Splash";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label txtArchive;
        private System.Windows.Forms.ProgressBar pbLoadApp;
        private System.Windows.Forms.Label lbLoadProgress;
    }
}