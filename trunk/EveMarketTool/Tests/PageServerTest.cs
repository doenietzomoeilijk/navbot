using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using NUnit.Framework;
using EveMarketTool.Tests.Mock_Objects;
using System.Configuration;

namespace EveMarketTool.Tests
{
    [TestFixture]
    public class PageServerTest
    {
        PageServer server;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            server = new PageServer();
        }

        [SetUp]
        public void TestCaseSetUp()
        {
            server.Reset();
        }

        [Test]
        public void TestNoPages()
        {
            string text = WebUtils.PlainText(WebUtils.ReadPage(server.Url));
            Assert.IsTrue(text.Contains("Oops"), "Didn't say oops");
            Assert.IsTrue(text.Contains("I seem to have lost my mind. Please report this to Tejar at once!"), "Didn't ask for a bug report");
        }

        [Test]
        public void TestDefaultPage()
        {
            MockPage page = new MockPage();
            server.AddPage(page);
            string text = WebUtils.PlainText(WebUtils.ReadPage(server.Url));
            Assert.IsTrue(text.Contains("test page"), "Didn't output test page");
        }

        [Test]
        public void TestPageWithoutHeaders()
        {
            MockPage page = new MockPage();
            server.AddPage(page);
            string text = WebUtils.PlainText(WebUtils.ReadPage(server.Url+page.Name()));
            Assert.IsTrue(text.Contains("test page"), "Didn't output test page");
            Assert.AreEqual(null, page.SystemName);
            Assert.AreEqual(null, page.CharName);
            Assert.AreEqual(null, page.CharId);
            Assert.AreEqual(1, page.Headers.Count);
            Assert.AreEqual("Host", page.Headers.GetKey(0));
        }

        [Test]
        public void TestPageWithHeaders()
        {
            MockPage page = new MockPage();
            server.AddPage(page);
            NameValueCollection headers = new NameValueCollection();
            headers.Add("Eve.solarsystemname", "Sol");
            headers.Add("Eve.charname", "Mark");
            headers.Add("Eve.charid", "1");
            headers.Add("Eve.otherinformation", "Sunsets are beautiful");
            string text = WebUtils.PlainText(WebUtils.ReadPageWithHeaders(server.Url + page.Name(), headers, null));
            Assert.IsTrue(text.Contains("test page"), "Didn't output test page");
            Assert.AreEqual("Sol", page.SystemName);
            Assert.AreEqual("Mark", page.CharName);
            Assert.AreEqual("1", page.CharId);
            Assert.AreEqual(5, page.Headers.Count);
            Assert.AreEqual("Sunsets are beautiful", page.Headers["Eve.otherinformation"]);
        }

        [Test]
        public void TestGetForm()
        {
            MockPage page = new MockPage();
            server.AddPage(page);
            string query = "?rabbits=funny&fortytwo=42&floating=134.2&german=99,31";
            WebUtils.ReadPage(server.Url + page.Name() + query);
            Assert.AreEqual("funny", page.Query["rabbits"]);
            Assert.AreEqual("42", page.Query["fortytwo"]);
            Assert.AreEqual("134.2", page.Query["floating"]);
            Assert.AreEqual("99,31", page.Query["german"]);
            Assert.AreEqual(4, page.Query.Count);
        }

        [Test]
        public void TestTrustMePage()
        {
            NameValueCollection outputHeaders = new NameValueCollection();
            string html = WebUtils.ReadPageWithHeaders(server.Url + "trustme", new NameValueCollection(), outputHeaders);
            Assert.AreEqual(ConfigurationSettings.AppSettings["URLPrefix"], outputHeaders["eve.trustme"]);
        }
    }
}
