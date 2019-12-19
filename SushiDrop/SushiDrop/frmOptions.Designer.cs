namespace SushiDrop
{
    partial class frmOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOptions));
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rad10Min = new System.Windows.Forms.RadioButton();
            this.rad3Min = new System.Windows.Forms.RadioButton();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.lblDemo = new System.Windows.Forms.Button();
            this.radOther = new System.Windows.Forms.RadioButton();
            this.txtOther = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 55F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(22, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(434, 85);
            this.label1.TabIndex = 2;
            this.label1.Text = "Sushi Drop!";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.txtOther);
            this.groupBox1.Controls.Add(this.radOther);
            this.groupBox1.Controls.Add(this.rad10Min);
            this.groupBox1.Controls.Add(this.rad3Min);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(37, 120);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(136, 105);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Choose Game Length:";
            // 
            // rad10Min
            // 
            this.rad10Min.AutoSize = true;
            this.rad10Min.Location = new System.Drawing.Point(15, 50);
            this.rad10Min.Name = "rad10Min";
            this.rad10Min.Size = new System.Drawing.Size(76, 17);
            this.rad10Min.TabIndex = 1;
            this.rad10Min.TabStop = true;
            this.rad10Min.Text = "10 minutes";
            this.rad10Min.UseVisualStyleBackColor = true;
            // 
            // rad3Min
            // 
            this.rad3Min.AutoSize = true;
            this.rad3Min.Location = new System.Drawing.Point(15, 27);
            this.rad3Min.Name = "rad3Min";
            this.rad3Min.Size = new System.Drawing.Size(70, 17);
            this.rad3Min.TabIndex = 0;
            this.rad3Min.TabStop = true;
            this.rad3Min.Text = "3 minutes";
            this.rad3Min.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.btnStart.Location = new System.Drawing.Point(296, 215);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(140, 49);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Play!";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnBack.Location = new System.Drawing.Point(37, 231);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(123, 33);
            this.btnBack.TabIndex = 5;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.BtnBack_Click);
            // 
            // lblDemo
            // 
            this.lblDemo.BackColor = System.Drawing.Color.WhiteSmoke;
            this.lblDemo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblDemo.Location = new System.Drawing.Point(166, 231);
            this.lblDemo.Name = "lblDemo";
            this.lblDemo.Size = new System.Drawing.Size(124, 33);
            this.lblDemo.TabIndex = 6;
            this.lblDemo.Text = "Run Demo";
            this.lblDemo.UseVisualStyleBackColor = false;
            this.lblDemo.Click += new System.EventHandler(this.LblDemo_Click);
            // 
            // radOther
            // 
            this.radOther.AutoSize = true;
            this.radOther.Location = new System.Drawing.Point(15, 73);
            this.radOther.Name = "radOther";
            this.radOther.Size = new System.Drawing.Size(54, 17);
            this.radOther.TabIndex = 2;
            this.radOther.TabStop = true;
            this.radOther.Text = "Other:";
            this.radOther.UseVisualStyleBackColor = true;
            this.radOther.CheckedChanged += new System.EventHandler(this.radOther_CheckedChanged);
            // 
            // txtOther
            // 
            this.txtOther.Enabled = false;
            this.txtOther.Location = new System.Drawing.Point(75, 72);
            this.txtOther.Name = "txtOther";
            this.txtOther.Size = new System.Drawing.Size(42, 20);
            this.txtOther.TabIndex = 7;
            // 
            // frmOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(473, 299);
            this.Controls.Add(this.lblDemo);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sushi Drop!";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rad10Min;
        private System.Windows.Forms.RadioButton rad3Min;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button lblDemo;
        private System.Windows.Forms.TextBox txtOther;
        private System.Windows.Forms.RadioButton radOther;
    }
}