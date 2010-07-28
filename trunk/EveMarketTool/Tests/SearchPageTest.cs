using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using NUnit.Framework;
using EveMarketTool.Tests.Mock_Objects;
using System.Windows.Forms;
using System.Globalization;
using System.Configuration;

namespace EveMarketTool.Tests
{
    [TestFixture]
    public class SearchPageTest
    {
        SearchPage page;
        string charName = "Jameson";
        string charId = "9816250";
        string system = "HighSec1";
        NameValueCollection trustedHeaders;
        NameValueCollection query;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            trustedHeaders = new NameValueCollection();
            trustedHeaders["Eve.solarsystemname"] = system;
            trustedHeaders["Eve.charname"] = charName;
            trustedHeaders["Eve.charid"] = charId;
        }

        [SetUp]
        public void TestCaseSetUp()
        {
            page = new SearchPage(TestObjectFactory.CreateMockTradeFinderFactory());
            query = new NameValueCollection();
        }

        [Test]
        public void FormElements()
        {
            Application.UserAppDataRegistry.DeleteValue("LastKnownIsk", false);
            Application.UserAppDataRegistry.DeleteValue("LastKnownCargoSpace", false);

            string html = page.Render(system, charName, charId, trustedHeaders, query);
            string lowerCaseText = WebUtils.PlainText(html).ToLower();

            Assert.IsTrue(html.Contains("<form>") && html.Contains("</form>"), "Form elements not found");
            Assert.IsTrue(html.Contains("<input type=\"text\" name=\"isk\" value=\"\""), "Empty isk entry box not found");
            Assert.IsTrue(html.Contains("<input type=\"text\" name=\"cargo\" value=\"\""), "Empty cargo entry box not found");
            Assert.IsTrue(html.Contains("<input type=\"submit\""), "Submit button not found");
            Assert.IsTrue(lowerCaseText.Contains("isk"), "Isk label not found");
            Assert.IsTrue(lowerCaseText.Contains("cargo"), "Cargo label not found");
        }

        [Test]
        public void FilledFormElements()
        {
            query["isk"] = "142000";
            query["cargo"] = "412";
            string html = page.Render(system, charName, charId, trustedHeaders, query);
            string lowerCaseText = WebUtils.PlainText(html).ToLower();

            Assert.IsTrue(html.Contains("<input type=\"text\" name=\"isk\" value=\"142000\""), "Isk box not filled");
            Assert.IsTrue(html.Contains("<input type=\"text\" name=\"cargo\" value=\"412\""), "Cargo box not filled");
        }

        [Test]
        public void MissingInput()
        {
            Application.UserAppDataRegistry.DeleteValue("LastKnownIsk", false);
            Application.UserAppDataRegistry.DeleteValue("LastKnownCargoSpace", false);

            string html = page.Render(system, charName, charId, trustedHeaders, query);
            string lowerCaseText = WebUtils.PlainText(html).ToLower();

            Assert.IsTrue(lowerCaseText.Contains("fill in your available isk and cargo space"), "Warning not displayed");
            Assert.IsFalse(lowerCaseText.Contains("Here's the trades"), "Local trades were displayed even when isk was not specified!");
            Assert.IsFalse(lowerCaseText.Contains("Here's how much profit we could make"), "Universal trades were displayed even when isk was not specified!");
        }

        [Test]
        public void ValuesFromRegistryUsed()
        {
            Application.UserAppDataRegistry.SetValue("LastKnownIsk", "31415000");
            Application.UserAppDataRegistry.SetValue("LastKnownCargoSpace", "926534.1");

            string html = page.Render(system, charName, charId, trustedHeaders, query);

            Assert.IsTrue(html.Contains("<input type=\"text\" name=\"isk\" value=\"31415000\""), "Isk box not filled");
            Assert.IsTrue(html.Contains("<input type=\"text\" name=\"cargo\" value=\"926534.1\""), "Cargo box not filled");
        }

        [Test]
        public void ValuesFromRegistryUpdated()
        {
            Application.UserAppDataRegistry.DeleteValue("LastKnownIsk", false);
            Application.UserAppDataRegistry.SetValue("LastKnownCargoSpace", "926534.1");
            query["isk"] = "142000";
            query["cargo"] = "412";

            string html = page.Render(system, charName, charId, trustedHeaders, query);

            Assert.IsTrue(html.Contains("<input type=\"text\" name=\"isk\" value=\"142000\""), "Isk box not filled");
            Assert.IsTrue(html.Contains("<input type=\"text\" name=\"cargo\" value=\"412\""), "Cargo box not filled");
            Assert.AreEqual("142000", Application.UserAppDataRegistry.GetValue("LastKnownIsk") as string);
            Assert.AreEqual("412", Application.UserAppDataRegistry.GetValue("LastKnownCargoSpace") as string);
        }

        [Test]
        public void NonNumberInput()
        {
            query["isk"] = "acephaline42";
            query["cargo"] = "1";
            string html = page.Render(system, charName, charId, trustedHeaders, query);
            string lowerCaseText = WebUtils.PlainText(html).ToLower();

            Assert.IsTrue(lowerCaseText.Contains("tell me how much isk you have using numbers"), "No request for isk in numbers");

            query = new NameValueCollection();
            query["isk"] = "1";
            query["cargo"] = "12explosion";
            html = page.Render(system, charName, charId, trustedHeaders, query);
            lowerCaseText = WebUtils.PlainText(html).ToLower();

            Assert.IsTrue(lowerCaseText.Contains("tell me how much cargo space you have using numbers"), "No request for cargo space in numbers");

            query = new NameValueCollection();
            query["isk"] = "@##2134";
            query["cargo"] = "321-afds";
            html = page.Render(system, charName, charId, trustedHeaders, query);
            lowerCaseText = WebUtils.PlainText(html).ToLower();

            Assert.IsTrue(lowerCaseText.Contains("tell me how much isk and cargo space you have using numbers"), "No request for isk AND cargo space in numbers");
        }

        [Test]
        public void NegativeInput()
        {
            query["isk"] = "-1200";
            query["cargo"] = "1";
            string html = page.Render(system, charName, charId, trustedHeaders, query);
            string lowerCaseText = WebUtils.PlainText(html).ToLower();

            Assert.IsTrue(lowerCaseText.Contains("negative isk"), "No mention of negative isk");

            query = new NameValueCollection();
            query["isk"] = "1";
            query["cargo"] = "-1";
            html = page.Render(system, charName, charId, trustedHeaders, query);
            lowerCaseText = WebUtils.PlainText(html).ToLower();

            Assert.IsTrue(lowerCaseText.Contains("negative cargo space"), "No mention of negative cargo space");

            query = new NameValueCollection();
            query["isk"] = "-43656625";
            query["cargo"] = "-13428";
            html = page.Render(system, charName, charId, trustedHeaders, query);
            lowerCaseText = WebUtils.PlainText(html).ToLower();

            Assert.IsTrue(lowerCaseText.Contains("negative isk and cargo space"), "No mention of negative isk AND cargo space");
        }

        [Test]
        public void NoLogFiles()
        {
            page = new SearchPage(new MockTradeFinderFactory());
            string html = page.Render(system, charName, charId, trustedHeaders, query);
            string lowerCaseText = WebUtils.PlainText(html).ToLower();

            Assert.IsTrue(lowerCaseText.Contains("export to file"), "No mention of using the export to file button");
            Assert.IsTrue(lowerCaseText.Contains("market data"), "No mention of needing market data");
        }

        [Test]
        public void EverythingOk()
        {
            page = new SearchPage(TestObjectFactory.CreateMockTradeFinderFactory());
            query["isk"] = "142000";
            query["cargo"] = "412";
            string html = page.Render(system, charName, charId, trustedHeaders, query);
            string lowerCaseText = WebUtils.PlainText(html).ToLower();

            Assert.IsTrue(lowerCaseText.Contains("here's the trades"), "Didn't say the trades are here");
            Assert.IsTrue(html.Contains(ConfigurationSettings.AppSettings["URLPrefix"] + "Reports"), "No reports link");
        }

        [Test]
        public void FormatIsk()
        {
            if (CultureInfo.CurrentCulture.Name == "de-DE")
            {
                Assert.AreEqual("0,0 isk", page.FormatIsk(0.0f));
                Assert.AreEqual("0,1 isk", page.FormatIsk(0.1f));
                Assert.AreEqual("123,4 isk", page.FormatIsk(123.4f));
                Assert.AreEqual("1,2K isk", page.FormatIsk(1234.5f));
                Assert.AreEqual("12,3K isk", page.FormatIsk(12345.6f));
                Assert.AreEqual("10K isk", page.FormatIsk(10000.0f));
                Assert.AreEqual("123,5K isk", page.FormatIsk(123456.7f));
                Assert.AreEqual("1,2M isk", page.FormatIsk(1234567.8f));
                Assert.AreEqual("2M isk", page.FormatIsk(2000000.0f));
                Assert.AreEqual("12,3M isk", page.FormatIsk(12345678.9f));
                Assert.AreEqual("123,5M isk", page.FormatIsk(123456789.0f));
                Assert.AreEqual("1.234,6M isk", page.FormatIsk(1234567890.0f));
            }
            else
            {
                Assert.Ignore("This test is written for systems using a German culture setting.");
            }
        }

        [Test]
        public void FormatPercent()
        {
            Assert.AreEqual("100%", page.FormatPercent(1.0f));
            Assert.AreEqual("0%", page.FormatPercent(0.0f));
            Assert.AreEqual("248%", page.FormatPercent(2.475f));
            Assert.AreEqual("17%", page.FormatPercent(0.1661f));
        }

        [Test]
        public void Pluralize()
        {
            Assert.AreEqual("jumps", page.Pluralize("jump", "jumps", 0));
            Assert.AreEqual("jumps", page.Pluralize("jump", "jumps", 2));
            Assert.AreEqual("jump", page.Pluralize("jump", "jumps", 1));
            Assert.AreEqual("jumps", page.Pluralize("jump", "jumps", 200));
        }

        [Test]
        public void ContainsTitle()
        {
            string html = page.Render(system, charName, charId, trustedHeaders, query);
            Assert.IsTrue(html.Contains("<title>NavBot</title>"), "HTML title tag missing");
        }
    }
}
