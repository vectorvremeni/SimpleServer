using System;

namespace TheGame
{
    /// <summary>
    /// интерфейс - это контракт между тем, кто обязался его реализовать, и тем, кто будет вызывать методы того, кто обязался его реализовать
    /// это можно представить себе как соглашение о том, что в любую розетку можно вставить любую вилку
    /// </summary>
    public interface IRenderer
    {
        /// <summary>
        /// нельзя написать тело метода, это просто соглашение для компилятора чтобы он понял правильно ли собраны типы
        /// </summary>
        /// <param name="GameField"></param>
        /// <param name="User"></param>
        /// <param name="DimX"></param>
        /// <param name="DimY"></param>
        /// <param name="CellUser"></param>
        /// <param name="CellEmpty"></param>
        /// <returns></returns>
        public String Render(GameCell[,] GameField,
            GamePoint User,
            int DimX,
            int DimY,
            String CellUser, String CellEmpty);
    }
}
