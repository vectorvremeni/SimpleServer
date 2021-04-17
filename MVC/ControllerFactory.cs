using Server.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;

namespace ConsoleApp3
{
    public class ControllerFactory
    {
        public String Execute(HTTPContext ct)
        {
			String res = "";
			Assembly asm = Assembly.GetCallingAssembly();

			List<TypeInfo> Controllers = asm.DefinedTypes.Where(x => x.BaseType == typeof(Controller)).ToList();

			AssemblyName an = asm.GetName();

			String ClassName = Controllers
				.Where(x => x.Name == ct.Controller)
				.SingleOrDefault()?
				.FullName;

			ObjectHandle tclass = Activator.CreateInstance(an.Name, ClassName);

			Type type = asm.GetType(ClassName);

			object unwc = tclass.Unwrap();

			((Controller)unwc).Context = ct;

			if (ct.Action != null)
			{
				res = type.GetMethod(ct.Action).Invoke(unwc, null).ToString();
			}
			return res;
		}
    }
}
