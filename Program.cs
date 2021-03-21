using GNGame;
using Server.MVC;
using System;
using System.Drawing;
using System.IO;
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
            Renderer r = new Renderer();
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
                if (!rawurl.Contains('.'))
                {
                    HTTPContext ct = GetContext(rawurl.TrimStart('/'));
                    if(ct!=null)
                    {
                        Assembly asm = Assembly.GetCallingAssembly();
                        AssemblyName an = asm.GetName();
                        String ClassName = an.Name + "." + ct.Controller;
                        ObjectHandle tclass = Activator.CreateInstance(an.Name, ClassName);
                        Type type = asm.GetType(ClassName);
                        object unwc = tclass.Unwrap();
                        ((Controller)unwc).Context = ct;
                        String res = "";
                        if(ct.Action!=null)
                        {
                            res = type.GetMethod(ct.Action).Invoke(unwc, null).ToString();
                        }
                    }
                }

                HttpListenerResponse response = context.Response;

                String responsestring = "hi from server. you sent: "+ rawurl;

                game.MoveUser(rawurl.TrimStart('/'));
                String GameField = game.RenderField();
                String tfile = GetFileContent("Game");
                tfile = tfile.Replace("<game />", GameField);

                byte[] buffer = Encoding.UTF8.GetBytes(tfile);
                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();

                Console.WriteLine($"String {responsestring} sent to client");
            }
        }

        private static HTTPContext GetContext(String URL)/// Game/f1/ttt
        {
            HTTPContext c = new HTTPContext();

            String[] s = URL.Split('/');

            if (s.Length > 0)
            {
                c.Controller = s[0];
            }
            if (s.Length > 1)
            {
                c.Action = s[1];
            }
            if(s.Length>2)
            {
                c.Params = s[2];
            }

            return c;
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
            String res = Program.SiteFolder+"/"+filename+".html";
            return res;
        }
    }
}
