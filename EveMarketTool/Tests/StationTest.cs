using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace EveMarketTool.Tests
{
    [TestFixture]
    public class StationTest
    {
        ItemDatabase database;
        Station station;
        ItemType kernite;
        ItemType navitas;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            database = TestObjectFactory.CreateItemDatabase();
            kernite = database.GetItemType("Kernite");
            navitas = database.GetItemType("Navitas");
        }

        [SetUp]
        public void TestCaseSetUp()
        {
            station = new Station(1, 0, "Station One", null);
        }

        [Test]
        public void TestAddItemForSale()
        {
            station.AddItemForSale(new Trade(kernite, 3.5f, 900));
            station.AddItemForSale(new Trade(kernite, 3.0f, 1000));
            station.AddItemForSale(new Trade(navitas, 1000.0f, 4));

            // cheapest item must come first
            Assert.AreEqual(2, station.ItemsForSale[kernite].Count);
            Assert.AreEqual(3.0f, station.ItemsForSale[kernite][0].UnitPrice);
            Assert.AreEqual(1000, station.ItemsForSale[kernite][0].Quantity);
            Assert.AreEqual(3.5f, station.ItemsForSale[kernite][1].UnitPrice);
            Assert.AreEqual(900, station.ItemsForSale[kernite][1].Quantity);

            Assert.AreEqual(1, station.ItemsForSale[navitas].Count);
            Assert.AreEqual(1000.0f, station.ItemsForSale[navitas][0].UnitPrice);
            Assert.AreEqual(4, station.ItemsForSale[navitas][0].Quantity);
        }

        [Test]
        public void TestAddItemWanted()
        {
            station.AddItemWanted(new Trade(kernite, 4.0f, 900));
            station.AddItemWanted(new Trade(kernite, 4.1f, 10));
            station.AddItemWanted(new Trade(navitas, 1100.0f, 4));
            station.AddItemWanted(new Trade(navitas, 33.3f, 333));

            // most expensive item must come first
            Assert.AreEqual(2, station.ItemsWanted[kernite].Count);
            Assert.AreEqual(4.1f, station.ItemsWanted[kernite][0].UnitPrice);
            Assert.AreEqual(10, station.ItemsWanted[kernite][0].Quantity);
            Assert.AreEqual(4.0f, station.ItemsWanted[kernite][1].UnitPrice);
            Assert.AreEqual(900, station.ItemsWanted[kernite][1].Quantity);

            Assert.AreEqual(2, station.ItemsWanted[navitas].Count);
            Assert.AreEqual(1100.0f, station.ItemsWanted[navitas][0].UnitPrice);
            Assert.AreEqual(4, station.ItemsWanted[navitas][0].Quantity);
            Assert.AreEqual(33.3f, station.ItemsWanted[navitas][1].UnitPrice);
            Assert.AreEqual(333, station.ItemsWanted[navitas][1].Quantity);
        }

        [Test]
        public void TestClearMarketData()
        {
            station.AddItemForSale(new Trade(kernite, 3.5f, 900));
            station.AddItemForSale(new Trade(kernite, 3.1f, 1000));
            station.AddItemWanted(new Trade(navitas, 33.3f, 333));

            station.ClearMarketData();
            Assert.AreEqual(0, station.ItemsForSale.Count);
            Assert.AreEqual(0, station.ItemsWanted.Count);
        }

        [Test]
        public void TestGetSaleValueOf()
        {
            station.AddItemWanted(new Trade(kernite, 4.0f, 900));
            station.AddItemWanted(new Trade(kernite, 4.1f, 10));

            Assert.AreEqual(41.0f, station.GetSaleValueOf(kernite, 10));
            Assert.AreEqual(41.0f+40.0f, station.GetSaleValueOf(kernite, 20));
            Assert.AreEqual(0.0f, station.GetSaleValueOf(kernite, 0));
            Assert.AreEqual(10*4.1f + 900*4.0f, station.GetSaleValueOf(kernite, 1000));
        }
    }
}
