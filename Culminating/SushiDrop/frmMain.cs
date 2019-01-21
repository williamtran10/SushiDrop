using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

//William Tran and Adrian Cerejo
//Main form
//Dec 6, 2018
//This form has the game and the logic that it uses

namespace SushiDrop
{
    public partial class frmMain : Form
    {
        //global constants
        const int TileSize = 60; //px height and width of tile
        const int BoardOffset = 60; //px difference from left side of board to left side of window, also from top of board to top of window
        readonly int TimeLimit; //time limit chosen, this is given through the constuctor
        
        //global variables
        Board mBoard = new Board(9, 9, TileSize, BoardOffset);
        public int[,] IntArray;
        
        string HighScoreFile = "HighScores.txt";

        List<Point> GlobalDeleteList = new List<Point>();
        List<int> ColumnsToDrop = new List<int>();
        List<int> UniqueValuesToDelete = new List<int>();
        List<Label> PointLabelList = new List<Label>();

        Point[] HintPoints = new Point[3];
        Tile[] HintPics = new Tile[3];
        bool HintUsed;
        bool GameIsOver = false;
        Point ClickOne;
        Point ClickTwo;
        Point Difference;
        int Score;
        int AnimationTickCounter;
    	int AnimationLength;
    	int ComboCounter;
        Tile SelectedTile;

        Stopwatch Stopwatch = new Stopwatch();

        enum State
        {
            Idle,
            Swapping,
            SwappingBack,
            Deleting,
            Dropping,
        };
        State GameState = State.Idle;

        public frmMain(int GameLength)
        {
            InitializeComponent();
            
            //Create highscore file
            if (File.Exists(HighScoreFile) == false)
                File.CreateText(HighScoreFile);

            TimeLimit = GameLength;

            Reset();
            
            //add each picturebox in mBoard to the form
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    this.Controls.Add(mBoard.Tiles[i, j]);
        }

        private void Reset()
        {
            //clear and remake entire board
            Random r = new Random();
            IntArray = new int[9, 9];

            int RandomNum = 0;

            //randomize each value in the int array
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    do
                    {
                        do
                        {
                            RandomNum = r.Next(1, 7);
                        } while (IntArray[i, j] == RandomNum); //keep doing this until it is a new number
                        IntArray[i, j] = RandomNum;
                    } while (CheckForThree(i, j, true)); //if a match of 3 is made, rerandomize it
                }
            }

            //IntArray = new int[,] {
            //    {1, 1, 2, 2, 3, 3, 4, 4, 5},
            //    {5, 1, 1, 2, 2, 3, 3, 4, 4},
            //    {5, 5, 1, 1, 2, 2, 3, 3, 4},
            //    {1, 1, 2, 2, 3, 3, 4, 4, 5},
            //    {5, 1, 1, 2, 2, 3, 3, 4, 4},
            //    {5, 5, 1, 1, 2, 2, 3, 3, 4},
            //    {1, 1, 2, 2, 3, 3, 4, 4, 5},
            //    {5, 1, 1, 2, 2, 3, 3, 4, 4},
            //    {5, 5, 1, 1, 2, 2, 3, 3, 4}
            //};

            //IntArray = new int[,] {
            //    {1, 2, 3, 4, 5, 6, 1, 2, 3},
            //    {6, 1, 2, 3, 4, 5, 6, 1, 2},
            //    {5, 6, 1, 2, 3, 4, 5, 6, 1},
            //    {1, 2, 3, 4, 5, 6, 1, 2, 3},
            //    {6, 1, 2, 3, 4, 5, 6, 1, 2},
            //    {5, 6, 1, 2, 3, 4, 5, 6, 1},
            //    {1, 2, 3, 4, 5, 6, 1, 2, 3},
            //    {6, 1, 2, 3, 4, 5, 6, 1, 2},
            //    {5, 6, 1, 2, 3, 4, 5, 6, 1}
            //};

            //reset values to what they were at the start
            Score = 0;
            lblScore.Text = Score.ToString();
            

            lblComboHeading.Text = "Combo (Point Multiplier):";
            ComboCounter = 0;
            lblCombo.Text = "0 (0x)";
            lblHintUsed.Visible = false;

            if (HintUsed)
                for (int i = 0; i < 3; i++)
                    HintPics[i].Dispose();
            if (SelectedTile != null)
                SelectedTile.Dispose();
            HintUsed = false;

            Stopwatch.Reset();
            Stopwatch.Start();

            mBoard.SetColors(IntArray);
            tmrStopwatch.Enabled = true;

            ClickOne = new Point(-1, -1); //set to (-1, -1) if it doesnt hold a proper value
            ClickTwo = new Point(-1, -1);

            CheckForPossibleMoves();
        }

        private void frmMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (GameState == State.Idle)
            {
                if (HintUsed)
                    for (int i = 0; i < 3; i++)
                        HintPics[i].Dispose();

                //if within bounds of grid
                if (e.X > BoardOffset && e.X < TileSize * mBoard.Columns + BoardOffset &&
                    e.Y > BoardOffset && e.X < TileSize * mBoard.Rows + BoardOffset)
                {
                    Point Temp = new Point((e.X - BoardOffset) / TileSize, (e.Y - BoardOffset) / TileSize);

                    if (ClickOne == new Point(-1, -1))
                    {
                        ClickOne = Temp;

                        SelectedTile = new Tile
                        {
                            Size = new Size(60, 60),
                            Image = Sprites.WhiteBorder,
                            BackColor = Color.Transparent
                        };
                        mBoard.Tiles[ClickOne.Y, ClickOne.X].Controls.Add(SelectedTile);
                        SelectedTile.BringToFront();
                    }
                        
                    else
                    {
                        ClickTwo = Temp;
                        SelectedTile.Dispose();

                        //check if adjacent
                        if (Math.Abs(ClickOne.X - ClickTwo.X) == 1 && ClickOne.Y == ClickTwo.Y ||
                            Math.Abs(ClickOne.Y - ClickTwo.Y) == 1 && ClickOne.X == ClickTwo.X)
                        {
                            PreSwap(true);
                        }
                        else
                        {
                            ClickOne = new Point(-1, -1);
                        }
                    }
                }
            }
        }


        public void PreSwap(bool FirstSwap)
        {
            //find difference to move by
            Difference.X = ClickOne.X - ClickTwo.X;
            Difference.Y = ClickOne.Y - ClickTwo.Y;
            if (FirstSwap)
                StartNewAnimation(State.Swapping, TileSize / 2);
            else
                StartNewAnimation(State.SwappingBack, TileSize / 2);
        }

        public void PostSwap(bool FirstSwap)
        {
            //perform swap in actual int array
            int Temp = IntArray[ClickOne.Y, ClickOne.X];
            IntArray[ClickOne.Y, ClickOne.X] = IntArray[ClickTwo.Y, ClickTwo.X];
            IntArray[ClickTwo.Y, ClickTwo.X] = Temp;

            mBoard.Tiles[ClickOne.Y, ClickOne.X].Location = new Point(BoardOffset + ClickOne.X * TileSize, BoardOffset + ClickOne.Y * TileSize);
            mBoard.Tiles[ClickTwo.Y, ClickTwo.X].Location = new Point(BoardOffset + ClickTwo.X * TileSize, BoardOffset + ClickTwo.Y * TileSize);

            mBoard.SetColors(IntArray);
            
            if (FirstSwap)
            {
                //if first swap then check for 3 
                if (CheckForThree(ClickOne.Y, ClickOne.X, false) | CheckForThree(ClickTwo.Y, ClickTwo.X, false))
                {
                    ComboCounter = 0;
                    lblCombo.Text = "1 (1x)";
                    lblComboHeading.Text = "Combo (Point Multiplier):";
                    PreDeleteBoxes();
                }
                else
                    PreSwap(false);
            }
            else
            {
                //if it was a swap back then erase ClickOne and let user click again
                ClickOne = new Point(-1, -1);
                GameState = State.Idle;
            }
        }

    	private void StartNewAnimation(State AnimationType, int Length)
        {
        	AnimationTickCounter = 0;
        	GameState = AnimationType;
        	AnimationLength = Length;
        	tmrAnimation.Start();
        }

        public bool CheckForThreeDirectional(int Row, int Col, bool IsReseting, bool CheckingVertically)
        {
            int ColorToFind = IntArray[Row, Col];

            List<Point> MiniDeleteList = new List<Point>
            {
                new Point(Col, Row)
            };

            if (CheckingVertically)
            {
                //up
                int i = Row - 1;
                while (i >= 0 && IntArray[i, Col] == ColorToFind)
                {
                    MiniDeleteList.Add(new Point(Col, i));
                    i--;
                }

                //down
                i = Row + 1;
                while (i <= 8 && IntArray[i, Col] == ColorToFind)
                {
                    MiniDeleteList.Add(new Point(Col, i));
                    i++;
                }
            }
            else
            {
                //left
                int i = Col - 1;
                while (i >= 0 && IntArray[Row, i] == ColorToFind)
                {
                    MiniDeleteList.Add(new Point(i, Row));
                    i--;
                }

                //right
                i = Col + 1;
                while (i <= 8 && IntArray[Row, i] == ColorToFind)
                {
                    MiniDeleteList.Add(new Point(i, Row));
                    i++;
                }
            }


            if (MiniDeleteList.Count >= 3)
            {
                if (IsReseting == false)
                {
                    GlobalDeleteList.AddRange(MiniDeleteList);

                    foreach (Point ThisPoint in MiniDeleteList)
                        mBoard.Tiles[ThisPoint.Y, ThisPoint.X].PointValue = 10 * MiniDeleteList.Count;
                }

                return true;
            }
            else
                return false;
        }

        public bool CheckForThree(int Row, int Col, bool IsReseting)
        {
            if (CheckForThreeDirectional(Row, Col, IsReseting, true) | CheckForThreeDirectional(Row, Col, IsReseting, false))
                return true;
            else
                return false;
        }

        public void PreDeleteBoxes()
        {
            GlobalDeleteList = GlobalDeleteList.Distinct().ToList();

            UniqueValuesToDelete = new List<int>();
            foreach (Point Point in GlobalDeleteList)
            {
                UniqueValuesToDelete.Add(IntArray[Point.Y, Point.X]);
            }
            UniqueValuesToDelete = UniqueValuesToDelete.Distinct().ToList();

            StartNewAnimation(State.Deleting, 16);
        }

        public void PostDeleteBoxes()
        {
            foreach (Point ThisPoint in GlobalDeleteList)
            {
                IntArray[ThisPoint.Y, ThisPoint.X] = 0;
                //int ThisPointValue = (int)(mBoard.Tiles[ThisPoint.Y, ThisPoint.X].PointValue * Math.Pow(1.5, ComboCounter));
                int ThisPointValue = (int)(mBoard.Tiles[ThisPoint.Y, ThisPoint.X].PointValue * Math.Pow(2, ComboCounter));

                if (HintUsed)
                    ThisPointValue = (int)(ThisPointValue * 0.5);

                //add point value label
                Label lblPointValue = new Label //create label
                {
                    Text = ThisPointValue.ToString(),                                    //assign it ThisPointValue 
                    AutoSize = true,
                    Font = new Font("Microsoft Sans Serif", 12, FontStyle.Regular),
                    ForeColor = Color.White
                };        
                
                if (ThisPointValue < 100)
                    lblPointValue.Size = new Size(30, 20);
                else if (ThisPointValue < 1000)
                    lblPointValue.Size = new Size(40, 20);
                else
                    lblPointValue.Size = new Size(50, 20);
                lblPointValue.Location = new Point(ThisPoint.X * TileSize + BoardOffset + TileSize / 2 - lblPointValue.Width / 2, ThisPoint.Y * TileSize + BoardOffset + 15);  //move the label to boxes that have been deleted

                PointLabelList.Add(lblPointValue);                                                 //add label to global list to dispose of after loop
                Controls.Add(lblPointValue);                                                       //draws label on form
                lblPointValue.BringToFront();                                                      //brings label to front

                Score += ThisPointValue;
                mBoard.Tiles[ThisPoint.Y, ThisPoint.X].PointValue = 0;
            }

            lblScore.Text = Score.ToString();
            mBoard.SetColors(IntArray);

            ColumnsToDrop = new List<int>(GlobalDeleteList.Select(x => x.X).Distinct().ToList());
            GlobalDeleteList = new List<Point>();
            PreDropColumns();
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R)
                mnuGameReset.PerformClick();
            else if (e.KeyCode == Keys.H)
                mnuGameHint.PerformClick();
        }


        private void PreDropColumns()
        {
            StartNewAnimation(State.Dropping, TileSize);
        }

        private void PostDropColumns()
        {
            foreach (Label PointLabel in PointLabelList)
            {
                PointLabel.Dispose();
            }
            PointLabelList = new List<Label>();

            Random r = new Random();
            List<Point> MovedTilesToCheck = new List<Point>();

            for (int i = 0; i < ColumnsToDrop.Count; i++)
            {
                int Column = ColumnsToDrop[i];
                int EmptySpacesInColumn = 0;

                for (int Row = 0; Row < 9; Row++)
                {
                    mBoard.Tiles[Row, Column].Location = new Point(BoardOffset + Column * TileSize, BoardOffset + Row * TileSize);
                    if (IntArray[Row, Column] == 0)
                        EmptySpacesInColumn++;
                }

                for (int Row = 8; Row >= EmptySpacesInColumn; Row--)
                {
                    //todo
                    //if (IntArray[i, Column] == 0)
                    //{
                    //    IntArray[i, Column] = IntArray[i - EmptySpacesInColumn, Column];
                    //    IntArray[i - EmptySpacesInColumn, Column] = 0;
                    //    MovedTilesToCheck.Add(new Point(Column, i));
                    //}

                    if (IntArray[Row, Column] == 0)
                    {
                        for (int j = Row; j >= 0; j--)
                        {
                            if (IntArray[j, Column] != 0)
                            {
                                IntArray[Row, Column] = IntArray[j, Column];
                                IntArray[j, Column] = 0;
                                MovedTilesToCheck.Add(new Point(Column, Row));
                                break;
                            }
                        }
                    }
                }

                for (int Row = 0; Row < 9; Row++)
                {
                    if (IntArray[Row, Column] == 0)
                    {
                        IntArray[Row, Column] = r.Next(1, 7);
                        MovedTilesToCheck.Add(new Point(Column, Row));
                    }    
                } 
            }
            mBoard.SetColors(IntArray);
            CheckNewTiles(MovedTilesToCheck);
            
        }

        private void CheckNewTiles(List<Point> TilesToCheck)
        {
            foreach (Point Tile in TilesToCheck)
            {
                CheckForThree(Tile.Y, Tile.X, false);
            }
            TilesToCheck = new List<Point>();

            //for (int i = TilesToCheck.Count - 1; i >= 0; i--)
            //{
            //    CheckForThree(TilesToCheck[i].Y, TilesToCheck[i].X, false);
            //    TilesToCheck.RemoveAt(i);
            //}

            if (GlobalDeleteList.Count > 0)
            {
                ComboCounter++;
                //lblCombo.Text = (ComboCounter + 1).ToString() + " (" + Math.Round(Math.Pow(1.5,ComboCounter), 2) + "x)";
                lblCombo.Text = (ComboCounter + 1).ToString() + " (" + Math.Pow(2,ComboCounter) + "x)";
                PreDeleteBoxes();
            }
            else
            {
                lblComboHeading.Text = "Previous Combo (Point Multiplier):";
                CheckForPossibleMoves();
                ClickOne = new Point(-1, -1);
                if (HintUsed)
                {
                    HintUsed = false;
                    lblHintUsed.Visible = false;
                }
                GameState = State.Idle;
            }
        }

        private void CheckForPossibleMoves()
        {
            if (HintFound() == false)
                EndGame(false);
        }

        private bool HintFound()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int ColorToCheck = IntArray[i, j];

                    //check vertical 3 in a row
                    //x OR x
                    //x    .
                    //.    x
                    //x    x
                    if (i < 6)
                    {
                        if (IntArray[i + 3, j] == ColorToCheck)
                        {
                            if (IntArray[i + 1, j] == ColorToCheck)
                            {
                                HintPoints[0] = new Point(j, i);
                                HintPoints[1] = new Point(j, i + 1);
                                HintPoints[2] = new Point(j, i + 3);
                                return true;
                            }
                            if (IntArray[i + 2, j] == ColorToCheck)
                            {
                                HintPoints[0] = new Point(j, i);
                                HintPoints[1] = new Point(j, i + 2);
                                HintPoints[2] = new Point(j, i + 3);
                                return true;
                            }
                        }
                    }
                    if (i < 7)
                    {
                        if (j > 0)
                        {
                            //.x OR .x
                            //x.    .x
                            //.x    x
                            if (IntArray[i + 1, j - 1] == ColorToCheck &&
                                IntArray[i + 2, j] == ColorToCheck)
                            {
                                HintPoints[0] = new Point(j, i);
                                HintPoints[1] = new Point(j - 1, i + 1);
                                HintPoints[2] = new Point(j, i + 2);
                                return true;
                            }
                            if (IntArray[i + 1, j] == ColorToCheck &&
                                IntArray[i + 2, j - 1] == ColorToCheck)
                            {
                                HintPoints[0] = new Point(j, i);
                                HintPoints[1] = new Point(j, i + 1);
                                HintPoints[2] = new Point(j - 1, i + 2);
                                return true;
                            }
                        }

                        if (j < 8)
                        {
                            //x. OR x.
                            //.x    x.
                            //x.    .x

                            if (IntArray[i + 1, j + 1] == ColorToCheck &&
                                IntArray[i + 2, j] == ColorToCheck)
                            {
                                HintPoints[0] = new Point(j, i);
                                HintPoints[1] = new Point(j + 1, i + 1);
                                HintPoints[2] = new Point(j, i + 2);
                                return true;
                            }
                            if (IntArray[i + 1, j] == ColorToCheck &&
                                IntArray[i + 2, j + 1] == ColorToCheck)
                            {
                                HintPoints[0] = new Point(j, i);
                                HintPoints[1] = new Point(j, i + 1);
                                HintPoints[2] = new Point(j + 1, i + 2);
                                return true;
                            }
                        }


                        //check horizontal 3 in a row
                        //xx.x
                        //OR
                        //x.xx
                        if (j < 6)
                        {
                            if (IntArray[i, j + 3] == ColorToCheck)
                            {
                                if (IntArray[i, j + 1] == ColorToCheck)
                                {
                                    HintPoints[0] = new Point(j, i);
                                    HintPoints[1] = new Point(j + 1, i);
                                    HintPoints[2] = new Point(j + 3, i);
                                    return true;
                                }
                                if (IntArray[i, j + 2] == ColorToCheck)
                                {
                                    HintPoints[0] = new Point(j, i);
                                    HintPoints[1] = new Point(j + 2, i);
                                    HintPoints[2] = new Point(j + 3, i);
                                    return true;
                                }
                            }
                        }
                        if (j < 7)
                        {
                            if (i > 0)
                            {
                                //.x.
                                //x.x
                                //OR
                                //..x
                                //xx.
                                if (IntArray[i - 1, j + 1] == ColorToCheck &&
                                    IntArray[i, j + 2] == ColorToCheck)
                                {
                                    HintPoints[0] = new Point(j, i);
                                    HintPoints[1] = new Point(j + 1, i - 1);
                                    HintPoints[2] = new Point(j + 2, i);
                                    return true;
                                }
                                if (IntArray[i, j + 1] == ColorToCheck &&
                                    IntArray[i - 1, j + 2] == ColorToCheck)
                                {
                                    HintPoints[0] = new Point(j, i);
                                    HintPoints[1] = new Point(j + 1, i);
                                    HintPoints[2] = new Point(j + 2, i - 1);
                                    return true;
                                }
                            }

                            if (i < 8)
                            {
                                //x.x
                                //.x.
                                //OR
                                //xx.
                                //..x

                                if (IntArray[i + 1, j + 1] == ColorToCheck &&
                                    IntArray[i, j + 2] == ColorToCheck)
                                {
                                    HintPoints[0] = new Point(j, i);
                                    HintPoints[1] = new Point(j + 1, i + 1);
                                    HintPoints[2] = new Point(j + 2, i);
                                    return true;
                                }
                                if (IntArray[i, j + 1] == ColorToCheck &&
                                    IntArray[i + 1, j + 2] == ColorToCheck)
                                {
                                    HintPoints[0] = new Point(j, i);
                                    HintPoints[1] = new Point(j + 1, i);
                                    HintPoints[2] = new Point(j + 2, i + 1);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void tmrStopwatch_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = Stopwatch.Elapsed;
            
            lblTimer.Text = String.Format("{0:00}:{1:00}:{2:00}", (int)ts.Minutes, ts.Seconds, ts.Milliseconds);
            if (ts.Minutes >= TimeLimit) //more than in case something goes wrong
                EndGame(true);
        }

        private void EndGame(bool TimeIsUp)
        {
            tmrStopwatch.Stop();
            tmrAnimation.Stop();
            Stopwatch.Stop();
            TopScoreCheck();

            if (TimeIsUp)
                MessageBox.Show("Time is up! Let's see if you made it to the leaderboard:", "Game Finished");
            else
                MessageBox.Show("No more possible moves left. Let's see if you made it to the leaderboard:", "Game Over");

            //make and show leaderboards form, making this form invisible while the new one is open 
            frmLeaderboards LeaderBoards = new frmLeaderboards();
            LeaderBoards.ShowDialog();
            LeaderBoards.Dispose();
            GameIsOver = true;
            this.Close();
        }

        private Bitmap ChangeOpacity(Image Image, int Alpha)
        {
            Bitmap Original = new Bitmap(Image);
            Bitmap NewImage = new Bitmap(Image.Width, Image.Height);

            Color OriginalColor;
            Color NewColor;

            for (int i = 0; i < Image.Width; i++)
                for (int j = 0; j < Image.Height; j++)
                {
                    OriginalColor = Original.GetPixel(i, j);
                    NewColor = Color.FromArgb(Alpha, OriginalColor.R, OriginalColor.G, OriginalColor.B);
                    NewImage.SetPixel(i, j, NewColor);
                }

            return NewImage;
        }

        private void tmrAnimation_Tick(object sender, EventArgs e)
        {
            //show different animation based on which state the game is in
            if (GameState == State.Swapping || GameState == State.SwappingBack)
            {
                //bring each tile 1px closer to the other one's starting position
                mBoard.Tiles[ClickOne.Y, ClickOne.X].Left -= Difference.X * 2;
                mBoard.Tiles[ClickTwo.Y, ClickTwo.X].Left += Difference.X * 2;
                mBoard.Tiles[ClickOne.Y, ClickOne.X].Top -= Difference.Y * 2;
                mBoard.Tiles[ClickTwo.Y, ClickTwo.X].Top += Difference.Y * 2;
                AnimationTickCounter++;

                //if the animation is done then stop timer and run postswap code
                if (AnimationTickCounter == AnimationLength)
                {
                    tmrAnimation.Stop();
                    if (GameState == State.Swapping)
                        PostSwap(true);
                    else
                        PostSwap(false);
                }
            }
            else if (GameState == State.Deleting)
            {
                Image NewSushi1 = null;
                Image NewSushi2 = null;
                Image NewSushi3 = null;
                Image NewSushi4 = null;
                Image NewSushi5 = null;
                Image NewSushi6 = null;

                int Alpha = (255 - AnimationTickCounter * 16 - 1);

                foreach (int Value in UniqueValuesToDelete)
                {
                    if (Value == 1)
                        NewSushi1 = ChangeOpacity(Sprites.Sushi1, Alpha);
                    else if (Value == 2)
                        NewSushi2 = ChangeOpacity(Sprites.Sushi2, Alpha);
                    else if (Value == 3)
                        NewSushi3 = ChangeOpacity(Sprites.Sushi3, Alpha);
                    else if (Value == 4)
                        NewSushi4 = ChangeOpacity(Sprites.Sushi4, Alpha);
                    else if (Value == 5)
                        NewSushi5 = ChangeOpacity(Sprites.Sushi5, Alpha);
                    else if (Value == 6)
                        NewSushi6 = ChangeOpacity(Sprites.Sushi6, Alpha);
                }

                foreach (Point Point in GlobalDeleteList)
                {
                    if (IntArray[Point.Y, Point.X] == 1)
                        mBoard.Tiles[Point.Y, Point.X].Image = NewSushi1;
                    else if (IntArray[Point.Y, Point.X] == 2)
                        mBoard.Tiles[Point.Y, Point.X].Image = NewSushi2;
                    else if (IntArray[Point.Y, Point.X] == 3)
                        mBoard.Tiles[Point.Y, Point.X].Image = NewSushi3;
                    else if (IntArray[Point.Y, Point.X] == 4)
                        mBoard.Tiles[Point.Y, Point.X].Image = NewSushi4;
                    else if (IntArray[Point.Y, Point.X] == 5)
                        mBoard.Tiles[Point.Y, Point.X].Image = NewSushi5;
                    else if (IntArray[Point.Y, Point.X] == 6)
                        mBoard.Tiles[Point.Y, Point.X].Image = NewSushi6;
                }

                AnimationTickCounter++;

                //if the animation is done then stop timer and run postdelete code
                if (AnimationTickCounter == AnimationLength)
                {
                    tmrAnimation.Stop();
                    PostDeleteBoxes();
                }
            }
            else if (GameState == State.Dropping)
            {
                //for (int i = 0; i < ColumnsToDrop.Count; i++)
                //{
                //    int col = ColumnsToDrop[i];
                //    for (int row = 0; row < 8; row++)
                //    {
                //        if (IntArray[row, col] == 0)
                //            break;
                //        else
                //            mBoard.Tiles[row, col].Top += SpacesToDrop[i];
                //    }
                //}

                for (int i = 0; i < ColumnsToDrop.Count; i++)
                {
                    int EmptySpaceCounter = 0;
                    int col = ColumnsToDrop[i];
                    for (int row = 8; row >= 0; row--)
                    {
                        if (IntArray[row, col] == 0)
                            EmptySpaceCounter++;
                        else
                            mBoard.Tiles[row, col].Top += EmptySpaceCounter;
                    }
                }
                AnimationTickCounter++;

                //if the animation is done then stop timer and run postdrop code
                if (AnimationTickCounter == AnimationLength)
                {
                    tmrAnimation.Stop();
                    PostDropColumns();
                }
            }
					
        }

        private void mnuGameHint_Click(object sender, EventArgs e)
        {
            if (GameState == State.Idle)
            {
                if (HintUsed == false)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        //Show hint
                        Tile HintTile = new Tile
                        {
                            Size = new Size(60, 60),
                            Image = Sprites.GreyBorder,
                            BackColor = Color.Transparent
                        };
                        mBoard.Tiles[HintPoints[i].Y, HintPoints[i].X].Controls.Add(HintTile);
                        HintTile.BringToFront();
                        HintPics[i] = HintTile;
                    }
                    HintUsed = true;
                    lblHintUsed.Visible = true;
                }
            }
            else
                MessageBox.Show("Hints cannot be shown while actions are being taken.");
        }

        private void mnuGameReset_Click(object sender, EventArgs e)
        {
            if (GameState == State.Idle)
                Reset();
            else
                MessageBox.Show("Game cannot be reset while actions are being taken.");
        }

        private void mnuAboutHelp_Click(object sender, EventArgs e)
        {
           
            MessageBox.Show("Objective: \nThe objective of this game is to match 3 or more of the same sushi and get the highest score possible.\n" +
                           "\nControls: \nWhen a possible match of 3 or more is found, click on the two boxes that will make the match possible. If the match is not possible, the boxes will return to their original positions.\n" +
                           "\nCombos: \nWhen boxes begin dropping, if matches are pre-existing, the combo multiplier will give 2x the acquired points. \n" +
                           "\nPoint System: \nEach sushi is worth 10 * amount matched. (i.e. Match of 3 is 10 * 3 since there are 3 sushi's matched totaling up to 90)" +
                           "\n\n Other Controls:\n H - Hint (Combo Multiplier will be reduced to 0.5x for that turn) \n R - Reset Board (All progress will be lost) ");
        }

        private void mnuAboutCredits_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This is a game made by William Tran and Adrian Cerejo over the course of December 2018 to January 2019.\nImages used were taken from the Sushi Go board game by Gamewright.\nThanks for playing!", "Sushi Drop Credits");
        }

        public void TopScoreCheck()
        {
            //variables to keep track of all high 6 scores on leaderboard
            string Score3First, Score3Second, Score3Third, Score10First, Score10Second, Score10Third;

            //Store all current high scores
            using (StreamReader ScoreReader = new StreamReader(HighScoreFile))
            {
                Score3First = ScoreReader.ReadLine();
                Score3Second = ScoreReader.ReadLine();
                Score3Third = ScoreReader.ReadLine();
                Score10First = ScoreReader.ReadLine();
                Score10Second = ScoreReader.ReadLine();
                Score10Third = ScoreReader.ReadLine();
            }

            //if any scores are null, set them to 0
            if (Score3First == null) Score3First = "0";
            if (Score3Second == null) Score3Second = "0";
            if (Score3Third == null) Score3Third = "0";
            if (Score10First == null) Score10First = "0";
            if (Score10Second == null) Score10Second = "0";
            if (Score10Third == null) Score10Third = "0";

            bool UpdateScores = false;

            //if time is 3 minutes
            if (TimeLimit == 3)
            {
                if (Score > int.Parse(Score3Third))
                {
                    if (Score > int.Parse(Score3Second))
                    {
                        Score3Third = Score3Second; //gets moved down

                        if (Score > int.Parse(Score3First)) 
                        {
                            //if score is best
                            Score3Second = Score3First;
                            Score3First = Score.ToString();
                        }
                        else 
                        {
                            //if score is better than second and third
                            Score3Second = Score.ToString();
                        }
                    }
                    else //if score is only better than third
                        Score3Third = Score.ToString();

                    UpdateScores = true;
                }
            }
            else //if time is 10 minutes
            {
                if (Score > int.Parse(Score10Third))
                {
                    if (Score > int.Parse(Score10Second))
                    {
                        Score10Third = Score10Second; //gets moved down

                        if (Score > int.Parse(Score10First))
                        {
                            //if score is best
                            Score10Second = Score10First;
                            Score10First = Score.ToString();
                        }
                        else
                        {
                            //if score is better than second and third
                            Score10Second = Score.ToString();
                        }
                    }
                    else //if score is only better than third
                        Score10Third = Score.ToString();

                    UpdateScores = true;
                }
            }

            if (UpdateScores)
                using (StreamWriter ScoreWriter = new StreamWriter(HighScoreFile))
                {
                    ScoreWriter.WriteLine(Score3First);
                    ScoreWriter.WriteLine(Score3Second);
                    ScoreWriter.WriteLine(Score3Third);
                    ScoreWriter.WriteLine(Score10First);
                    ScoreWriter.WriteLine(Score10Second);
                    ScoreWriter.WriteLine(Score10Third);
                }

        }

        private void mnuGameExit_Click(object sender, EventArgs e)
        {
             this.Close();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (GameIsOver == false && MessageBox.Show("Are you sure you want to exit? All progress will be lost.", "Exit Sushi Drop!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                e.Cancel = true;
        }
    }
}
