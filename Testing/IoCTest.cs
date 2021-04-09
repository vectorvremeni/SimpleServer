using IoC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTests
{
	[TestClass]
	public class IoCTest
	{
		[TestMethod]
		public void T1_IoC()
		{
			var container = new IoCContainer();

			container.Register<IWelcomer, Welcomer>();
			container.Register<IWriter, ConsoleWriter>();

            Type t = typeof(Welcomer);

			var welcomer = container.Create<IWelcomer>();

            String w = "World";

			String test = welcomer.SayHelloTo("World");

            Assert.AreEqual($"Hello {w}!", test);

		}
	}

    public interface IWelcomer
    {
        String SayHelloTo(string name);
    }

    public class Welcomer : IWelcomer
    {
        private IWriter writer;

        public Welcomer(IWriter writer)
        {
            this.writer = writer;
        }

        public String SayHelloTo(string name)
        {
            return writer.Write($"Hello {name}!");
        }
    }

    public interface IWriter
    {
        String Write(string s);
    }

    public class ConsoleWriter : IWriter
    {
        public String Write(string s)
        {
            return s;
        }
    }
}
