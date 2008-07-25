using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using EveMarketTool.Tests.Mock_Objects;

namespace EveMarketTool.Tests
{
    [TestFixture]
    public class TradeFinderFactoryTest
    {
        Map map;
        ItemDatabase database;
        Market market;
        Parameters parameters;
        string testLogPath;
        string testNoLogPath;
        string archiveBasePath;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            testLogPath = TestObjectFactory.TestDirectory + "Logs";
            testNoLogPath = TestObjectFactory.TestDirectory + "NoLogs";
            archiveBasePath = TestObjectFactory.TestDirectory + "Logs" + "\\Temp";
            map = TestObjectFactory.CreateMap();
            database = TestObjectFactory.CreateItemDatabase();
            market = TestObjectFactory.CreateMarket(database, map);
            parameters = new Parameters(1000.0f, 100.0f, "Sol", TripType.SingleTrip);
        }

        [SetUp]
        public void TestCaseSetUp()
        {
            if (Directory.Exists(archiveBasePath))
                Directory.Delete(archiveBasePath, true);

            Directory.CreateDirectory(archiveBasePath);
            Directory.CreateDirectory(archiveBasePath + "\\Archive");
        }

        [Test]
        public void TestNoLogs()
        {
            TradeFinder finder = TradeFinderFactory.Create(map, market, testNoLogPath, parameters);
            Assert.IsNull(finder, "Valid trade finder returned by create instead of null");
        }

        [Test]
        public void TestAutoArchiveNoDirectory()
        {
            if (Directory.Exists(archiveBasePath + "\\Archive"))
                Directory.Delete(archiveBasePath + "\\Archive", true);

            TradeFinderFactory.ArchiveOutOfDateLogs(archiveBasePath);

            Assert.IsTrue(Directory.Exists(archiveBasePath + "\\Archive"), "Didn't create the archive directory");
        }

        [Test]
        public void TestArchiveOutOfDateLogs()
        {
            // copy files from Old Logs to the temporary logs dir
            string[] oldLogs = Directory.GetFiles(testLogPath + "\\Old Logs\\", "*.txt", System.IO.SearchOption.TopDirectoryOnly);
            foreach (string file in oldLogs)
            {
                FileInfo info = new FileInfo(file);
                info.CopyTo(archiveBasePath + "\\" + info.Name);
            }

            // copy files from Special Logs to the temporary logs dir
            string[] specialLogs = Directory.GetFiles(testLogPath + "\\Special Logs\\", "*.txt", System.IO.SearchOption.TopDirectoryOnly);
            foreach (string file in specialLogs)
            {
                FileInfo info = new FileInfo(file);
                info.CopyTo(archiveBasePath + "\\" + info.Name);
            }

            // copy files from Logs to the temporary logs dir
            string[] newLogs = Directory.GetFiles(testLogPath, "*.txt", System.IO.SearchOption.TopDirectoryOnly);
            foreach (string file in newLogs)
            {
                FileInfo info = new FileInfo(file);
                info.CopyTo(archiveBasePath + "\\" + info.Name);
            }

            TradeFinderFactory.ArchiveOutOfDateLogs(archiveBasePath);

            Assert.IsTrue(Directory.Exists(archiveBasePath + "\\Archive"), "Archive directory went missing!");

            string[] unarchivedLogs = Directory.GetFiles(archiveBasePath, "*.txt", System.IO.SearchOption.TopDirectoryOnly);
            // Note: we expect newLogs.Length + 2 because two new item types are in the Old Logs directory
            Assert.AreEqual(newLogs.Length + 2, unarchivedLogs.Length, "Some of the new logs have gone missing");
            
            string[] archivedLogs = Directory.GetFiles(archiveBasePath + "\\Archive", "*.txt", System.IO.SearchOption.TopDirectoryOnly);
            for (int i = 0; i < archivedLogs.Length; i++)
            {
                archivedLogs[i] = (new FileInfo(archivedLogs[i])).Name;
            }

            // Note: we expect oldLogs.Length + 1 because one of the new item types is doubled
            Assert.AreEqual(oldLogs.Length + 1, archivedLogs.Length);
            foreach (string log in oldLogs)
            {
                Assert.Contains((new FileInfo(log)).Name, archivedLogs);
            }
        }

        [Test]
        public void TestCreate()
        {
            TradeFinder finder = MockTradeFinderFactory.CreateMock(map, market, testLogPath, parameters);
            Assert.IsNotNull(finder, "Null trade finder returned by create instead of a valid one");
            Assert.IsNotEmpty(finder.SingleTrips);
        }

        [Test]
        public void TestArchiveOldLogs()
        {
            // copy files from Old Logs to the temporary logs dir
            string[] oldLogs = Directory.GetFiles(testLogPath + @"\Old Logs\", "*.txt", System.IO.SearchOption.TopDirectoryOnly);
            foreach (string file in oldLogs)
            {
                FileInfo info = new FileInfo(file);
                info.CopyTo(archiveBasePath + @"\" + info.Name);
            }

            // Create a dummy log whose name indicates it is less than a day old (created at midnight today will do)
            FileInfo newInfo = new FileInfo(oldLogs[0]);
            DateTime now = TimeZone.CurrentTimeZone.ToUniversalTime(DateTime.Now - TimeSpan.FromHours(6));
            string newName = archiveBasePath + "\\" + string.Format("MarketTestInput - AutoGen - {0}.{1}.{2} {3}0000.txt", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            newInfo.CopyTo(newName);

            TradeFinderFactory factory = new TradeFinderFactory(archiveBasePath);
            factory.ArchiveOldLogs();

            Assert.IsTrue(Directory.Exists(archiveBasePath + "\\Archive"), "Archive directory went missing!");
            
            string[] unarchivedLogs = Directory.GetFiles(archiveBasePath, "*.txt", System.IO.SearchOption.TopDirectoryOnly);
            Assert.AreEqual(1, unarchivedLogs.Length, "The wrong number of logs were left unarchived");
            Assert.AreEqual(newName, unarchivedLogs[0]);
            
            string[] archivedLogs = Directory.GetFiles(archiveBasePath + "\\Archive", "*.txt", System.IO.SearchOption.TopDirectoryOnly);
            for (int i = 0; i < archivedLogs.Length; i++)
            {
                archivedLogs[i] = (new FileInfo(archivedLogs[i])).Name;
            }

            Assert.AreEqual(oldLogs.Length, archivedLogs.Length);
            foreach (string log in oldLogs)
            {
                Assert.Contains((new FileInfo(log)).Name, archivedLogs);
            }
        }

        [Test]
        public void TestArchiveAllLogs()
        {
            // copy files from Old Logs to the temporary logs dir
            string[] oldLogs = Directory.GetFiles(testLogPath + "\\Old Logs\\", "*.txt", System.IO.SearchOption.TopDirectoryOnly);
            foreach (string file in oldLogs)
            {
                FileInfo info = new FileInfo(file);
                info.CopyTo(archiveBasePath + "\\" + info.Name);
            }

            TradeFinderFactory factory = new TradeFinderFactory(archiveBasePath);
            factory.ArchiveAllLogs();

            Assert.IsTrue(Directory.Exists(archiveBasePath + "\\Archive"), "Archive directory went missing!");

            string[] unarchivedLogs = Directory.GetFiles(archiveBasePath, "*.txt", System.IO.SearchOption.TopDirectoryOnly);
            Assert.AreEqual(0, unarchivedLogs.Length, "Some logs were not archived");

            string[] archivedLogs = Directory.GetFiles(archiveBasePath + "\\Archive", "*.txt", System.IO.SearchOption.TopDirectoryOnly);
            for (int i = 0; i < archivedLogs.Length; i++)
            {
                archivedLogs[i] = (new FileInfo(archivedLogs[i])).Name;
            }

            Assert.AreEqual(oldLogs.Length, archivedLogs.Length);
            foreach (string log in oldLogs)
            {
                Assert.Contains((new FileInfo(log)).Name, archivedLogs);
            }
        }
    }
}
