namespace WindowsFormsApplication2
{
    partial class frmPalette
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
            this.components = new System.ComponentModel.Container();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.pnlInfo = new System.Windows.Forms.Panel();
            this.txtData = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.lblInfoDescription = new System.Windows.Forms.Label();
            this.lblInfoName = new System.Windows.Forms.Label();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.pnlButtons.SuspendLayout();
            this.pnlInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.tabControl1);
            this.pnlButtons.Location = new System.Drawing.Point(12, 12);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(520, 623);
            this.pnlButtons.TabIndex = 5;
            // 
            // tabControl1
            // 
            this.tabControl1.Location = new System.Drawing.Point(0, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(508, 606);
            this.tabControl1.TabIndex = 0;
            // 
            // pnlInfo
            // 
            this.pnlInfo.Controls.Add(this.txtData);
            this.pnlInfo.Controls.Add(this.button2);
            this.pnlInfo.Controls.Add(this.button1);
            this.pnlInfo.Controls.Add(this.lblInfoDescription);
            this.pnlInfo.Controls.Add(this.lblInfoName);
            this.pnlInfo.Location = new System.Drawing.Point(543, 12);
            this.pnlInfo.Name = "pnlInfo";
            this.pnlInfo.Size = new System.Drawing.Size(235, 623);
            this.pnlInfo.TabIndex = 6;
            // 
            // txtData
            // 
            this.txtData.Location = new System.Drawing.Point(19, 311);
            this.txtData.Multiline = true;
            this.txtData.Name = "txtData";
            this.txtData.ReadOnly = true;
            this.txtData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtData.Size = new System.Drawing.Size(186, 164);
            this.txtData.TabIndex = 15;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(19, 253);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(186, 23);
            this.button2.TabIndex = 12;
            this.button2.Text = "Test Remove Module";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(19, 282);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(186, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "Test Serial";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblInfoDescription
            // 
            this.lblInfoDescription.AutoSize = true;
            this.lblInfoDescription.Location = new System.Drawing.Point(16, 54);
            this.lblInfoDescription.Name = "lblInfoDescription";
            this.lblInfoDescription.Size = new System.Drawing.Size(0, 13);
            this.lblInfoDescription.TabIndex = 10;
            // 
            // lblInfoName
            // 
            this.lblInfoName.AutoSize = true;
            this.lblInfoName.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfoName.Location = new System.Drawing.Point(13, 18);
            this.lblInfoName.Name = "lblInfoName";
            this.lblInfoName.Size = new System.Drawing.Size(0, 31);
            this.lblInfoName.TabIndex = 8;
            this.lblInfoName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // frmPalette
            // 
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(799, 640);
            this.Controls.Add(this.pnlInfo);
            this.Controls.Add(this.pnlButtons);
            this.Name = "frmPalette";
            this.Text = "Palette Controller";
            this.pnlButtons.ResumeLayout(false);
            this.pnlInfo.ResumeLayout(false);
            this.pnlInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Panel pnlInfo;
        private System.Windows.Forms.Label lblInfoName;
        private System.Windows.Forms.Label lblInfoDescription;
        private System.Windows.Forms.TabControl tabControl1;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtData;

    }
}

