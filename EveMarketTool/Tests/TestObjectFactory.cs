using System;
using System.Collections.Generic;
using System.Text;
using EveMarketTool.Tests.Mock_Objects;

namespace EveMarketTool.Tests
{
    class TestObjectFactory
    {
        public static string TestDirectory = @"C:\dev\NavBot\EveMarketTool\Tests\";

        public static ItemDatabase CreateItemDatabase()
        {
            return new ItemDatabase(TestDirectory + "ItemDatabaseTestInput.csv");
        }

        public static Map CreateMap()
        {
            /* Solar system test map
             * 
             *               deadend1                  isolated1                 lowsec2 --- highsecIsolated
             *                  |                                                    |
             *  highsec1 --- highsec2 --- highsec3 --- highsec4 --- highsec5 --- highsec6 --- lowsec1
             *     |                                                                             |
             *     \-----------------------------------------------------------------------------/
             */

            SolarSystem highSec1 = new SolarSystem(1, "HighSec1", 0.93f);
            SolarSystem highSec2 = new SolarSystem(2, "HighSec2", 1.0f);
            SolarSystem highSec3 = new SolarSystem(3, "HighSec3", 0.45f);
            SolarSystem highSec4 = new SolarSystem(4, "HighSec4", 0.59f);
            SolarSystem highSec5 = new SolarSystem(5, "HighSec5", 0.65f);
            SolarSystem highSec6 = new SolarSystem(6, "HighSec6", 0.78f);
            SolarSystem deadEnd1 = new SolarSystem(7, "DeadEnd1", 0.9f);
            SolarSystem lowSec1 = new SolarSystem(8, "LowSec1", 0.44f);
            SolarSystem lowSec2 = new SolarSystem(9, "LowSec2", 0.34f);
            SolarSystem highSecIsolated1 = new SolarSystem(10, "HighSecIsolated", 0.78f);
            SolarSystem isolated1 = new SolarSystem(11, "Isolated1", 0.78f);

            GateBetween(highSec1, highSec2);
            GateBetween(highSec2, highSec3);
            GateBetween(highSec2, deadEnd1);
            GateBetween(highSec3, highSec4);
            GateBetween(highSec4, highSec5);
            GateBetween(highSec5, highSec6);
            GateBetween(highSec6, lowSec1);
            GateBetween(highSec6, lowSec2);
            GateBetween(highSecIsolated1, lowSec2);
            GateBetween(lowSec1, highSec1);

            Station highSec11 = new Station(11, 0, "HighSec1 Station1", highSec1);
            Station highSec12 = new Station(12, 0, "HighSec1 Station2", highSec1);

            Station deadEnd11 = new Station(21, 0, "DeadEnd1 Station1", deadEnd1);

            Station highSec61 = new Station(31, 0, "HighSec6 Station1", highSec6);
            Station highSec62 = new Station(32, 0, "HighSec6 Station2", highSec6);

            Station highSecIsolated11 = new Station(41, 0, "HighSecIsolated Station1", highSecIsolated1);

            Station isolated11 = new Station(51, 0, "Isolated1 Station1", isolated1);
            Station isolated12 = new Station(52, 0, "Isolated1 Station2", isolated1);
            Station isolated13 = new Station(53, 0, "Isolated1 Station3", isolated1);

            MockSolarSystemsReader mockSystemsReader = new MockSolarSystemsReader();
            mockSystemsReader.AddSystem(highSec1);
            mockSystemsReader.AddSystem(highSec2);
            mockSystemsReader.AddSystem(highSec3);
            mockSystemsReader.AddSystem(highSec4);
            mockSystemsReader.AddSystem(highSec5);
            mockSystemsReader.AddSystem(highSec6);
            mockSystemsReader.AddSystem(deadEnd1);
            mockSystemsReader.AddSystem(lowSec1);
            mockSystemsReader.AddSystem(lowSec2);
            mockSystemsReader.AddSystem(highSecIsolated1);
            mockSystemsReader.AddSystem(isolated1);
            MockStationReader mockStationReader = new MockStationReader();
            mockStationReader.AddStation(highSec11);
            mockStationReader.AddStation(highSec12);
            mockStationReader.AddStation(deadEnd11);
            mockStationReader.AddStation(highSec61);
            mockStationReader.AddStation(highSec62);
            mockStationReader.AddStation(highSecIsolated11);
            mockStationReader.AddStation(isolated11);
            mockStationReader.AddStation(isolated12);
            mockStationReader.AddStation(isolated13);
            
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
