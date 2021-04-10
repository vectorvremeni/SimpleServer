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
		
		public static GoGame game;
		static String SiteFolder = "Files";

		static void Main(string[] args)
		{
			
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

				String responsestring = "";

				String TrimmedURL = rawurl.TrimStart('/');

				if (!rawurl.Contains('.'))
				{
					HTTPContext ct = GetContext(TrimmedURL);

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
					
					game.MoveUser(TrimmedURL);
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

		private static HTTPContext GetContext(String URL)
		{
			// создаём наш контекст
			HTTPContext c = new HTTPContext();

			// разбиваем строку на части
			String[] s = URL.Split('/');

			// если там одна часть, то это имя контроллера
			if (s.Length > 0)
			{
				// заполняем его
				c.Controller = s[0];
			}

			// если там две части
			if (s.Length > 1)
			{
				// то заполняем вторую - Action
				c.Action = s[1];
			}

			// если есть ещё и третья, то пользователь передал параметр, кладём его пока просто в одну переменную
			if (s.Length > 2)
			{
				c.Params = s[2];
			}

			// возвращаем готовый заполненный объект HTTPContext
			return c;
		}

		/// <summary>
		/// получаем содержимое файла по имени
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		static String GetFileContent(String filename)
		{
			// собираем имя файла из имени папки где хранятся файлы (чтобы не было бардака) и самого имени
			String temp = AssembleFileName(filename);

			// если такой файл существует
			if (File.Exists(temp))
			{
				// читаем его содержимое и возвращаем
				String stringtemp = File.ReadAllText(temp);
				return stringtemp;
			}
			else
			{
				// если нет (это обычно favicon.ico который запрашивает браузер автоматически, или мы неправильно указали имя файла), то возвращаем пустую строку
				return "";
			}

		}

		/// <summary>
		/// я не хочу думать о том, как собрать из названия папки где файлы и самого файла полный путь, поэтому эта функция будет отвечать за это
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		static String AssembleFileName(String filename)
		{
			// тут немного усложним - есть специальный сборщик, Path
			String res = Path.Combine( Program.SiteFolder,filename + ".html");
			return res;
		}
	}
}
