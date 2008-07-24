using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool.Tests.Mock_Objects
{
    public class MockTradeFinderFactory : TradeFinderFactory
    {
        public MockTradeFinderFactory() : base("")
        {
        }

        Map map;
        Market market;
        string logFilePath;
        Parameters parameters;

        public int ArchiveCalls = 0;
        public int ArchiveOldCalls = 0;
        public int ArchiveAllCalls = 0;
        public string[] Reports = new string[] { "C:\\SomePath\\OtherPath\\Path with spaces\\The Citadel - Navitas - 2007.02.27 112233.txt", "C:\\ShortPath\\New Region - Long-limbed Roes - 2007.02.28 072233.txt", "/Linux/style/path/with spaces/Lonetrek - Kernite - 2007.01.28 172658.txt" };

        public MockTradeFinderFactory(Map map, Market market, string logFilePath, Parameters parameters) : base("")
        {
            this.map = map;
            this.market = market;
            this.logFilePath = logFilePath;
            this.parameters = parameters;
        }

        public override TradeFinder Create()
        {
            if (map != null)
                return Create(map, market, TestObjectFactory.TestDirectory + "Logs", parameters);
            else 
                return null;
        }

        public override string[] ReportList()
        {
            return Reports;
        }

        public static TradeFinder CreateMock(Map map, Market market, string logFilePath, Parameters parameters)
        {
            return Create(map, market, TestObjectFactory.TestDirectory + "Logs", parameters);
        }

        public override void ArchiveOutOfDateLogs()
        {
            ArchiveCalls++;
        }

        public override void ArchiveOldLogs()
        {
            ArchiveOldCalls++;
        }

        public override void ArchiveAllLogs()
        {
            ArchiveAllCalls++;
        }
    }
}
