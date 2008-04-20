using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using NUnit.Framework;
using EveMarketTool.Tests.Mock_Objects;

namespace EveMarketTool.Tests
{
    [TestFixture]
    public class ReportsPageTest
    {
        ReportsPage page;
        NameValueCollection emptyHeaders;
        NameValueCollection emptyQuery;
        MockTradeFinderFactory factory;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
        }

        [SetUp]
        public void TestCaseSetUp()
        {
            factory = TestObjectFactory.CreateMockTradeFinderFactory();
            page = new ReportsPage(factory);
            emptyHeaders = new NameValueCollection();
            emptyQuery = new NameValueCollection();
        }

        [Test]
        public void TestRender()
        {
            factory.ArchiveCalls = 0;
            string html = page.Render(null, null, null, emptyHeaders, emptyQuery);
            string lowerCaseText = WebUtils.PlainText(html).ToLower();

            Assert.IsTrue(lowerCaseText.Contains("navitas"), "No mention of the navitas reports");
            Assert.IsTrue(lowerCaseText.Contains("kernite"), "No mention of the kernite reports");
            Assert.IsTrue(lowerCaseText.Contains("the citadel"), "No mention of the citadel");
            Assert.IsTrue(lowerCaseText.Contains("lonetrek"), "No mention of lonetrek");
            Assert.IsTrue(lowerCaseText.Contains("long-limbed roes"), "No mention of long-limbed roes (was it split at the hyphen?)");

            int navitasPosition = lowerCaseText.IndexOf("navitas");
            int kernitePosition = lowerCaseText.IndexOf("kernite");
            int roesPosition = lowerCaseText.IndexOf("long-limbed roes");

            // should be sorted by date (oldest first): kernite, navitas, roes
            Assert.Less(kernitePosition, navitasPosition, "The Kernite and Navitas were not sorted by date (Kernite should come first)");
            Assert.Less(navitasPosition, roesPosition, "The Navitas and Long-Limbed Roes were not sorted by date (Navitas should come first)");

            // Archive old logs should be called once per render
            Assert.AreEqual(1, factory.ArchiveCalls);

            Assert.IsTrue(html.Contains("http://localhost:9999/Reports?archive=old"), "No link to archive old reports");
            Assert.IsTrue(html.Contains("http://localhost:9999/Reports?archive=all"), "No link to archive all reports");
            Assert.IsTrue(html.Contains("http://localhost:9999/Search"), "No search link");
            Assert.IsTrue(html.Contains("http://localhost:9999/Reports"), "No refresh reports link");
        }

        [Test]
        public void TestNoReports()
        {
            factory.ArchiveCalls = 0;
            factory.Reports = new string[] { };
            string html = page.Render(null, null, null, emptyHeaders, emptyQuery);
            string lowerCaseText = WebUtils.PlainText(html).ToLower();

            Assert.IsFalse(html.Contains("http://localhost:9999/Reports?archive=old"), "Shouldn't link to archive old reports");
            Assert.IsFalse(html.Contains("http://localhost:9999/Reports?archive=all"), "Shouldn't link to archive all reports");
            Assert.IsTrue(html.Contains("http://localhost:9999/Reports"), "No refresh reports link");
        }

        [Test]
        public void TestArchiveOld()
        {
            NameValueCollection query = new NameValueCollection();
            query["archive"] = "old";
            factory.ArchiveOldCalls = 0;
            string html = page.Render(null, null, null, emptyHeaders, query);
            string lowerCaseText = WebUtils.PlainText(html).ToLower();
            Assert.AreEqual(1, factory.ArchiveOldCalls, "ArchiveOldLogs() should have been called once");
        }

        [Test]
        public void TestArchiveAll()
        {
            NameValueCollection query = new NameValueCollection();
            query["archive"] = "all";
            factory.ArchiveAllCalls = 0;
            string html = page.Render(null, null, null, emptyHeaders, query);
            string lowerCaseText = WebUtils.PlainText(html).ToLower();
            Assert.AreEqual(1, factory.ArchiveAllCalls, "ArchiveAllLogs() should have been called once");
        }

        [Test]
        public void ContainsTitle()
        {
            string html = page.Render(null, null, null, emptyHeaders, new NameValueCollection());
            Assert.IsTrue(html.Contains("<title>NavBot</title>"), "HTML title tag missing");
        }
    }
}
