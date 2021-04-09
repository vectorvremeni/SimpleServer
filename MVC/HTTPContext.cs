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
      
        public Dictionary<String,String> Params { get; set; }

        public static HTTPContext GetContext(String URL)
		{
			HTTPContext c = new HTTPContext();

			String[] s = URL.Split('/');

			if (s.Length > 0)
			{
				c.Controller = s[0];
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
				Dictionary<string, string> tobj = new Dictionary<string, string>();
				String [] tp = s[2].Split('&');
				for (int i = 0; i < tp.Length; i++)
				{
					String[] ttp = tp[i].Split('=');
					if (ttp.Length == 2)
					{
						tobj.Add(ttp[0], ttp[1]);
					}
					else
					{
						if (!String.IsNullOrWhiteSpace(ttp[0]))
						{
							tobj.Add("par"+tp.Length, ttp[0]);
						}
					}
				}
				c.Params = tobj;
			}
			return c;
		}
	}
}
