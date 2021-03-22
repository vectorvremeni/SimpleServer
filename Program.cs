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
		/// <summary>
		/// наша игра ходилка
		/// </summary>
		public static GoGame game;
		static String SiteFolder = "Files";

		static void Main(string[] args)
		{
			// создаём HTML версию рендерера
			HTMLRenderer r = new HTMLRenderer();
			// создаём объект игры и пробрасываем ему рендерер (внедрение через конструктор)
			game = new GoGame(r);

			// создаём слушатель HTTP запросов
			HttpListener listener = new HttpListener();
			// настраиваем его чтобы он слушал на определённом порту
			listener.Prefixes.Add("http://*:55555/");
			// стартуем его
			listener.Start();

			// на всякий случай выводим в консоль чтобы мы знали что сервер слушает
			Console.WriteLine("listening");

			// инициализируем игру (ToDo: потом нужно это убрать в контроллер)
			game.Init(10, 5);

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
				String TrimmedURL = rawurl.TrimStart('/');

				// дальше идёт ветвление на MVC и просто файлы: если запрошен какойто файл, отдаём его. если нет точки в строке (признак расширения файла) то идём на MVC
				if (!rawurl.Contains('.'))
				{
					///MVC веточка

					// что такое HTTPContext? поставьте туда мышку и нажмите F12 (или RMB -> Go To Definition) 
					// тупо три переменные сформированные в одну кучку - класс
					// что делает функция GetContext - тоже можно посмотреть через F12:
					// она URL вида Controller/Action/Parameters разбирает на части по разделителю '/' и складывает в нужные части с проверками есть они или нет
					// не хотим париться как она это делает - выносим в отдельную функцию

					

					HTTPContext ct = GetContext(TrimmedURL);

					// если контекст удалось заполнить (хоть чтото)...
					if (ct != null)
					{
						// ...начинаем изучать Reflection
						// Reflection это среды исполнения CLR отвечать на вопросы программы о самой себе.
						// получаем ссылку на ту сборку которая запрашивает информацию о самой этой сборке
						Assembly asm = Assembly.GetCallingAssembly();

						// ещё одна новая тема - лямбда-выражения.
						// их особенность в том, что в качестве параметра им передаются не переменные, а алгоритмы (упакованные в анонимные функции)
						// например:
						// список (List) типов TypeInfo 


						// тут будет список всех типов из сборки, унаследованных от Controller
						List<TypeInfo> Controllers = 
							// получается так:
							// мы просим сборку (asm) 
							asm
							// дать нам все определённые типы (defined types)
							.DefinedTypes
							// дальше одно и то же объяснение разными словами - каждая строчка объясняет одноитоже
							// где (кагбэ функция без имени, которая принимает параметр и возвращает значение)
							// Where (то что пришло => а это я верну)
							// где (какойто входной аргумент типа TypeInfo (потому что List<TypeInfo>)) стрелка лямбды (=>) выражение которое возвращает да или нет (фильтр по которому ищем)
							// где (инфа_о_типе => унаследован ли инфа_о_типе от класса Controller?).преобразовать к списку
							// где (TypeInfo => является ли элемент BaseType переданного TypeInfo типом "Controller" приведённым к типу Type?).к списку
							.Where(x => x.BaseType == typeof(Controller)).ToList();

						// фух! сложно!
						// вот так правильно, на уроке не получалось потому что подход мы использовали слишком простой
						// вот так сложнее но эффективнее - можно класть типы в любую папочку и использовать для них любой namespace,
						// Reflection через методы сборки подберёт их из самой сборки и правильно настроит
						// вот магия Reflection!

						// получаем имя сборки - как мы называемся?
						AssemblyName an = asm.GetName();

						// получаем полное имя класса вместе с namespace чтобы проблема папочек решалась автоматически
						// имя класса с неймспейсом =  кого имя совпадает с тем что считается именем контроллера, пришедшим из браузера
						String ClassName = Controllers
							// список контроллеров, отфильтруй нам контроллеры по имени, которое совпадает с тем что в ct.Controller, 
							.Where(x => x.Name == ct.Controller)
							// и проконтролируй чтобы он там был, а если его там нет верни null
							.SingleOrDefault()? // внимание, ? означает что дальше идём только если тут чтото есть, если нет то следующая строка не будет выполняться. (сокращённая запись if != null)
							// если в предыдущей строке не был результат null то берём из того что получили FullName - это и будет имя класса (fully-qualified name), например assembly.namespace.classname, или к примеру Server.MVC.Home
							.FullName;

						// из имени класса и имени сборки создаём наш класс, завёрнутый в обслуживающую обёртку
						ObjectHandle tclass = Activator.CreateInstance(an.Name, ClassName);

						// получаем его тип (класс это сам класс например "Home" или "Game", а Type - это объект, который описывает этот класс, через него можно получить информацию об этом классе)
						Type type = asm.GetType(ClassName);

						// разворачиваем обёртку - получаем объект, созданный активатором из имени сборки и имени класса
						object unwc = tclass.Unwrap();

						//приводим развёрнутый объект к типу Controller чтобы иметь доступ к контексту, и чтобы компилятор мог проконтролировать это до исполнения
						((Controller)unwc).Context = ct;

						// если указан какойто метод, который нужно выполнить, то выполняем его (он поумолчанию Index)
						if (ct.Action != null)
						{
							// заполняем наш ответ тем, что вернёт метод контроллера (функция класса)
							// просим тип вызвать (invoke) заданный метод для заданного объекта класса
							// это так, потому что метод берётся из класса, а вызывается для объекта,
							// поэтому методу Invoke нужно передать объект, для которого вызвать этот метод
							// в конце приводим это всё к строке, потому что иначе компилятор не сможет проконтролировать тип
							responsestring = type.GetMethod(ct.Action).Invoke(unwc, null).ToString();
						}
					}
				}
				else
				{
					// веточка не MVC, старый тип запуска игр
					
					// идём игроком (выполняем команду с клиента)
					game.MoveUser(TrimmedURL);

					// рендерим поле
					String GameField = game.RenderField();

					//получаем файл-шаблон для нашей игры
					String tfile = GetFileContent("Game");

					//заменяем там тэг "<game /> на отрендеренное игровое поле
					tfile = tfile.Replace("<game />", GameField);

					//заполняем строку вывода на клиент
					responsestring = tfile;
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
