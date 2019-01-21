namespace SushiDrop
{
    partial class frmOpeningScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOpeningScreen));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rad10Min = new System.Windows.Forms.RadioButton();
            this.rad3Min = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnInfo = new System.Windows.Forms.Button();
            this.btnHighScore = new System.Windows.Forms.Button();
            this.btnCredits = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.rad10Min);
            this.groupBox1.Controls.Add(this.rad3Min);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(43, 151);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(136, 90);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Choose Game Length:";
            // 
            // rad10Min
            // 
            this.rad10Min.AutoSize = true;
            this.rad10Min.Location = new System.Drawing.Point(12, 54);
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
            this.rad3Min.Location = new System.Drawing.Point(12, 25);
            this.rad3Min.Name = "rad3Min";
            this.rad3Min.Size = new System.Drawing.Size(70, 17);
            this.rad3Min.TabIndex = 0;
            this.rad3Min.TabStop = true;
            this.rad3Min.Text = "3 minutes";
            this.rad3Min.UseVisualStyleBackColor = true;
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
            this.label1.TabIndex = 1;
            this.label1.Text = "Sushi Drop!";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(185, 151);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(140, 90);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start!";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnInfo
            // 
            this.btnInfo.Location = new System.Drawing.Point(331, 151);
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.Size = new System.Drawing.Size(86, 26);
            this.btnInfo.TabIndex = 3;
            this.btnInfo.Text = "How To Play";
            this.btnInfo.UseVisualStyleBackColor = true;
            this.btnInfo.Click += new System.EventHandler(this.btnInfo_Click);
            // 
            // btnHighScore
            // 
            this.btnHighScore.Location = new System.Drawing.Point(331, 183);
            this.btnHighScore.Name = "btnHighScore";
            this.btnHighScore.Size = new System.Drawing.Size(86, 26);
            this.btnHighScore.TabIndex = 4;
            this.btnHighScore.Text = "Leaderboards";
            this.btnHighScore.UseVisualStyleBackColor = true;
            this.btnHighScore.Click += new System.EventHandler(this.btnHighScore_Click);
            // 
            // btnCredits
            // 
            this.btnCredits.Location = new System.Drawing.Point(331, 215);
            this.btnCredits.Name = "btnCredits";
            this.btnCredits.Size = new System.Drawing.Size(86, 26);
            this.btnCredits.TabIndex = 5;
            this.btnCredits.Text = "Credits";
            this.btnCredits.UseVisualStyleBackColor = true;
            this.btnCredits.Click += new System.EventHandler(this.btnCredits_Click);
            // 
            // frmOpeningScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(473, 299);
            this.Controls.Add(this.btnCredits);
            this.Controls.Add(this.btnHighScore);
            this.Controls.Add(this.btnInfo);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmOpeningScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sushi Drop!";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rad10Min;
        private System.Windows.Forms.RadioButton rad3Min;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnInfo;
        private System.Windows.Forms.Button btnHighScore;
        private System.Windows.Forms.Button btnCredits;
    }
}