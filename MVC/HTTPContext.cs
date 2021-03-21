using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.MVC
{
    public class HTTPContext
    {
        public String Controller { get; set; }
        public String Action { get; set; } = "Index";
        public String Params { get; set; }
    }
}
