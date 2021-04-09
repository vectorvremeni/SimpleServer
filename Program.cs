using GNGame;
using IoC;
using Server;
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
		/// <summary>
		/// наша игра ходилка
		/// </summary>
		static String SiteFolder = "Files";
		static MiddlewareContainer App = new MiddlewareContainer();

		static void Main(string[] args)
		{
			var container = new IoCContainer();
			ControllerFactory factory = new ControllerFactory(container);

			container.Register<IRenderer, HTMLRenderer>();
			container.RegisterSingleton<GoGame, GoGame>();
			container.RegisterSingleton<GuessGame, GuessGame>();

			GoGame game = container.Create<GoGame>();
			game.Init(5, 5);

			App.Add(x =>
			{
				if (x.RawURL.Contains("favicon.ico"))
				{
					x.RawURL = "";
				}
				return x;
			});

			App.Add(x =>
			{
				x.RawURL = x.RawURL.TrimStart('/');
				return x;
			});

			App.Add(x => 
			{
				return HTTPContext.GetContext(x.RawURL);
			});

			// создаём HTML версию рендерера
			HTMLRenderer r = new HTMLRenderer();
			// создаём объект игры и пробрасываем ему рендерер (внедрение через конструктор)
			//game = new GoGame(r);

			// создаём слушатель HTTP запросов
			HttpListener listener = new HttpListener();
			// настраиваем его чтобы он слушал на определённом порту
			listener.Prefixes.Add("http://*:55555/");
			// стартуем его
			listener.Start();

			// на всякий случай выводим в консоль чтобы мы знали что сервер слушает
			Console.WriteLine("listening");

			// инициализируем игру (ToDo: потом нужно это убрать в контроллер)
			//game.Init(10, 5);

			// создаём бесконечный цикл чтобы слушать запросы с клиента
			while (true)
			{
				// контекст - это кто, что спросил, с какого IP, какой браузер итд - сопутствующие запросу данные
				// чтобы мы не парились с тем как нам возвращать данные на клиент по этому запросу, уже всё сделано за нас:
				// поскольку контекст содержит запрос (Request) и ответ (Response) мы можем просто положить чтото в response и это пойдёт на клиент

				// получаем контекст из слушателя - это происходит при любом запросе с клиента
				// (всмысле в этой точке программа ждёт запроса, цикл просто так не гоняется без дела)
				// когда запрос получен, выполняется эта строка: получаем контекст из слушателя
				HttpListenerContext context = listener.GetContext();
				// получаем запрос из контекста
				HttpListenerRequest request = context.Request;

				// чтобы не бегать по сложному объекту контекст, искать там запрос, доставать оттуда URL который пользователь ввёл в браузер, просто извлекаем его в переменную
				String rawurl = request.RawUrl;

				// сюда мы будем класть ответ сервера, неважно какая подсистема его сформирует
				String responsestring = "";

				// убираем / вначале, чтобы переиспользовать эту строку в нескольких местах


				App.Init(new HTTPContext {RawURL = rawurl});
				App.Run();

				HTTPContext ct = App.GetContext();


				// дальше идёт ветвление на MVC и просто файлы: если запрошен какойто файл, отдаём его. если нет точки в строке (признак расширения файла) то идём на MVC
				if (!rawurl.Contains('.'))
				{
					try
					{
						IActionResult mvcres = factory.GetResult(ct);
						responsestring = mvcres.Content;
					}
					catch (Exception e)
					{
						responsestring = e.Message + Environment.NewLine + "<br/>" + e.StackTrace;
					}
				}
				else
				{
					if (ct.RawURL == "favicon.ico")
					{
						responsestring = "";
					}
					else
					{

						game.MoveUser(rawurl.TrimStart('/'));

						// рендерим поле
						String GameField = game.RenderField();

						//получаем файл-шаблон для нашей игры
						String tfile = GetFileContent("Game");

						//заменяем там тэг "<game /> на отрендеренное игровое поле
						tfile = tfile.Replace("<game />", GameField);

						//заполняем строку вывода на клиент
						responsestring = tfile;
					}
				}

				// получаем ответ клиенту из контекста
				HttpListenerResponse response = context.Response;

				// буфер - чтобы можно было обрабатывать по частям, кодируем всё в UTF8
				byte[] buffer = Encoding.UTF8.GetBytes(responsestring);
				// сколько там букаф? осилим?
				response.ContentLength64 = buffer.Length;
				// получаем поток из ответа
				Stream output = response.OutputStream;
				// пишем в поток содержимое буфера
				output.Write(buffer, 0, buffer.Length);
				// закрываем поток
				output.Close();

				// выводим в консоль на всякий случай чтобы если на клиенте чтото пойдёт не так мы в консоли видели проблему
				Console.WriteLine($"String {responsestring} sent to client");
			}
		}

		/// <summary>
		/// преобразуем строку вида Game/f1/ttt в объект HTTPContext - просто распихиваем всё по полочкам, чтобы не работать со строкой или массивом
		/// </summary>
		/// <param name="URL"></param>
		/// <returns></returns>
		

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

	public class Middlware
	{
		public Func<HTTPContext, HTTPContext> UseMethod;
        public Middlware(Func<HTTPContext,HTTPContext> n)
        {
			UseMethod = n;
        }
	}

	public class MiddlewareContainer
	{
		public static List<Middlware> MDWS = new List<Middlware>();
		public HTTPContext context;

		public void Init(HTTPContext context)
		{
			this.context = context;
		}
		public void Add(Func<HTTPContext, HTTPContext> f)
		{
			Middlware m = new Middlware(f);
			MDWS.Add(m);
		}
		public HTTPContext GetContext()
		{
			return context;
		}

		public void Run()
		{
			MDWS.ForEach(x =>
			{
				context = x.UseMethod(context);
			});
		}
	}
}
