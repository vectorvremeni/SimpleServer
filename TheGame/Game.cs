using System;
using System.Collections.Generic;
using System.Text;

namespace TheGame
{
    public class Game
    {
        public GameCell[,] GameField;
        public static String CellEmpty = "O";
        public static String CellUser = "X";
        public GamePoint User = new GamePoint(0,0);
        
        public int DimX
        {
            get
            {
                return GameField.GetLength(1);
            }
            private set
            {

            }
        }

        public int DimY
        {
            get
            {
                return GameField.GetLength(0);
            }
            private set
            {

            }
        }


        /// <summary>
        /// fill initial field
        /// </summary>
        /// <param name="x">width</param>
        /// <param name="y">height</param>
        public void Init(int x, int y)
        {
            GameField = new GameCell[y, x];
            for(int yy=0; yy<y; yy++)
            {
                for(int xx=0;xx<x;xx++)
                {
                    GameField[yy, xx] = new GameCell();
                }
            }
        }

        public String RenderField()
        {

            String temp = "";
            for (int yy = 0; yy < DimY; yy++)
            {
                for (int xx = 0; xx < DimX; xx++)
                {
                    if (yy == User.Y && xx == User.X)
                    {
                        temp += CellUser;
                    }
                    else
                    {
                        temp += CellEmpty;
                    }
                }
                temp += Environment.NewLine+"<br />";
            }
            return temp;
        }
        
        public void MoveUser(string directoin)
        {
            if (directoin == "UP" && User.Y != 0)
            {
                User.Y--;
            }
            else if (directoin == "DOWN" && User.Y < DimY - 1)
            {
                User.Y++;
            }
            else if (directoin == "RIGHT" && User.X < DimX -1)
            {
                User.X++;
            }
            else if (directoin == "LEFT" && User.X != 0)
            {
                User.X--;
            }
        }
    }
}
