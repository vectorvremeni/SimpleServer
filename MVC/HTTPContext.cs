using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.MVC
{
    public class HTTPContext
    {
        public String Controller { get; set; } = "Home";
        public String Action { get; set; } = "Index";
        public String Params { get; set; }

        public String RawURL = null;

		public static HTTPContext CreateContext(String URL)
		{
			HTTPContext c = new HTTPContext();

			String[] s = URL.Split('/');

			if (s.Length > 0)
			{
				if (!String.IsNullOrWhiteSpace(s[0]))
				{
					c.Controller = s[0];
				}
			}

			if (s.Length > 1)
			{
				if (!String.IsNullOrWhiteSpace(s[1]))
				{
					c.Action = s[1];
				}
			}

			if (s.Length > 2)
			{
				c.Params = s[2];
			}

			c.RawURL = URL;

			return c;
		}
	}
}
