using System;
using System.Collections;
using System.Collections.Generic;
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

            var t = test<int>().GetEnumerator();

            Console.WriteLine(t.MoveAndGet());
            Console.WriteLine(t.MoveAndGet());
            Console.WriteLine(t.MoveAndGet());
        }

        public static IEnumerable test<T>()
        {
            yield return 3;
            yield return 5;
            yield return 8;
        }
    }

    public static class Extenstion
    {
        public static object MoveAndGet(this IEnumerator t)
        {
            t.MoveNext();
            return t.Current;
        }
    }
}
