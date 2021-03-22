using System;
using System.Collections.Generic;
using System.Text;

namespace TheGame
{
    public class GoGame
    {
        public GameCell[,] GameField;
        public static String CellEmpty = "O";
        public static String CellUser = "X";
        public GamePoint User = new GamePoint(0, 0);

        public static String D_UP = "UP";
        public static String D_DOWN = "DOWN";
        public static String D_LEFT = "LEFT";
        public static String D_RIGHT = "RIGHT";

        public IRenderer _r;

        /// <summary>
        /// принуждаем игру принять в конструкторе зависимость - конкретрный рендерер. этим мы обезопасиваем себя от неправильного использования нашего класса другими программистами.
        /// если они захотят создать наш тип, не заполня его рендерер у них не получится
        /// в то же время никто им не мешает сделать свой рендерер, который будет работать так как они хотят, для этого им нужно будет реализовать интерфейс
        /// </summary>
        /// <param name="r"></param>
        public GoGame(IRenderer r)
        {
            _r = r;
        }

        /// <summary>
        /// Property - геттер/сеттер, выглядит как переменная - работает как функция
        /// </summary>
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

        /// <summary>
        /// я не хочу помнить о размерностях массива, поэтому просто оборачиваю это в функциональность
        /// </summary>
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
        /// при старте игры заполняем поле объектами GameCell
        /// </summary>
        /// <param name="x">width</param>
        /// <param name="y">height</param>
        public void Init(int x, int y)
        {
            GameField = new GameCell[y, x];
            for (int yy = 0; yy < y; yy++)
            {
                for (int xx = 0; xx < x; xx++)
                {
                    GameField[yy, xx] = new GameCell();
                }
            }
        }

        /// <summary>
        /// ну рендерим теперь... почему так просто? за всё отвечает рендерер, мы просто возвращаем то что он нарендерил
        /// </summary>
        /// <returns></returns>
        public String RenderField()
        {
            String temp = _r.Render(GameField, User, DimX, DimY, CellUser, CellEmpty);
            return temp;
        }

        /// <summary>
        /// перемещаем пользователя, меняя его координаты
        /// </summary>
        /// <param name="directoin"></param>
        public void MoveUser(string directoin)
        {
            if (directoin == D_UP && User.Y != 0)
            {
                User.Y--;
            }
            else if (directoin == D_DOWN && User.Y < DimY - 1)
            {
                User.Y++;
            }
            else if (directoin == D_RIGHT && User.X < DimX - 1)
            {
                User.X++;
            }
            else if (directoin == D_LEFT && User.X != 0)
            {
                User.X--;
            }
        }
    }

    /// <summary>
    /// вот например - этот рендерер хочет встроиться в конструктор класса игры. там написано, что она принимает только тех, кто реализует интерфейс IRenderer (тоесть имеет функцию Render)
    /// как она устроена пока не важно, потому что она должна вернуть String - пусть делает что хочет, главное что сигнатура совпадает и её можно вызывать
    /// мы говорим ладно, давайте реализовывать этот интерфейс - пишем эту функцию, она смотрит где юзер и если он там, возвращает то что ему передали как юзер, если нет то то, что считается пустой клеткой, возвращает отрендеренное поле
    /// </summary>
    public class HTMLRenderer:IRenderer
    {
        public String Render(GameCell[,] GameField,
            GamePoint User,
            int DimX,
            int DimY,
            String CellUser, String CellEmpty)
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
                temp += Environment.NewLine + "<br />";
            }
            return temp;
        }
    }


    /// <summary>
    /// а вот другой рендерер, который работает точно так же, но возвращает уже версию для консоли - без <br/>
    /// </summary>
    public class ConsoleRenderer:IRenderer
    {
        public String Render(GameCell[,] GameField,
            GamePoint User,
            int DimX,
            int DimY,
            String CellUser, String CellEmpty)
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
                temp += Environment.NewLine;
            }
            return temp;
        }
    }
}
