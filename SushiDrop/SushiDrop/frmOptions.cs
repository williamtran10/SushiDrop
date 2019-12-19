using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SushiDrop
{
    public partial class frmOptions : Form
    {
        public frmOptions()
        {
            InitializeComponent();
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            frmMain NewWindow;
            if (rad3Min.Checked == true) NewWindow = new frmMain(3);
            else if (rad10Min.Checked == true) NewWindow = new frmMain(10);
            else {
                int time;
                if (!String.IsNullOrWhiteSpace(txtOther.Text) && int.TryParse(txtOther.Text, out time) && time < 100) NewWindow = new frmMain(time);
                else {
                    MessageBox.Show("Time entered must be an integer less than 100.");
                    return;
                }
            }
            //make this form invisible while the new window opens, make it visible again when new window closes
            this.Visible = false;
            NewWindow.ShowDialog();
            NewWindow.Dispose();
            this.Visible = true;
        }

        private void radOther_CheckedChanged(object sender, EventArgs e)
        {
            if (radOther.Checked == true) txtOther.Enabled = true;
            else txtOther.Enabled = false;
        }

        private void LblDemo_Click(object sender, EventArgs e)
        {
            frmMain NewWindow = new frmMain();
            this.Visible = false;
            NewWindow.ShowDialog();
            NewWindow.Dispose();
            this.Visible = true;
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
