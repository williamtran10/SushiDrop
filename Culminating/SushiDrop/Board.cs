using System.Drawing;

//William Tran and Adrian Cerejo
//Board Class
//Dec 6, 2018
//Used to represent the array of tiles/pictureboxes in the game

//todo: check if get/sets are needed

namespace SushiDrop
{
    class Board
    {
        //fields
        private readonly Tile[,] mTiles; //2d array of tiles that is shown on the main form
        private readonly int mRows, mColumns, mTileSize;
        
        //constructor
        public Board(int Rows, int Columns, int TileSize, int BoardOffset)
        {
            //store field values
            this.mRows = Rows;
            this.mColumns = Columns;
            this.mTileSize = TileSize;

            //create 2d array of tiles
            mTiles = new Tile[Rows, Columns];

            //create each tile in array and assign its location in the grid
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    mTiles[i, j] = new Tile
                    {
                        Size = new Size(TileSize, TileSize),
                        Location = new Point(j * TileSize + BoardOffset, i * TileSize + BoardOffset),
                        BackColor = Color.FromArgb(93, 3, 3)
                    };
                    //mTiles[i, j].BorderStyle = BorderStyle.FixedSingle;
                }
            }
        }
            
        //methods
        public void SetColors(int[,] ValueArray)
        {
            //loop through each tile and set its image to the one coresponding to its parallel value in the int array
            for (int i = 0; i < mRows; i++)
            {
                for (int j = 0; j < mColumns; j++)
                {
                    Image PicToUse;
                    if (ValueArray[i, j] == 1)
                        PicToUse = Sprites.Sushi1;
                    else if (ValueArray[i, j] == 2)
                        PicToUse = Sprites.Sushi2;
                    else if (ValueArray[i, j] == 3)
                        PicToUse = Sprites.Sushi3;
                    else if (ValueArray[i, j] == 4)
                        PicToUse = Sprites.Sushi4;
                    else if (ValueArray[i, j] == 5)
                        PicToUse = Sprites.Sushi5;
                    else if (ValueArray[i, j] == 6)
                        PicToUse = Sprites.Sushi6;
                    else
                        PicToUse = Sprites.BLANK_ICON; //defaults to blank
                    mTiles[i, j].Image = PicToUse;
                }
            }
        }

        public Tile[,] Tiles //get access to the tile array
        {
            get { return mTiles; }
        }

        public int Rows //get access to the row amount
        {
            get { return mRows; }
        }

        public int Columns //get access to the column amount
        {
            get { return mColumns; }
        }
    }
}
