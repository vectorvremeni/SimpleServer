using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheGame;

namespace GameTests
{
    [TestClass]
    public class GameTesting
    {
        [TestMethod]
        public void A1_Dimensions_Test()
        {
            Renderer r = new Renderer();
            GoGame g = new GoGame(r);
            g.Init(5, 5);

            Assert.AreEqual(5, g.DimX);
            Assert.AreEqual(5, g.DimY);

            g = new GoGame(r);

            g.Init(10, 5);

            Assert.AreEqual(10, g.DimX);
            Assert.AreEqual(5, g.DimY);
        }

        [TestMethod]
        public void A2_MoveUser_Test()
        {
            Renderer r = new Renderer();
            GoGame g = new GoGame(r);
            g.Init(5, 5);

            Assert.AreEqual(0, g.User.X);

            g.MoveUser("RIGHT");

            Assert.AreEqual(1, g.User.X);
        }
    }
}
