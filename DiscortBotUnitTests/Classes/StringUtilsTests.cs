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
            Assert.AreEqual("Eye", result);
        }

        [TestMethod]
        public void ReplaceLoopHoles_EndChange()
        {
            string content = "ey3";
            string result = StringUtils.ReplaceLoopHoles(content);
            Assert.AreEqual("eyE", result);
        }

        [TestMethod]
        public void ReplaceLoopHoles_MiddleChange()
        {
            string content = "P3N";
            string result = StringUtils.ReplaceLoopHoles(content);
            Assert.AreEqual("PEN", result);
        }

        [TestMethod]
        public void ReplaceLoopHoles_AllChange()
        {
            string content = "3333";
            string result = StringUtils.ReplaceLoopHoles(content);
            Assert.AreEqual("EEEE", result);
        }

        [TestMethod]
        public void ReplaceLoopHoles_AccentAndSpaces()
        {
            string content = "è é ê ë";
            string result = StringUtils.ReplaceLoopHoles(content);
            Assert.AreEqual("e e e e", result);
        }
        #endregion

        #region StripWhitespace
        [TestMethod]
        public void StripWhitespace_NullInput()
        {
            string content = "";
            string result = StringUtils.StripWhitespace(content);
            Assert.AreEqual(result, "");
        }

        [TestMethod]
        public void StripWhitespace_Space()
        {
            string content = " ";
            string result = StringUtils.StripWhitespace(content);
            Assert.AreEqual(result, "");
        }

        [TestMethod]
        public void StripWhitespace_Tab()
        {
            string content = "  ";
            string result = StringUtils.StripWhitespace(content);
            Assert.AreEqual(result, "");
        }

        [TestMethod]
        public void StripWhitespace_HasWhitespace()
        {
            string content = "Leon The Professional";
            string result = StringUtils.StripWhitespace(content);
            Assert.AreEqual(result, "LeonTheProfessional");
        }

        [TestMethod]
        public void StripWhitespace_HasWhitespaceAndAccentCharacters()
        {
            string content = "Léon The Professional";
            string result = StringUtils.StripWhitespace(content);
            Assert.AreEqual(result, "LéonTheProfessional");
        }

        [TestMethod]
        public void StripWhitespace_HasWhitespaceAndSpecialCharacters()
        {
            string content = "Léon: The Professional";
            string result = StringUtils.StripWhitespace(content);
            Assert.AreEqual(result, "Léon:TheProfessional");
        }

        #endregion

        #region StripSpecialCharacters
        [TestMethod]
        public void StripSpecialCharacters_NullInput()
        {
            string content = "";
            string result = StringUtils.StripSpecialCharacters(content);
            Assert.AreEqual(result, "");
        }

        [TestMethod]
        public void StripSpecialCharacters_SpaceInput()
        {
            string content = " ";
            string result = StringUtils.StripSpecialCharacters(content);
            Assert.AreEqual(result, " ");
        }

        [TestMethod]
        public void StripSpecialCharacters_NoSpecialCharacters()
        {
            string content = "Léon The Professional";
            string result = StringUtils.StripSpecialCharacters(content);
            Assert.AreEqual(result, content);
        }

        [TestMethod]
        public void StripSpecialCharacters_SpecialCharacters()
        {
            string content = "Léon: The Professional";
            string result = StringUtils.StripSpecialCharacters(content);
            Assert.AreEqual(result, "Léon The Professional");
        }
        #endregion

    }
}
