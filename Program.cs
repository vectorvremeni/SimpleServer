using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using TheGame;

namespace ConsoleApp3
{
    class Program
    {
        public static Game game = new Game();
        static String SiteFolder = "Files";

        static void Main(string[] args)
        {
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
