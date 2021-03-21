using GNGame;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTests
{
    [TestClass]
    public class GNGameTests
    {
        [TestMethod]
        public void T1_1()
        {
            GuessGame g = new GuessGame();
            g.init(3);
            g.init_test(5);

            string r1 = g.guess(3);
            Assert.AreEqual(GuessGame.Gr_less,r1);

            string r2 = g.guess(8);
            Assert.AreEqual(GuessGame.Gr_more, r2);

            string r3 = g.guess(10);
            Assert.AreEqual(GuessGame.Gr_lose, r3);

            g.init(3);
            g.init_test(5);

            string r4 = g.guess(5);
            Assert.AreEqual(GuessGame.Gr_equal, r4);
        }
    }
}
