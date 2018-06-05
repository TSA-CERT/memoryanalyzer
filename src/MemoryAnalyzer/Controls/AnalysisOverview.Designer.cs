namespace MemoryAnalyzer.Controls
{
    partial class AnalysisOverview
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtProjectName = new System.Windows.Forms.TextBox();
            this.txtMemoryImage = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtProfile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbBinary = new System.Windows.Forms.ComboBox();
            this.btnMemoryImage = new System.Windows.Forms.Button();
            this.btnProfile = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Project Name:";
            // 
            // txtProjectName
            // 
            this.txtProjectName.Location = new System.Drawing.Point(102, 7);
            this.txtProjectName.Name = "txtProjectName";
            this.txtProjectName.Size = new System.Drawing.Size(306, 22);
            this.txtProjectName.TabIndex = 1;
            this.txtProjectName.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // txtMemoryImage
            // 
            this.txtMemoryImage.Location = new System.Drawing.Point(102, 79);
            this.txtMemoryImage.Name = "txtMemoryImage";
            this.txtMemoryImage.Size = new System.Drawing.Size(306, 22);
            this.txtMemoryImage.TabIndex = 3;
            this.txtMemoryImage.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Memory Image:";
            // 
            // txtProfile
            // 
            this.txtProfile.Location = new System.Drawing.Point(102, 107);
            this.txtProfile.Name = "txtProfile";
            this.txtProfile.Size = new System.Drawing.Size(306, 22);
            this.txtProfile.TabIndex = 5;
            this.txtProfile.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Profile:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 16);
            this.label4.TabIndex = 6;
            this.label4.Text = "Analyzer:";
            // 
            // cmbBinary
            // 
            this.cmbBinary.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBinary.FormattingEnabled = true;
            this.cmbBinary.Location = new System.Drawing.Point(102, 35);
            this.cmbBinary.Name = "cmbBinary";
            this.cmbBinary.Size = new System.Drawing.Size(306, 24);
            this.cmbBinary.TabIndex = 7;
            // 
            // btnMemoryImage
            // 
            this.btnMemoryImage.Location = new System.Drawing.Point(415, 79);
            this.btnMemoryImage.Name = "btnMemoryImage";
            this.btnMemoryImage.Size = new System.Drawing.Size(119, 22);
            this.btnMemoryImage.TabIndex = 8;
            this.btnMemoryImage.Text = "Browse...";
            this.btnMemoryImage.UseVisualStyleBackColor = true;
            this.btnMemoryImage.Click += new System.EventHandler(this.btnMemoryImage_Click);
            // 
            // btnProfile
            // 
            this.btnProfile.Location = new System.Drawing.Point(415, 107);
            this.btnProfile.Name = "btnProfile";
            this.btnProfile.Size = new System.Drawing.Size(119, 22);
            this.btnProfile.TabIndex = 9;
            this.btnProfile.Text = "Autodetect";
            this.btnProfile.UseVisualStyleBackColor = true;
            this.btnProfile.Click += new System.EventHandler(this.btnProfile_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Enabled = false;
            this.btnUpdate.Location = new System.Drawing.Point(102, 156);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 10;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(183, 156);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // AnalysisOverview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnProfile);
            this.Controls.Add(this.btnMemoryImage);
            this.Controls.Add(this.cmbBinary);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtProfile);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtMemoryImage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtProjectName);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "AnalysisOverview";
            this.Size = new System.Drawing.Size(963, 508);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtProjectName;
        private System.Windows.Forms.TextBox txtMemoryImage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtProfile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbBinary;
        private System.Windows.Forms.Button btnMemoryImage;
        private System.Windows.Forms.Button btnProfile;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnCancel;
    }
}
