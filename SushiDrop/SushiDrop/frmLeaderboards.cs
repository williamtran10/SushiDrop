using System;
using System.Windows.Forms;
using System.IO;

//William Tran and Adrian Cerejo
//Leaderboards Form
//Dec 17, 2018
//This form displays the top 3 high scores for each mode (3 minutes and 10 minutes)

namespace SushiDrop
{
    public partial class frmLeaderboards : Form
    {
        //uses string to access the highscores text file
        string HighScores = "HighScores.txt";

        public frmLeaderboards()
        {
            InitializeComponent();
            //Displays leaderboards when opened
            Output();
        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            //close form
            this.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            //shor confirm messagebox
            if (MessageBox.Show("Are you sure you want to reset all scores?", "Reset Scores", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //Empty the file and output
                using (System.IO.StreamWriter ScoreWriter = new StreamWriter(HighScores))
                    ScoreWriter.Close();
                Output();
            }
        }

        public void Output()
        {
            //6 strings to hold each high score value
            string Score3First, Score3Second, Score3Third, Score10First, Score10Second, Score10Third;

            //Read in all scores from text file to display later
            using (StreamReader ScoreReader = new StreamReader(HighScores))
            {
                Score3First = ScoreReader.ReadLine();
                Score3Second = ScoreReader.ReadLine();
                Score3Third = ScoreReader.ReadLine();
                Score10First = ScoreReader.ReadLine();
                Score10Second = ScoreReader.ReadLine();
                Score10Third = ScoreReader.ReadLine();
            }

            //if any of the scores are null set them to 0
            if (Score3First == null) Score3First = "0";
            if (Score3Second == null) Score3Second = "0";
            if (Score3Third == null) Score3Third = "0";
            if (Score10First == null) Score10First = "0";
            if (Score10Second == null) Score10Second = "0";
            if (Score10Third == null) Score10Third = "0";
            
            //display each score in its respective label
            lbl3Min1.Text = Score3First;
            lbl3Min2.Text = Score3Second;
            lbl3Min3.Text = Score3Third;
            lbl10Min1.Text = Score10First;
            lbl10Min2.Text = Score10Second;
            lbl10Min3.Text = Score10Third;

        }
    }
}
