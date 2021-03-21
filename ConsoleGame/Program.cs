using System;
using TheGame;

namespace ConsoleGame
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleRenderer r = new ConsoleRenderer();

            Game g = new Game(r);
            g.Init(5, 5);

            g.MoveUser("RIGHT");
            g.MoveUser("RIGHT");

            String temp = g.RenderField();

            Console.WriteLine(temp);
        }
    }
}
