using GNGame;
using Server.MVC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using TheGame;

namespace ConsoleApp3
{
	class Program
	{
		static MiddlewareContainer App = new MiddlewareContainer();
		public static GoGame game;
		static String SiteFolder = "Files";

		static void Main(string[] args)
		{
			App.Add(x =>
			{
				return new HTTPContext {RawURL=""};
			});

			App.Add(x =>
			{
				return new HTTPContext {RawURL=x.RawURL.TrimStart('/') };
			});

			App.Add(x =>
			{
				return HTTPContext.CreateContext(x.RawURL);
			});

			HTMLRenderer r = new HTMLRenderer();
			
			game = new GoGame(r);

			HttpListener listener = new HttpListener();
			listener.Prefixes.Add("http://*:55555/");
			listener.Start();

			Console.WriteLine("listening");
			game.Init(10, 5);

			while (true)
			{
				HttpListenerContext context = listener.GetContext();
				HttpListenerRequest request = context.Request;

				String rawurl = request.RawUrl;

				App.Init(rawurl);

				String responsestring = "";

				App.Run();

				if (!rawurl.Contains('.'))
				{
					HTTPContext ct = App.GetContext();

					if (ct != null)
					{
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
							responsestring = type.GetMethod(ct.Action).Invoke(unwc, null).ToString();
						}
					}
				}
				else
				{
					game.MoveUser(App.GetContext().RawURL);
					String GameField = game.RenderField();

					String tfile = GetFileContent("Game");

					tfile = tfile.Replace("<game />", GameField);

					responsestring = tfile;
				}

				HttpListenerResponse response = context.Response;

				byte[] buffer = Encoding.UTF8.GetBytes(responsestring);
				response.ContentLength64 = buffer.Length;
				Stream output = response.OutputStream;
				output.Write(buffer, 0, buffer.Length);
				output.Close();

				Console.WriteLine($"String {responsestring} sent to client");
			}
		}

		

		static String GetFileContent(String filename)
		{
			String temp = AssembleFileName(filename);

			if (File.Exists(temp))
			{
				String stringtemp = File.ReadAllText(temp);
				return stringtemp;
			}
			else
			{
				return "";
			}
		}

		
		static String AssembleFileName(String filename)
		{
			String res = Path.Combine( Program.SiteFolder,filename + ".html");
			return res;
		}
	}
	public class MiddlewareContainer
    {
		public List<Func<HTTPContext, HTTPContext>> Handlers = new List<Func<HTTPContext, HTTPContext>>();

		public void Init(String URL)
        {
			this.Context = new HTTPContext {RawURL=URL};
        }

		private HTTPContext Context;

		public void Add(Func<HTTPContext, HTTPContext> f)
        {
			Handlers.Add(f);
        }

		public void Run()
        {
			Handlers.ForEach(x =>
			{
				Context = x(Context);
			});
        }

		public HTTPContext GetContext()
        {
			return Context;
        }
    }
}
