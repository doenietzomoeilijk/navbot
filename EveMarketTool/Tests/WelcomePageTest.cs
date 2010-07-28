using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using NUnit.Framework;
using System.Configuration;



namespace EveMarketTool.Tests
{
    [TestFixture]
    public class WelcomePageTest
    {
        WelcomePage page;
        string charName = "Jameson";
        string charId = "9816250";
        string system = "Sol";
        NameValueCollection emptyHeaders;
        NameValueCollection emptyQuery;
        NameValueCollection trustedHeaders;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            page = new WelcomePage();
            emptyHeaders = new NameValueCollection();
            emptyQuery = new NameValueCollection();
            trustedHeaders = new NameValueCollection();
            trustedHeaders["Eve.solarsystemname"] = system;
            trustedHeaders["Eve.charname"] = charName;
            trustedHeaders["Eve.charid"] = charId;
        }

        [SetUp]
        public void TestCaseSetUp()
        {
        }

        [Test]
        public void WelcomePageWithoutTrust()
        {
            string html = page.Render(null, null, null, emptyHeaders, emptyQuery);
            string textOnly = WebUtils.PlainText(html);

            Assert.IsTrue(textOnly.Contains("Welcome captain, it is my pleasure to serve you."), "Not a pleasure to serve me");
            Assert.IsTrue(textOnly.Contains("To do this, I need to be added to your 'trusted' list."), "No trust request");
            Assert.IsTrue(html.Contains(ConfigurationSettings.AppSettings["URLPrefix"] + "trustme"), "No trustme link");
            Assert.IsTrue(textOnly.Contains("This lets me know which system we are in, which helps me find the most profitable routes for us to take!"), "No trust explanation");
            Assert.IsTrue(html.Contains("<title>NavBot</title>"), "HTML title tag missing");
        }

        [Test]
        public void WelcomePageWithTrust()
        {
            string html = page.Render(system, charName, charId, trustedHeaders, emptyQuery);
            string textOnly = WebUtils.PlainText(html);

            Assert.IsTrue(textOnly.Contains("Jameson"), "Forgot Captain Jameson's name");
            Assert.IsTrue(textOnly.Contains("to assist you in finding the most profitable trade routes in the universe."), "No purpose given");
            Assert.IsTrue(html.Contains(ConfigurationSettings.AppSettings["URLPrefix"] + "Search"), "No search link");
            Assert.IsTrue(html.Contains(ConfigurationSettings.AppSettings["URLPrefix"] + "Reports"), "No reports link");
            Assert.IsTrue(html.Contains("<title>NavBot</title>"), "HTML title tag missing");
        }
    }
}
