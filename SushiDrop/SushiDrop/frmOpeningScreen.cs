using System;
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
            NewWindow = new frmMain(3);
                
            //make this form invisible while the new window opens, make it visible again when new window closes
            this.Visible = false;
            NewWindow.ShowDialog();
            NewWindow.Dispose();
            this.Visible = true;
            
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
            MessageBox.Show("Objective: \nThe objective of this game is to match 3 or more of the same sushi to get the highest score possible.\n" +
                           "\nControls: \nWhen a possible match of 3 or more is found, click on the one sushi to select it, then click on an adjacent sushi to perform a swap and check for a possible match. If the match is not found, the boxes will return to their original positions.\n" +
                           "\nCombos: \nWhen boxes have settled after dropping, if another match is found then the combo counter will increase. Each increase in the combo counter will multiply the score value of all subsequent matches by 2. Therefore, the combo increases the score value exponentially. The combo counter resets after no match is found.\n" +
                           "\nPoint System: \nEach match is worth more based on how many sushis are in it. A match of 3 has a base point value 30 each, a match of 4 has a base point value of 40 each, and so on." +
                           "\n\n Other Controls:\n H - Hint (Combo Multiplier will be reduced to 0.5x for that turn) \n R - Reset Board (All progress will be lost)");
        }

        private void LblGameOptions_Click(object sender, EventArgs e)
        {
            frmOptions Options = new frmOptions();
            this.Visible = false;
            Options.ShowDialog();
            Options.Dispose();
            this.Visible = true;
        }
    }
}
