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
        const string HighScoreFile = "HighScores.txt";
        readonly int TimeLimit; //time limit chosen, is given through the constuctor
        
        //global variables
        Board mBoard; //contains array of tiles/pictureboxes. Images selected are based one IntArray
        int[,] IntArray; //represents the image of each tile on the board as a value from 1-6

        List<Point> GlobalDeleteList;
        List<int> ColumnsToDrop;
        List<int> UniqueValuesToDelete;
        List<Label> PointLabelList;

        Point[] HintPoints = new Point[3];
        Tile[] HintMarkers = new Tile[3];
        bool HintUsed;
        bool GameIsOver = false;
        Point ClickOne, ClickTwo, Difference;
        int Score, AnimationTickCounter, AnimationLength, ComboCounter;
        Tile SelectedTileMarker;

        Stopwatch Stopwatch = new Stopwatch();

        enum State //all possible states of game
        {
            Idle,
            Swapping,
            SwappingBack,
            Deleting,
            Dropping,
        };
        State GameState = State.Idle; //current state of the game



        public frmMain(int GameLength)
        {
            InitializeComponent();

            //initialize main arrays and relevant lists
            IntArray = new int[9, 9];

            mBoard = new Board(9, 9, TileSize, BoardOffset, IntArray);
            PointLabelList = new List<Label>();
            GlobalDeleteList = new List<Point>();

            //Create highscore file
            if (File.Exists(HighScoreFile) == false)
                File.CreateText(HighScoreFile);
                
            //set time limit
            TimeLimit = GameLength;
            
            //add each tile in mBoard to the form
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    this.Controls.Add(mBoard.Tiles[i, j]);
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            //include reset here instead of in contructor so that no match is possible, the board will show first before showing the end game messagebox
            Reset();
        }

        private void Reset()
        {
            //clear entire IntArray
            Array.Clear(IntArray, 0, IntArray.Length);

            //randomize each value in the IntArray
            Random r = new Random();
            int RandomNum = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    do
                    {
                        do
                        {
                            RandomNum = r.Next(1, 7);
                        } while (IntArray[i, j] == RandomNum); //keep randomizing until it is a different number than the one there
                        IntArray[i, j] = RandomNum;
                    } while (CheckForMatch(i, j, true)); //keep randomizing until it doesnt already make a match of 3
                }
            }

            //reset values to initial values
            Score = 0;
            lblScore.Text = "0";
            ComboCounter = 0;
            lblComboHeading.Text = "Combo (Point Multiplier):";
            lblCombo.Text = "0 (0x)";
            lblHintUsed.Visible = false;
            ClickOne = new Point(-1, -1); //set to (-1, -1) to show it has no valid value
            ClickTwo = new Point(-1, -1);

            //dispose of any hintmarkers or selectedtile markers
            if (HintUsed)
                for (int i = 0; i < 3; i++)
                    HintMarkers[i].Dispose();
            if (SelectedTileMarker != null)
                SelectedTileMarker.Dispose();
            HintUsed = false;

            //restart recorded time
            Stopwatch.Reset();
            Stopwatch.Start();
            tmrStopwatch.Enabled = true;

            //set new colors
            mBoard.SetAllImages();
            
            //see if there are any possbile moves + find the next hint
            CheckForPossibleMoves();
        }

        private void frmMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (GameState == State.Idle) //only do this when there are no ongoing animations
            {
                if (HintUsed) //dispose of any hint markers
                    for (int i = 0; i < 3; i++)
                        HintMarkers[i].Dispose();

                //if within bounds of grid...
                if (e.X > BoardOffset && e.X < TileSize * mBoard.Columns + BoardOffset &&
                    e.Y > BoardOffset && e.X < TileSize * mBoard.Rows + BoardOffset)
                {
                    //represents which tile the user clicked on
                    Point Temp = new Point((e.X - BoardOffset) / TileSize, (e.Y - BoardOffset) / TileSize);

                    //if this is the first click
                    if (ClickOne == new Point(-1, -1)) 
                    {
                        //set ClickOne to the temp
                        ClickOne = Temp;

                        //add the SelectedTileMarker to the tile clicked so that it will have an transparent center
                        SelectedTileMarker = new Tile
                        {
                            Size = new Size(60, 60),
                            Image = Sprites.WhiteBorder,
                            BackColor = Color.Transparent
                        };
                        mBoard.Tiles[ClickOne.Y, ClickOne.X].Controls.Add(SelectedTileMarker);
                        SelectedTileMarker.BringToFront();
                    }
                    //if this is the second click    
                    else
                    {
                        //store it as ClickTwo and get rid of the SelectedTileMarker
                        ClickTwo = Temp;
                        SelectedTileMarker.Dispose();

                        //check if adjacent
                        if ((Math.Abs(ClickOne.X - ClickTwo.X) == 1 && ClickOne.Y == ClickTwo.Y) ||
                            (Math.Abs(ClickOne.Y - ClickTwo.Y) == 1 && ClickOne.X == ClickTwo.X))
                        {
                            //swap
                            PreSwap(true);
                        }
                        else
                        {
                            //reset ClickOne so that a new ClickOne can be selected
                            ClickOne = new Point(-1, -1);
                        }
                    }
                }
            }
        }

        private bool CheckForMatch(int Row, int Col, bool IsReseting) //works as a hub the two version of the directional check for three methods
        {
            //return true only if there is a match in either direction
            if (CheckForMatchDirectional(Row, Col, IsReseting, true) | CheckForMatchDirectional(Row, Col, IsReseting, false))
                return true;
            else
                return false;
        }

        private bool CheckForMatchDirectional(int Row, int Col, bool IsReseting, bool IsCheckingVertically) //checks for a match in a specific direction
        {
            //this is the value to look for
            int ValueToFind = IntArray[Row, Col];

            //start the minideletelist with the current point
            List<Point> MiniDeleteList = new List<Point>
            {
                new Point(Col, Row)
            };

            if (IsCheckingVertically) //check vertical directions
            {
                //check up
                int RowToCheck = Row - 1;
                while (RowToCheck >= 0 && IntArray[RowToCheck, Col] == ValueToFind) //if in bounds and is correct value
                {
                    //for each one found add it to the mini delete list and check the next one
                    MiniDeleteList.Add(new Point(Col, RowToCheck));
                    RowToCheck--;
                }

                //check down
                RowToCheck = Row + 1;
                while (RowToCheck <= 8 && IntArray[RowToCheck, Col] == ValueToFind)
                {
                    MiniDeleteList.Add(new Point(Col, RowToCheck));
                    RowToCheck++;
                }
            }
            else //check horizontal directions
            {
                //check left
                int ColToCheck = Col - 1;
                while (ColToCheck >= 0 && IntArray[Row, ColToCheck] == ValueToFind)
                {
                    MiniDeleteList.Add(new Point(ColToCheck, Row));
                    ColToCheck--;
                }

                //check right
                ColToCheck = Col + 1;
                while (ColToCheck <= 8 && IntArray[Row, ColToCheck] == ValueToFind)
                {
                    MiniDeleteList.Add(new Point(ColToCheck, Row));
                    ColToCheck++;
                }
            }

            if (MiniDeleteList.Count >= 3) //if at least 3 were found: return true
            {
                if (IsReseting == false) //if method is called during the game instead of when the game is being reset
                {
                    //add all points to delete to the global list
                    GlobalDeleteList.AddRange(MiniDeleteList);

                    //calculate the base value of each tile and assign it for use later
                    foreach (Point ThisPoint in MiniDeleteList)
                        mBoard.Tiles[ThisPoint.Y, ThisPoint.X].PointValue = 10 * MiniDeleteList.Count;
                }

                return true;
            }
            else
                return false;
        }

        private void PreSwap(bool IsFirstSwap) //happens before swap animation
        {
            //find difference between the two clicks' indexes: this will be how much they move by
            Difference.X = ClickOne.X - ClickTwo.X;
            Difference.Y = ClickOne.Y - ClickTwo.Y;
            
            //run the proper animation based on if this is the first swap or the swap back
            if (IsFirstSwap)
                StartNewAnimation(State.Swapping, TileSize / 2);
            else
                StartNewAnimation(State.SwappingBack, TileSize / 2);
        }

        private void PostSwap(bool FirstSwap) //happens after swap animation
        {
            //perform swap in actual int array
            int Temp = IntArray[ClickOne.Y, ClickOne.X];
            IntArray[ClickOne.Y, ClickOne.X] = IntArray[ClickTwo.Y, ClickTwo.X];
            IntArray[ClickTwo.Y, ClickTwo.X] = Temp;

            //reset the locations of the swapped tiles and reassign their images so they appear to have swapped places
            mBoard.Tiles[ClickOne.Y, ClickOne.X].Location = new Point(BoardOffset + ClickOne.X * TileSize, BoardOffset + ClickOne.Y * TileSize);
            mBoard.Tiles[ClickTwo.Y, ClickTwo.X].Location = new Point(BoardOffset + ClickTwo.X * TileSize, BoardOffset + ClickTwo.Y * TileSize);
            mBoard.SetNewImage(ClickOne.Y, ClickOne.X);
            mBoard.SetNewImage(ClickTwo.Y, ClickTwo.X);

            if (FirstSwap)
            {
                //if first swap then check for 3 in both clicks
                if (CheckForMatch(ClickOne.Y, ClickOne.X, false) | CheckForMatch(ClickTwo.Y, ClickTwo.X, false))
                {
                    //if match is found then set combo labels and start deleting boxes
                    ComboCounter = 1;
                    lblCombo.Text = "1 (1x)";
                    lblComboHeading.Text = "Combo (Point Multiplier):";
                    PreDeleteTiles();
                }
                else
                    PreSwap(false); //swap it back
            }
            else
            {
                //if it was a swap back then erase ClickOne and let user click again
                ClickOne = new Point(-1, -1);
                GameState = State.Idle;
            }
        }

        private void PreDeleteTiles() //happens before delete animation
        {
            //keep only unique points of list
            GlobalDeleteList = GlobalDeleteList.Distinct().ToList();

            //create a new list of values in IntArray that will be deleted
            //this is used to detemine which pictures are required to change the opacity of while doing the delete animation
            UniqueValuesToDelete = new List<int>();
            foreach (Point Point in GlobalDeleteList)
                UniqueValuesToDelete.Add(IntArray[Point.Y, Point.X]);
            UniqueValuesToDelete = UniqueValuesToDelete.Distinct().ToList();

            //starts the animation. length of 16 is chosen because on each timer tick, the opacity goes down by 16
            StartNewAnimation(State.Deleting, 16);
        } 

        private void PostDeleteTiles() //happens after delete animation
        {
            //loop through each point to delete
            foreach (Point ThisPoint in GlobalDeleteList)
            {
                IntArray[ThisPoint.Y, ThisPoint.X] = 0; //show empty spaces using 0

                //calculate final point value using base point value * combo multiplier * hint multiplier
                int ThisPointValue = (int)(mBoard.Tiles[ThisPoint.Y, ThisPoint.X].PointValue * Math.Pow(2, ComboCounter - 1));
                if (HintUsed)
                    ThisPointValue = (int)(ThisPointValue * 0.5);

                //add label to display the point value and initalize its properties
                Label lblPointValue = new Label
                {
                    Text = ThisPointValue.ToString(),                                    
                    AutoSize = true,
                    Font = new Font("Microsoft Sans Serif", 12, FontStyle.Regular),
                    ForeColor = Color.White
                };        
                //resize label's width (prior to autosize) to use in centering calulations
                if (ThisPointValue < 100)
                    lblPointValue.Size = new Size(30, 20);
                else if (ThisPointValue < 1000)
                    lblPointValue.Size = new Size(40, 20);
                else
                    lblPointValue.Size = new Size(50, 20);
                //center the label in the middle of the tile that was just deleted
                lblPointValue.Location = new Point(ThisPoint.X * TileSize + BoardOffset + TileSize / 2 - lblPointValue.Width / 2, ThisPoint.Y * TileSize + BoardOffset + 15);

                PointLabelList.Add(lblPointValue);       //add label to list so it can be easily disposed later
                Controls.Add(lblPointValue);             //draws label on form
                lblPointValue.BringToFront();            //brings label to front

                //adds the PointValue to the total score
                Score += ThisPointValue;                       
                //reset tile's point value
                mBoard.Tiles[ThisPoint.Y, ThisPoint.X].PointValue = 0;
                //set its new image (Sprite.BLANK_ICON)
                mBoard.SetNewImage(ThisPoint.Y, ThisPoint.X);
            }

            //make total score show onscreen
            lblScore.Text = Score.ToString();

            ColumnsToDrop = new List<int>(GlobalDeleteList.Select(x => x.X).Distinct().ToList()); //keeps trach of which columns need to be dropped in dropping animation
            GlobalDeleteList = new List<Point>(); //reset to use next time
            PreDropColumns(); //drop the columns
        } 

        private void PreDropColumns() //happens before drop animation
        {
            //run drop animation
            StartNewAnimation(State.Dropping, TileSize);
        } 

        private void PostDropColumns() //happens after drop animation
        {
            //dispose every pointlabel and remake the list
            foreach (Label PointLabel in PointLabelList)
                PointLabel.Dispose();
            PointLabelList = new List<Label>();

            Random r = new Random();
            List<Point> MovedTilesToCheck = new List<Point>(); //tiles to check for new matches

            foreach (int Col in ColumnsToDrop)
            {
                int EmptySpacesInColumn = 0;

                //reset the location of each tile in the columns and count number of empty spaces
                for (int Row = 0; Row < 9; Row++)
                {
                    mBoard.Tiles[Row, Col].Location = new Point(BoardOffset + Col * TileSize, BoardOffset + Row * TileSize);
                    if (IntArray[Row, Col] == 0)
                        EmptySpacesInColumn++;
                }

                //iterating from bottom to top
                for (int Row = 8; Row >= 0; Row--)
                {
                    if (Row >= EmptySpacesInColumn) //for each row in the column, except for the ones that will be empty spaces at the top
                    {
                        if (IntArray[Row, Col] == 0) //if it is blank: find the closest valid value above it, copy that value down, and turn the original value to 0 to represent how the tile moved down
                        {
                            for (int UpperRow = Row - 1; UpperRow >= 0; UpperRow--)
                            {
                                if (IntArray[UpperRow, Col] != 0)
                                {
                                    IntArray[Row, Col] = IntArray[UpperRow, Col];
                                    IntArray[UpperRow, Col] = 0;
                                    //update its image and check it for a match later
                                    mBoard.SetNewImage(Row, Col);
                                    MovedTilesToCheck.Add(new Point(Col, Row));
                                    break;
                                }
                            }
                        }
                    }
                    else      
                    {
                        //for each empty space in the column, which would have bubbled to the top, assign it a new random value and check it for a match later
                        IntArray[Row, Col] = r.Next(1, 7);
                        MovedTilesToCheck.Add(new Point(Col, Row));
                        mBoard.SetNewImage(Row, Col);
                    }
                }
            }
            CheckNewTiles(MovedTilesToCheck);
        }

        private void CheckNewTiles(List<Point> TilesToCheck) //check the tiles that were moved for any new matches
        {
            //check for a match using each tile in list
            foreach (Point Tile in TilesToCheck)
                CheckForMatch(Tile.Y, Tile.X, false);
            //clear list
            TilesToCheck = new List<Point>();

            if (GlobalDeleteList.Count > 0)
            {
                //if a match is found then increment the combo and delete the new tiles (which will run the whole cycle of animations again)
                ComboCounter++;
                lblCombo.Text = (ComboCounter).ToString() + " (" + Math.Pow(2,ComboCounter - 1) + "x)";
                PreDeleteTiles();
            }
            else
            {
                //if there are no further matches found then cahnge combo label and check for possible moves
                lblComboHeading.Text = "Previous Combo (Point Multiplier):";
                CheckForPossibleMoves();
                //reset hint usage
                if (HintUsed)
                {
                    HintUsed = false;
                    lblHintUsed.Visible = false;
                }
                //clear ClickOne and allow the user to click again
                ClickOne = new Point(-1, -1);
                GameState = State.Idle;
            }
        }

        private void CheckForPossibleMoves()
        {
            //if no possible move/hint is found then end the game
            if (HintFound() == false)
                EndGame(false); //bool here modifies text in game end messagebox
        }

        private bool HintFound() //finds a single possible move
        {
            //for each tile...
            for (int Row = 0; Row < 8; Row++)
            {
                for (int Col = 0; Col < 8; Col++)
                {
                    int ColorToCheck = IntArray[Row, Col]; //this is the color to look for

                    //
                    //try to use each possible pattern for a match of 3 on IntArray[i,j]
                    //in each diagram: # is [i,j], 
                    //                 x is a value matching [i,j]
                    //                 . is any other value
                    //if a match is found then add all of its points to array of hind points and return true
                    //if no match is found then procede to the next index
                    //
                    
                    //check vertical 3 in a row
                    if (Row < 6) //only check a pattern if it would lie within the bounds of the array
                    {
                        if (IntArray[Row + 3, Col] == ColorToCheck)
                        {
                            //# OR #
                            //x    .
                            //.    x
                            //x    x
                            if (IntArray[Row + 1, Col] == ColorToCheck)
                            {
                                HintPoints[0] = new Point(Col, Row);
                                HintPoints[1] = new Point(Col, Row + 1);
                                HintPoints[2] = new Point(Col, Row + 3);
                                return true;
                            }
                            if (IntArray[Row + 2, Col] == ColorToCheck)
                            {
                                HintPoints[0] = new Point(Col, Row);
                                HintPoints[1] = new Point(Col, Row + 2);
                                HintPoints[2] = new Point(Col, Row + 3);
                                return true;
                            }
                        }
                    }
                    if (Row < 7)
                    {
                        if (Col > 0)
                        {
                            //.# OR .#
                            //x.    .x
                            //.x    x
                            if (IntArray[Row + 1, Col - 1] == ColorToCheck &&
                                IntArray[Row + 2, Col] == ColorToCheck)
                            {
                                HintPoints[0] = new Point(Col, Row);
                                HintPoints[1] = new Point(Col - 1, Row + 1);
                                HintPoints[2] = new Point(Col, Row + 2);
                                return true;
                            }
                            if (IntArray[Row + 1, Col] == ColorToCheck &&
                                IntArray[Row + 2, Col - 1] == ColorToCheck)
                            {
                                HintPoints[0] = new Point(Col, Row);
                                HintPoints[1] = new Point(Col, Row + 1);
                                HintPoints[2] = new Point(Col - 1, Row + 2);
                                return true;
                            }
                        }
                        if (Col < 8)
                        {
                            //#. OR #.
                            //.x    x.
                            //x.    .x

                            if (IntArray[Row + 1, Col + 1] == ColorToCheck &&
                                IntArray[Row + 2, Col] == ColorToCheck)
                            {
                                HintPoints[0] = new Point(Col, Row);
                                HintPoints[1] = new Point(Col + 1, Row + 1);
                                HintPoints[2] = new Point(Col, Row + 2);
                                return true;
                            }
                            if (IntArray[Row + 1, Col] == ColorToCheck &&
                                IntArray[Row + 2, Col + 1] == ColorToCheck)
                            {
                                HintPoints[0] = new Point(Col, Row);
                                HintPoints[1] = new Point(Col, Row + 1);
                                HintPoints[2] = new Point(Col + 1, Row + 2);
                                return true;
                            }
                        }

                        //check horizontal 3 in a row
                        if (Col < 6)
                        {
                            if (IntArray[Row, Col + 3] == ColorToCheck)
                            {
                                //#x.x
                                //OR
                                //#.xx
                                if (IntArray[Row, Col + 1] == ColorToCheck)
                                {
                                    HintPoints[0] = new Point(Col, Row);
                                    HintPoints[1] = new Point(Col + 1, Row);
                                    HintPoints[2] = new Point(Col + 3, Row);
                                    return true;
                                }
                                if (IntArray[Row, Col + 2] == ColorToCheck)
                                {
                                    HintPoints[0] = new Point(Col, Row);
                                    HintPoints[1] = new Point(Col + 2, Row);
                                    HintPoints[2] = new Point(Col + 3, Row);
                                    return true;
                                }
                            }
                        }
                        if (Col < 7)
                        {
                            if (Row > 0)
                            {
                                //.x.
                                //#.x
                                //OR
                                //..x
                                //#x.
                                if (IntArray[Row - 1, Col + 1] == ColorToCheck &&
                                    IntArray[Row, Col + 2] == ColorToCheck)
                                {
                                    HintPoints[0] = new Point(Col, Row);
                                    HintPoints[1] = new Point(Col + 1, Row - 1);
                                    HintPoints[2] = new Point(Col + 2, Row);
                                    return true;
                                }
                                if (IntArray[Row, Col + 1] == ColorToCheck &&
                                    IntArray[Row - 1, Col + 2] == ColorToCheck)
                                {
                                    HintPoints[0] = new Point(Col, Row);
                                    HintPoints[1] = new Point(Col + 1, Row);
                                    HintPoints[2] = new Point(Col + 2, Row - 1);
                                    return true;
                                }
                            }
                            if (Row < 8)
                            {
                                //#.x
                                //.x.
                                //OR
                                //#x.
                                //..x

                                if (IntArray[Row + 1, Col + 1] == ColorToCheck &&
                                    IntArray[Row, Col + 2] == ColorToCheck)
                                {
                                    HintPoints[0] = new Point(Col, Row);
                                    HintPoints[1] = new Point(Col + 1, Row + 1);
                                    HintPoints[2] = new Point(Col + 2, Row);
                                    return true;
                                }
                                if (IntArray[Row, Col + 1] == ColorToCheck &&
                                    IntArray[Row + 1, Col + 2] == ColorToCheck)
                                {
                                    HintPoints[0] = new Point(Col, Row);
                                    HintPoints[1] = new Point(Col + 1, Row);
                                    HintPoints[2] = new Point(Col + 2, Row + 1);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false; //only happens if there are no possible matches across the entire board
        }

        private Bitmap ChangeOpacity(Image Image, int Alpha) //cahnges image's opacity, used for deleting animation
        {
            //variables to store images and colors
            Bitmap Original = new Bitmap(Image);
            Bitmap NewImage = new Bitmap(Image.Width, Image.Height);
            Color OriginalColor;
            Color NewColor;

            //loop though each pixel
            for (int i = 0; i < Image.Width; i++)
                for (int j = 0; j < Image.Height; j++)
                {
                    //get original color values, add in alpha value, set pixel to the new image
                    OriginalColor = Original.GetPixel(i, j);
                    NewColor = Color.FromArgb(Alpha, OriginalColor.R, OriginalColor.G, OriginalColor.B);
                    NewImage.SetPixel(i, j, NewColor);
                }

            //return image with new alpha value
            return NewImage;
        }

        private void StartNewAnimation(State AnimationType, int Length)
        {
            //reset counter and set variables that control the animation
        	AnimationTickCounter = 0;
        	GameState = AnimationType;
        	AnimationLength = Length;

            //start the animations
        	tmrAnimation.Start();
        }

        private void tmrAnimation_Tick(object sender, EventArgs e)
        {
            //show different animation based on which state the game is in
            if (GameState == State.Swapping || GameState == State.SwappingBack)
            {
                //bring each tile 2px closer to the other one's starting position
                mBoard.Tiles[ClickOne.Y, ClickOne.X].Left -= Difference.X * 2;
                mBoard.Tiles[ClickTwo.Y, ClickTwo.X].Left += Difference.X * 2;
                mBoard.Tiles[ClickOne.Y, ClickOne.X].Top -= Difference.Y * 2;
                mBoard.Tiles[ClickTwo.Y, ClickTwo.X].Top += Difference.Y * 2;
                AnimationTickCounter++;

                //if the animation is done then stop timer and run respective postswap code
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
                //all possible new images
                Image NewSushi1 = null;
                Image NewSushi2 = null;
                Image NewSushi3 = null;
                Image NewSushi4 = null;
                Image NewSushi5 = null;
                Image NewSushi6 = null;

                //alpha is based on animation tick counter, decements by 16 each tick
                int Alpha = (255 - AnimationTickCounter * 16 - 1);

                //only create new images for the values that will be used
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
                //assign new images to each tile to be deleted
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
                    PostDeleteTiles();
                }
            }
            else if (GameState == State.Dropping)
            {
                //in each column...
                foreach (int Col in ColumnsToDrop)
                {
                    //if the block was not deleted then move it down by the amount of empty spaces under it
                    //this way, all columns will settle at the same time
                    int EmptySpaceCounter = 0;
                    for (int Row = 8; Row >= 0; Row--)
                    {
                        if (IntArray[Row, Col] == 0)
                            EmptySpaceCounter++;
                        else
                            mBoard.Tiles[Row, Col].Top += EmptySpaceCounter;
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

        private void tmrStopwatch_Tick(object sender, EventArgs e)
        {
            //display the time elapsed taken from stopwatch
            TimeSpan ts = Stopwatch.Elapsed;
            lblTimer.Text = String.Format("{0:00}:{1:00}:{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds);
            if (ts.Minutes >= TimeLimit) //if the time is over the limit then end game
                EndGame(true);
        }

        private void EndGame(bool TimeIsUp)
        {
            //stop all timers/stopwatches
            tmrStopwatch.Stop();
            tmrAnimation.Stop();
            Stopwatch.Stop();

            //check to see if this score made the leaderboards
            TopScoreCheck();

            //show message saying game is done
            if (TimeIsUp)
                MessageBox.Show("Time is up! Let's see if you made it to the leaderboard:", "Game Finished");
            else
                MessageBox.Show("No more possible moves left. Let's see if you made it to the leaderboard:", "Game Over");

            //make and show leaderboards form, making this form invisible while the new one is open 
            frmLeaderboards LeaderBoards = new frmLeaderboards();
            LeaderBoards.ShowDialog();
            LeaderBoards.Dispose();
            GameIsOver = true; //prevents "Are you sure you want to quit" dialog from showing
            this.Close();
        }

        private void TopScoreCheck()
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

            bool UpdateScores = false; //keeps trackif scores need to be updated

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

                    UpdateScores = true; //update for all cases above
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

                    UpdateScores = true; //update for all cases above
                }
            }

            if (UpdateScores) //update all scores including the one with the new value
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

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            //hotkeys/shortcuts
            if (e.KeyCode == Keys.R)
                mnuGameReset.PerformClick();
            else if (e.KeyCode == Keys.H)
                mnuGameHint.PerformClick();
        }

        private void mnuGameHint_Click(object sender, EventArgs e)
        {
            if (GameState == State.Idle && HintUsed == false) //if no animations are playing and a hint hasnt been used yet
            {
                for (int i = 0; i < 3; i++) //for 3 spaces in hintmarker array
                {
                    //spawn new hint marker
                    Tile HintTile = new Tile
                    {
                        Size = new Size(60, 60),
                        Image = Sprites.GreyBorder,
                        BackColor = Color.Transparent
                    };
                    //add hintmarker to tile under it to make it transparent and bring it to front
                    mBoard.Tiles[HintPoints[i].Y, HintPoints[i].X].Controls.Add(HintTile);
                    HintTile.BringToFront();
                    //add it to the array to it can be easily deleted later
                    HintMarkers[i] = HintTile;
                }
                //record that a hint was used
                HintUsed = true;
                lblHintUsed.Visible = true;
            }
            else //show message explaining why hint cant be used
                MessageBox.Show("Hints cannot be shown while actions are being taken or if another hint is already showing.");
        }

        private void mnuGameReset_Click(object sender, EventArgs e)
        {
            //only allow resets when no animation is ongoing
            if (GameState == State.Idle)
                Reset();
            else
                MessageBox.Show("Game cannot be reset while actions are being taken.");
        }

        private void mnuAboutHelp_Click(object sender, EventArgs e)
        {
           MessageBox.Show("Objective: \nThe objective of this game is to match 3 or more of the same sushi to get the highest score possible.\n" +
                           "\nControls: \nWhen a possible match of 3 or more is found, click on the one sushi to select it, then click on an adjacent sushi to perform a swap and check for a possible match. If the match is not found, the boxes will return to their original positions.\n" +
                           "\nCombos: \nWhen boxes have settled after dropping, if another match is found then the combo counter will increase. Each increase in the combo counter will multiply the score value of all subsequent matches by 2. Therefore, the combo increases the score value exponentially. The combo counter resets after no match is found.\n" +
                           "\nPoint System: \nEach match is worth more based on how many sushis are in it. A match of 3 has a base point value 30 each, a match of 4 has a base point value of 40 each, and so on." +
                           "\n\n Other Controls:\n H - Hint (Combo Multiplier will be reduced to 0.5x for that turn) \n R - Reset Board (All progress will be lost)");
        }

        private void mnuAboutCredits_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This is a game made by William Tran and Adrian Cerejo over the course of December 2018 to January 2019.\nImages used were taken from the Sushi Go board game by Gamewright.\nThanks for playing!", "Sushi Drop Credits");
        }

        private void mnuGameExit_Click(object sender, EventArgs e)
        {
             this.Close();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //on closing if the game isnt already finished ask "Are you sure?"
            if (GameIsOver == false && MessageBox.Show("Are you sure you want to exit? All progress will be lost.", "Exit Sushi Drop!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                e.Cancel = true;
        }


    }
}
