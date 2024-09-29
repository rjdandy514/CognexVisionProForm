namespace CognexVisionProForm
{
    partial class ShowResultData
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
            this.tbToolMessage = new System.Windows.Forms.RichTextBox();
            this.lbToolResultData = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // tbToolMessage
            // 
            this.tbToolMessage.Location = new System.Drawing.Point(12, 107);
            this.tbToolMessage.Name = "tbToolMessage";
            this.tbToolMessage.Size = new System.Drawing.Size(506, 96);
            this.tbToolMessage.TabIndex = 0;
            this.tbToolMessage.Text = "";
            // 
            // lbToolResultData
            // 
            this.lbToolResultData.FormattingEnabled = true;
            this.lbToolResultData.Location = new System.Drawing.Point(12, 6);
            this.lbToolResultData.Name = "lbToolResultData";
            this.lbToolResultData.Size = new System.Drawing.Size(506, 95);
            this.lbToolResultData.TabIndex = 1;
            // 
            // ShowResultData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lbToolResultData);
            this.Controls.Add(this.tbToolMessage);
            this.Name = "ShowResultData";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.ShowResultData_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox tbToolMessage;
        private System.Windows.Forms.ListBox lbToolResultData;
    }
}