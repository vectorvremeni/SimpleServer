using System;

namespace TheGame
{
    public interface IRenderer
    {
        public String Render(GameCell[,] GameField,
            GamePoint User,
            int DimX,
            int DimY,
            String CellUser, String CellEmpty);
    }
}
