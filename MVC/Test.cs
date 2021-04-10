using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.MVC
{
    public class Test:Controller
    {
        public String Index()
        {
            return "Hi frfom Test:Index";
        }

        public String Test2()
        {
            return "Hi frfom Test2:TEST!!!!!!!!!!!";
        }
    }
}
