using System;
using System.Collections.Generic;
using System.Text;
using EveMarketTool.Tests.Mock_Objects;

namespace EveMarketTool.Tests
{
    class TestObjectFactory
    {
        public static string TestDirectory = "C:\\Dokumente und Einstellungen\\Mark\\Eigene Dateien\\My Code\\NavBot\\EveMarketTool\\Tests\\";

        public static ItemDatabase CreateItemDatabase()
        {
            return new ItemDatabase(TestDirectory + "ItemDatabaseTestInput.csv");
        }

        public static Map CreateMap()
        {
            /* Solar system test map (#stations, if more than 1):
             *                  /-- line3A
             * line1 --- line2 <                alone
             *  (2)             \-- line3B       (3)
             */

            SolarSystem line1 = new SolarSystem(1, "Line1");
            line1.Security = 1.0f;
            Station line11 = new Station(11, 0, "Line1 Station1", line1);
            Station line12 = new Station(12, 0, "Line1 Station2", line1);

            SolarSystem line2 = new SolarSystem(2, "Line2");
            line2.Security = 0.9f;
            Station line21 = new Station(21, 0, "Line2 Station1", line2);
            
            SolarSystem line3A = new SolarSystem(3, "Line3A");
            line3A.Security = 0.8f;
            Station line3A1 = new Station(31, 0, "Line3A Station1", line3A);
            Station line3A2 = new Station(32, 0, "Line3A Station2", line3A);
            
            SolarSystem line3B = new SolarSystem(4, "Line3B");
            line3B.Security = 0.7f;
            Station line3B1 = new Station(41, 0, "Line3B Station1", line3B);
            
            SolarSystem alone = new SolarSystem(5, "Alone");
            alone.Security = 0.0f;
            Station alone1 = new Station(51, 0, "Alone Station1", alone);
            Station alone2 = new Station(52, 0, "Alone Station2", alone);
            Station alone3 = new Station(53, 0, "Alone Station3", alone);

            GateBetween(line1, line2);
            GateBetween(line2, line3A);
            GateBetween(line2, line3B);

            MockSolarSystemsReader mockSystemsReader = new MockSolarSystemsReader();
            mockSystemsReader.AddSystem(line1);
            mockSystemsReader.AddSystem(line2);
            mockSystemsReader.AddSystem(line3A);
            mockSystemsReader.AddSystem(line3B);
            mockSystemsReader.AddSystem(alone);
            MockStationReader mockStationReader = new MockStationReader();
            mockStationReader.AddStation(line11);
            mockStationReader.AddStation(line12);
            mockStationReader.AddStation(line21);
            mockStationReader.AddStation(line3A1);
            mockStationReader.AddStation(line3A2);
            mockStationReader.AddStation(line3B1);
            mockStationReader.AddStation(alone1);
            mockStationReader.AddStation(alone2);
            mockStationReader.AddStation(alone3);
            
            return new Map(mockSystemsReader, mockStationReader);
        }

        static void GateBetween(SolarSystem a, SolarSystem b)
        {
            a.AddGateTo(b);
            b.AddGateTo(a);
        }

        public static Market CreateMarket()
        {
            ItemDatabase itemDatabase = TestObjectFactory.CreateItemDatabase();
            Map map = TestObjectFactory.CreateMap();
            return CreateMarket(itemDatabase, map);
        }

        public static Market CreateMarket(ItemDatabase itemDatabase, Map map)
        {
            Market market = new Market(itemDatabase, map);
            market.ReadFromFullPath(TestObjectFactory.TestDirectory + "Logs\\MarketTestInput - Kernite - 2007.01.28 172658.txt");
            market.ReadFromFullPath(TestObjectFactory.TestDirectory + "Logs\\MarketTestInput - Navitas - 2007.01.28 172734.txt");
            return market;
        }

        public static TradeFinder CreateTradeFinder()
        {
            Map map = TestObjectFactory.CreateMap();
            ItemDatabase database = TestObjectFactory.CreateItemDatabase();
            Market market = TestObjectFactory.CreateMarket(database, map);
            Parameters parameters = new Parameters(1000.0f, 100.0f, "Line1", TripType.SingleTrip);
            return MockTradeFinderFactory.CreateMock(map, market, TestObjectFactory.TestDirectory + "Logs", parameters);
        }

        public static MockTradeFinderFactory CreateMockTradeFinderFactory()
        {
            Map map = TestObjectFactory.CreateMap();
            ItemDatabase database = TestObjectFactory.CreateItemDatabase();
            Market market = TestObjectFactory.CreateMarket(database, map);
            Parameters parameters = new Parameters(1000.0f, 100.0f, "Line1", TripType.SingleTrip);
            return new MockTradeFinderFactory(map, market, TestObjectFactory.TestDirectory + "Logs", parameters);
        }
    }
}
