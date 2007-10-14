using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace EveMarketTool.Tests
{
    [TestFixture]
    public class MarketTest
    {
        Market market;
        ItemDatabase itemDatabase;
        Map map;
        ItemType kernite;
        ItemType navitas;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            itemDatabase = TestObjectFactory.CreateItemDatabase();
            map = TestObjectFactory.CreateMap();
            market = TestObjectFactory.CreateMarket(itemDatabase, map);
            kernite = itemDatabase.GetItemType("Kernite");
            navitas = itemDatabase.GetItemType("Navitas");
        }

        [SetUp]
        public void TestCaseSetUp()
        {
        }

        [Test]
        public void TestStationsWithItemForSale()
        {
            // Kernite is sold at every station but one
            StationList stations = market.StationsWithItemForSale[kernite];
            Assert.AreEqual(map.StationList.Count -1, stations.Count);
            foreach (Station s in map.StationList)
            {
                if(s.ItemsForSale.ContainsKey(kernite))
                    Assert.Contains(s, stations);
            }

            // The Navitas is sold at three stations - 11, 12 and 41
            stations = market.StationsWithItemForSale[navitas];
            Assert.AreEqual(3, stations.Count);
            Assert.Contains(map.GetStation(11), stations);
            Assert.Contains(map.GetStation(12), stations);
            Assert.Contains(map.GetStation(41), stations);
        }

        [Test]
        public void TestStationsWithItemWanted()
        {
            // Kernite is wanted at every station but one
            StationList stations = market.StationsWithItemWanted[kernite];
            Assert.AreEqual(map.StationList.Count - 1, stations.Count);
            foreach (Station s in map.StationList)
            {
                if (s.ItemsWanted.ContainsKey(kernite))
                    Assert.Contains(s, stations);
            }

            // The Navitas is wanted at one station - 31
            stations = market.StationsWithItemWanted[navitas];
            Assert.AreEqual(1, stations.Count);
            Assert.Contains(map.GetStation(31), stations);
        }
    }
}
