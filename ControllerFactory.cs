﻿using IoC;
using Server.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	public interface IActionResult
	{
		public String Content { get; set; }
	}

	public class StringResult : IActionResult
	{
		public string Content { get; set; }
	}
	public class ControllerFactory
	{
		IoCContainer services;
        public ControllerFactory(IoCContainer services)
        {
			this.services = services;
        }
		public IActionResult GetResult(String URL)
		{
			HTTPContext ct = HTTPContext.GetContext(URL);

			if (ct != null)
			{
				Assembly asm = Assembly.GetCallingAssembly();

				List<TypeInfo> Controllers = asm.DefinedTypes.Where(x => x.BaseType == typeof(Controller)).ToList();

				AssemblyName an = asm.GetName();

				String ClassName = Controllers.Where(x => x.Name == ct.Controller).SingleOrDefault()?.FullName;

				Type type = asm.GetType(ClassName);

				services.Register(type, type);

				var unwc = services.Create(type);

				((Controller)unwc).Context = ct;

				if (ct.Action != null)
				{
					object[] prms = ct.Params?.ToList().ConvertAll(x => x.Value).ToArray();

					StringResult r = new StringResult { Content = type.GetMethod(ct.Action).Invoke(unwc, prms).ToString() };
					return r;
				}
			}
			return new StringResult {Content = "" };
		}
	}
}
