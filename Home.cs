using Server.MVC;
using System;

namespace Server
{
    public class Home:Controller
    {
        public String Index()
        {
            return "hi from Home:index";
        }

        public String Register()
        {
            return "hi from Home:Register";
        }
    }
}
