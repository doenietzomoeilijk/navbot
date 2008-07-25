using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace EveMarketTool.Tests
{
    [TestFixture]
    public class RoundTripTest
    {
        RoundTrip trip;
        RoundTrip badTrip;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            ItemDatabase database = TestObjectFactory.CreateItemDatabase();
            Map map = TestObjectFactory.CreateMap();
            SingleTrip there = new SingleTrip(map, map.GetStation(31), map.GetStation(11));
            there.AddPurchase(new Trade(database.GetItemType("Navitas"), 1000.0f, 1));
            there.Destination.AddItemWanted(new Trade(database.GetItemType("Navitas"), 1100.0f, 1));
            SingleTrip backAgain = new SingleTrip(map, map.GetStation(11), map.GetStation(31));
            backAgain.AddPurchase(new Trade(database.GetItemType("Kernite"), 300.0f, 50));
            backAgain.Destination.AddItemWanted(new Trade(database.GetItemType("Kernite"), 301.0f, 50));
            trip = new RoundTrip(there, backAgain);

            SingleTrip backWithNothing = new SingleTrip(map, map.GetStation(31), map.GetStation(41));
            badTrip = new RoundTrip(there, backWithNothing);
        }

        [SetUp]
        public void TestCaseSetUp()
        {
        }

        [Test]
        public void TestSecurity()
        {
            Assert.AreEqual(SecurityStatus.Level.LowSecShortcut, trip.Security);
        }

        [Test]
        public void TestProfit()
        {
            Assert.AreEqual(150.0f, trip.Profit);
        }

        [Test]
        public void TestProfitPerWarp()
        {
            Assert.AreEqual(150.0f / 22.0f, trip.ProfitPerWarp(true));
        }

        [Test]
        public void TestCompareByProfitPerWarp()
        {
            Assert.AreEqual(-1, RoundTrip.CompareByProfitPerWarpSecure(trip, badTrip));
            Assert.AreEqual(0, RoundTrip.CompareByProfitPerWarpSecure(trip, trip));
            Assert.AreEqual(1, RoundTrip.CompareByProfitPerWarpSecure(badTrip, trip));
        }
    }
}
