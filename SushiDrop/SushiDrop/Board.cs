using System.Drawing;

//William Tran and Adrian Cerejo
//Board Class
//Dec 6, 2018
//Used to represent the array of tiles/pictureboxes in the game

namespace SushiDrop
{
    class Board
    {
        //fields
        private readonly Tile[,] mTiles; //2d array of tiles that is shown on the main form, parallel to mIntArray
        private readonly int mRows, mColumns, mTileSize;
        private readonly int[,] mIntArray; //2d array of ints, passed by reference from IntArray in frmMain, parallel to mTiles

        //constructor
        public Board(int Rows, int Columns, int TileSize, int BoardOffset, int[,] IntArray)
        {
            //store field values
            this.mRows = Rows;
            this.mColumns = Columns;
            this.mTileSize = TileSize;
            this.mIntArray = IntArray;

            //create 2d array of tiles
            mTiles = new Tile[Rows, Columns];

            //create each tile in array, assign its relevant properties
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
 
                }
            }
        }

        //properties
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

        //methods
        public void SetAllImages()
        {
            //loop through each tile and set its image
            for (int i = 0; i < mRows; i++)
                for (int j = 0; j < mColumns; j++)
                    SetNewImage(i, j);

        }

        public void SetNewImage(int Row, int Column)
        {
            //find image corresponding with parallel value in mIntArray
            Image PicToUse;
            if (mIntArray[Row, Column] == 1)
                PicToUse = Sprites.Sushi1;
            else if (mIntArray[Row, Column] == 2)
                PicToUse = Sprites.Sushi2;
            else if (mIntArray[Row, Column] == 3)
                PicToUse = Sprites.Sushi3;
            else if (mIntArray[Row, Column] == 4)
                PicToUse = Sprites.Sushi4;
            else if (mIntArray[Row, Column] == 5)
                PicToUse = Sprites.Sushi5;
            else if (mIntArray[Row, Column] == 6)
                PicToUse = Sprites.Sushi6;
            else
                PicToUse = Sprites.BLANK_ICON; //defaults to blank

            //set image of tile to found image
            mTiles[Row, Column].Image = PicToUse;
        }
    }
}
