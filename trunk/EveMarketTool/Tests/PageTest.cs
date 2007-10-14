using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace EveMarketTool.Tests
{
    [TestFixture]
    public class PageTest
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
        }

        [Test]
        public void ReadFromResource()
        {
            string text = Page.ReadFromResource("Tests.PageTestInput.txt");
            Assert.AreEqual("Shall I compare thee to a summer's day?", text);
        }
    }
}
