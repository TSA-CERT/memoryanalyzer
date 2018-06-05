namespace MemoryAnalyzer
{
    partial class Settings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.label1 = new System.Windows.Forms.Label();
            this.btnVolatility = new System.Windows.Forms.Button();
            this.txtVolatility = new System.Windows.Forms.TextBox();
            this.txtRekall = new System.Windows.Forms.TextBox();
            this.btnRekall = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Volatility Executable:";
            // 
            // btnVolatility
            // 
            this.btnVolatility.Location = new System.Drawing.Point(424, 8);
            this.btnVolatility.Name = "btnVolatility";
            this.btnVolatility.Size = new System.Drawing.Size(81, 23);
            this.btnVolatility.TabIndex = 1;
            this.btnVolatility.Text = "Browse...";
            this.btnVolatility.UseVisualStyleBackColor = true;
            this.btnVolatility.Click += new System.EventHandler(this.btnVolatility_Click);
            // 
            // txtVolatility
            // 
            this.txtVolatility.Location = new System.Drawing.Point(123, 10);
            this.txtVolatility.Name = "txtVolatility";
            this.txtVolatility.Size = new System.Drawing.Size(295, 20);
            this.txtVolatility.TabIndex = 2;
            // 
            // txtRekall
            // 
            this.txtRekall.Location = new System.Drawing.Point(123, 39);
            this.txtRekall.Name = "txtRekall";
            this.txtRekall.Size = new System.Drawing.Size(295, 20);
            this.txtRekall.TabIndex = 5;
            // 
            // btnRekall
            // 
            this.btnRekall.Location = new System.Drawing.Point(424, 37);
            this.btnRekall.Name = "btnRekall";
            this.btnRekall.Size = new System.Drawing.Size(81, 23);
            this.btnRekall.TabIndex = 4;
            this.btnRekall.Text = "Browse...";
            this.btnRekall.UseVisualStyleBackColor = true;
            this.btnRekall.Click += new System.EventHandler(this.btnRekall_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Rekall Executable:";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(424, 66);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(81, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(337, 66);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(81, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(527, 102);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtRekall);
            this.Controls.Add(this.btnRekall);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtVolatility);
            this.Controls.Add(this.btnVolatility);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.Text = "Select Analyzers";
            this.Load += new System.EventHandler(this.ImageSelection_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnVolatility;
        private System.Windows.Forms.TextBox txtVolatility;
        private System.Windows.Forms.TextBox txtRekall;
        private System.Windows.Forms.Button btnRekall;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}