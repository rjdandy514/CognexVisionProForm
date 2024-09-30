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
            this.label9 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbPreprocessResultdata = new System.Windows.Forms.ListBox();
            this.tbPreprocessMessage = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // tbToolMessage
            // 
            this.tbToolMessage.Location = new System.Drawing.Point(12, 342);
            this.tbToolMessage.Name = "tbToolMessage";
            this.tbToolMessage.Size = new System.Drawing.Size(453, 96);
            this.tbToolMessage.TabIndex = 0;
            this.tbToolMessage.Text = "";
            // 
            // lbToolResultData
            // 
            this.lbToolResultData.FormattingEnabled = true;
            this.lbToolResultData.Location = new System.Drawing.Point(12, 252);
            this.lbToolResultData.Name = "lbToolResultData";
            this.lbToolResultData.Size = new System.Drawing.Size(453, 56);
            this.lbToolResultData.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(12, 225);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(165, 24);
            this.label9.TabIndex = 38;
            this.label9.Text = "Tool Block Results";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 315);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(181, 24);
            this.label1.TabIndex = 39;
            this.label1.Text = "Tool Block Message";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(188, 24);
            this.label2.TabIndex = 43;
            this.label2.Text = "Preprocess Message";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(172, 24);
            this.label3.TabIndex = 42;
            this.label3.Text = "Preprocess Results";
            // 
            // lbPreprocessResultdata
            // 
            this.lbPreprocessResultdata.FormattingEnabled = true;
            this.lbPreprocessResultdata.Location = new System.Drawing.Point(12, 36);
            this.lbPreprocessResultdata.Name = "lbPreprocessResultdata";
            this.lbPreprocessResultdata.Size = new System.Drawing.Size(453, 56);
            this.lbPreprocessResultdata.TabIndex = 41;
            // 
            // tbPreprocessMessage
            // 
            this.tbPreprocessMessage.Location = new System.Drawing.Point(12, 126);
            this.tbPreprocessMessage.Name = "tbPreprocessMessage";
            this.tbPreprocessMessage.Size = new System.Drawing.Size(453, 96);
            this.tbPreprocessMessage.TabIndex = 40;
            this.tbPreprocessMessage.Text = "";
            // 
            // ShowResultData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(925, 569);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbPreprocessResultdata);
            this.Controls.Add(this.tbPreprocessMessage);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lbToolResultData);
            this.Controls.Add(this.tbToolMessage);
            this.Name = "ShowResultData";
            this.Text = "x";
            this.Load += new System.EventHandler(this.ShowResultData_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox tbToolMessage;
        private System.Windows.Forms.ListBox lbToolResultData;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lbPreprocessResultdata;
        private System.Windows.Forms.RichTextBox tbPreprocessMessage;
    }
}