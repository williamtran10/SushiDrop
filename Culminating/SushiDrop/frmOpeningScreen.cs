using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//William Tran and Adrian Cerejo
//Opening Form
//Jan 16, 2018
//Acts as a main menu before the game starts

namespace SushiDrop
{
    public partial class frmOpeningScreen : Form
    {
        public frmOpeningScreen()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //makes a new game window based on the time limit chosen
            frmMain NewWindow;
            if (rad3Min.Checked)
                NewWindow = new frmMain(3);
            else
                NewWindow = new frmMain(10);
                
            //make this form invisible while the new window opens, make it visible again when new window closes
            if (NewWindow.IsDisposed == false) //if there were no possible moves from the start then the window would automatically selfdispose
            {
                this.Visible = false;
                NewWindow.ShowDialog();
                NewWindow.Dispose();
                this.Visible = true;
            }
            
        }

        private void btnHighScore_Click(object sender, EventArgs e)
        {
            //make and show leaderboards form, making this form invisible while the new one is open 
            frmLeaderboards LeaderBoards = new frmLeaderboards();
            this.Visible = false;
            LeaderBoards.ShowDialog();
            LeaderBoards.Dispose();
            this.Visible = true;
        }

        private void btnCredits_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This is a game made by William Tran and Adrian Cerejo over the course of December 2018 to January 2019.\nImages used were taken from the Sushi Go board game by Gamewright.\nThanks for playing!", "Sushi Drop Credits");
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Objective: \nThe objective of this game is to match 3 or more of the same sushi and get the highest score possible.\n" +
                            "\nControls: \nWhen a possible match of 3 or more is found, click on the two boxes that will make the match possible. If the match is not possible, the boxes will return to their original positions.\n" +
                            "\nCombos: \nWhen boxes begin dropping, if matches are pre-existing, the combo multiplier will give 2x the acquired points. \n" +
                            "\nPoint System: \nEach sushi is worth 10 * amount matched. (i.e. Match of 3 is 10 * 3 since there are 3 sushi's matched totaling up to 90)" + 
                            "\n\n Other Controls:\n H - Hint (Combo Multiplier will be reduced to 0.5x for that turn) \n R - Reset Board (All progress will be lost) ");
        }
    }
}
