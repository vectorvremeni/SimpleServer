using Server.MVC;
using System;
using TheGame;

namespace Server.MVC
{
    public class Games:Controller
    {
        public Games(GoGame g)
        {

        }
        public String Index(String t)
        {
            return "Game List" + t;
        }
    }
}
