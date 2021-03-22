using Server.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.MVC
{
    public class GuessGame:Controller
    {
        public String Index()
        {
            return "Hi from GuessGame:Index";
        }

        public String Play()
        {
            return "Hi from GuessGame:Play";
        }
    }
}
