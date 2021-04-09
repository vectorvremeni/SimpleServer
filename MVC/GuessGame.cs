using GNGame;
using Server.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.MVC
{
    public class GG:Controller
    {
        GuessGame g;
        public GG(GuessGame gg)
        {
            g = gg;
        }

        public String Init()
        {
            g.init(5);
            return "game init complete";
        }
        public String Guess(String num)
        {
            String res = g.guess(int.Parse(num));
            return res;
        }

        public String Play()
        {
            return "Hi from GuessGame:Play";
        }
    }
}
