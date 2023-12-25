using Microsoft.VisualStudio.TestTools.UnitTesting;
using DiscordBot.Classes;

namespace DiscordBotUnitTests
{
    [TestClass]
    public class StringUtilsTests
    {

        #region ReplaceLoopHoles
        [TestMethod]
        public void ReplaceLoopHoles_NullInput()
        {
            string content = "";
            string result = StringUtils.ReplaceLoopHoles(content);
            Assert.AreEqual(result, "");
        }

        [TestMethod]
        public void ReplaceLoopHoles_StartChange()
        {
            string content = "3ye";
            string result = StringUtils.ReplaceLoopHoles(content);
            Assert.AreEqual(result, "eye");
        }

        [TestMethod]
        public void ReplaceLoopHoles_EndChange()
        {
            string content = "ey3";
            string result = StringUtils.ReplaceLoopHoles(content);
            Assert.AreEqual(result, "eye");
        }

        [TestMethod]
        public void ReplaceLoopHoles_MiddleChange()
        {
            string content = "P3N";
            string result = StringUtils.ReplaceLoopHoles(content);
            Assert.AreEqual(result, "PeN");
        }

        [TestMethod]
        public void ReplaceLoopHoles_AllChange()
        {
            string content = "3333";
            string result = StringUtils.ReplaceLoopHoles(content);
            Assert.AreEqual(result, "eeee");
        }
        #endregion

    }
}
