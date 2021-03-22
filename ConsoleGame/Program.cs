using System;
using TheGame;

namespace ConsoleGame
{
    class Program
    {
        static void Main(string[] args)
        {
            // мы тут тестируем наш рендерер - он рендерит не в HTML а в консоль, тоесть там просто нет <br />
            ConsoleRenderer r = new ConsoleRenderer();

            GoGame g = new GoGame(r);
            g.Init(5, 5);

            g.MoveUser(GoGame.D_RIGHT);
            g.MoveUser(GoGame.D_DOWN);

            String temp = g.RenderField();

            Console.WriteLine(temp);
        }
    }
}
