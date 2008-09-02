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
            // Some item is for sale at 8 stations
            StationList stations = market.StationsWithItemsForSale;
            Assert.AreEqual(8, stations.Count);
            foreach (Station s in map.StationList)
            {
                if (s.ItemsForSale.ContainsKey(kernite))
                {
                    Assert.Contains(s, stations);
                }
            }

        }

        [Test]
        public void TestStationsWithItemWanted()
        {
            // Some item is wanted at all 9 stations (due to regional sale areas)
            StationList stations = market.StationsWithItemsWanted;
            Assert.AreEqual(9, stations.Count);
            foreach (Station s in map.StationList)
            {
                if (s.ItemsWanted.ContainsKey(kernite))
                {
                    bool itemFound = stations.Contains(s);
                    if (!itemFound)
                    {
                        itemFound = s.System.ItemsWanted.ContainsKey(kernite);
                    }
                    Assert.IsTrue(itemFound);
                }
            }
        }

    }
}
