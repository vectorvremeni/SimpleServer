using System;
using TheGame;

namespace ConsoleGame
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleRenderer r = new ConsoleRenderer();

            /*
            
            var cls = construict(TypeName)

            cls.call(loadgame)
            
            */

            GoGame g = new GoGame(r);
            g.Init(5, 5);

            g.MoveUser(GoGame.D_RIGHT);
            g.MoveUser(GoGame.D_DOWN);

            String temp = g.RenderField();

            Console.WriteLine(temp);
        }
    }
}
