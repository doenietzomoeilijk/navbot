using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace EveMarketTool.Tests
{
    [TestFixture]
    public class ReportInfoTest
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
        }

        [SetUp]
        public void TestCaseSetUp()
        {
        }

        [Test]
        public void ItemName()
        {
            ReportInfo reportInfo = new ReportInfo("SomePath\\Logs\\MarketTestInput - Kernite - 2007.01.28 112233.txt");
            Assert.AreEqual("Kernite", reportInfo.ItemName);

            reportInfo = new ReportInfo("SomePath\\Logs\\MarketTestInput-Kernite-2007.01.28 112233.txt");
            Assert.AreEqual("Kernite", reportInfo.ItemName);
        }

        [Test]
        public void Region()
        {
            ReportInfo reportInfo = new ReportInfo("SomePath\\Logs\\FunnyRegion - Kernite - 2007.01.28 112233.txt");
            Assert.AreEqual("FunnyRegion", reportInfo.Region);

            reportInfo = new ReportInfo("SomePath\\Logs\\FunnyRegion-Kernite-2007.01.28 112233.txt");
            Assert.AreEqual("FunnyRegion", reportInfo.Region);
        }

        [Test]
        public void ItemDate()
        {
            DateTime expected = new DateTime(2007, 01, 14, 9, 30, 13);
            ReportInfo reportInfo = new ReportInfo("SomePath\\Logs\\MarketTestInput - Kernite - 2007.01.14 093013.txt");
            // dates should be reported in local time
            Assert.AreEqual(TimeZone.CurrentTimeZone.ToLocalTime(expected), reportInfo.Date);

            reportInfo = new ReportInfo("SomePath\\Logs\\MarketTestInput-Kernite-2007.01.14 093013.txt");
            // dates should be reported in local time
            Assert.AreEqual(TimeZone.CurrentTimeZone.ToLocalTime(expected), reportInfo.Date);
        }

        [Test]
        public void ItemDateIsInTheFuture()
        {
            ReportInfo reportInfo = new ReportInfo("SomePath\\Logs\\MarketTestInput - Kernite - 5007.01.01 010101.txt");
            // dates from the future should be clamped to the current date/time (accurate to the minute is good enough)
            string expected = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
            Assert.AreEqual(expected, reportInfo.Date.ToShortDateString() + " " + reportInfo.Date.ToShortTimeString());

            reportInfo = new ReportInfo("SomePath\\Logs\\MarketTestInput-Kernite-5007.01.01 010101.txt");
            // dates from the future should be clamped to the current date/time (accurate to the minute is good enough)
            Assert.AreEqual(expected, reportInfo.Date.ToShortDateString() + " " + reportInfo.Date.ToShortTimeString());
        }

        [Test]
        public void ItemsWithDashesInTheirName()
        {
            ReportInfo reportInfo = new ReportInfo("Lonetrek - Limited Ocular Filter - Beta - 2007.05.04 144310.txt");
            Assert.AreEqual("Limited Ocular Filter - Beta", reportInfo.ItemName);

            reportInfo = new ReportInfo("Lonetrek - Limited Ocular Filter 'Gunslinger' - TXL-1 - 2007.05.04 144310.txt");
            Assert.AreEqual("Limited Ocular Filter 'Gunslinger' - TXL-1", reportInfo.ItemName);

            reportInfo = new ReportInfo("Lonetrek-Limited Ocular Filter 'Gunslinger' - TXL-1-2007.05.04 144310.txt");
            Assert.AreEqual("Limited Ocular Filter 'Gunslinger' - TXL-1", reportInfo.ItemName);
        }
    }
}
