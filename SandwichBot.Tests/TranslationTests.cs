using HoLLy.DiscordBot.Sandwich.Tools;
using NUnit.Framework;

namespace SandwichBot.Tests
{
    public class TranslationTests
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public void GenerateToken()
        {
            Assert.AreEqual("783049.878026", GoogleTranslate.GenerateToken("test", "431875.931032818"));
            Assert.AreEqual("661741.823272", GoogleTranslate.GenerateToken("test", "431877.1894360868"));
            Assert.AreEqual("460360.102733", GoogleTranslate.GenerateToken("This is a test.", "431877.1894360868"));
        }

        [Test]
        public void TestTranslation()
        {
            Assert.AreEqual("This is a translation.", GoogleTranslate.Translate("Dit is een vertaling.").Result);
        }
    }
}
