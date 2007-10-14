using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace EveMarketTool.Tests
{
    [TestFixture]
    public class SingleTripTest
    {
        SingleTrip trip;
        SingleTrip empty;
        Map map;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            ItemDatabase database = TestObjectFactory.CreateItemDatabase();
            map = TestObjectFactory.CreateMap();
            trip = new SingleTrip(map, map.GetStation(41), map.GetStation(31));
            trip.AddPurchase(new Trade(database.GetItemType("Navitas"), 1000.0f, 3));
            trip.Destination.AddItemWanted(new Trade(database.GetItemType("Navitas"), 1100.0f, 3));

            empty = new SingleTrip(map, map.GetStation(11), map.GetStation(52));
        }

        [SetUp]
        public void TestCaseSetUp()
        {
        }

        [Test]
        public void TestProfit()
        {
            Assert.AreEqual(300.0f, trip.Profit);
            Assert.AreEqual(0.0f, empty.Profit);
        }

        [Test]
        public void TestProfitPerWarp()
        {
            Assert.AreEqual(300.0f/5.0f, trip.ProfitPerWarp);
            Assert.AreEqual(0.0f, empty.Profit);
        }

        [Test]
        public void TestProfitPerWarpFrom()
        {
            Assert.AreEqual(300.0f/10.0f, trip.ProfitPerWarpFrom(map.GetSystem(1)));
            Assert.AreEqual(0.0f, empty.ProfitPerWarpFrom(map.GetSystem(2)));
        }

        [Test]
        public void TestJumps()
        {
            Assert.AreEqual(2, trip.Jumps);
            Assert.AreEqual(int.MaxValue, empty.Jumps);
        }

        [Test]
        public void TestWarps()
        {
            Assert.AreEqual(5, trip.Warps);
            Assert.AreEqual(int.MaxValue, empty.Warps);
        }

        [Test]
        public void TestSecurity()
        {
            Assert.AreEqual(0.7f, trip.Security);
            Assert.AreEqual(0.0f, empty.Security);
        }

        [Test]
        public void TestCost()
        {
            Assert.AreEqual(3000.0f, trip.Cost);
            Assert.AreEqual(0.0f, empty.Cost);
        }

        [Test]
        public void TestQuantity()
        {
            Assert.AreEqual(3, trip.Quantity);
            Assert.AreEqual(0, empty.Quantity);
        }

        [Test]
        public void TestTypeName()
        {
            Assert.AreEqual("Navitas", trip.TypeName);
            Assert.AreEqual("Nothing", empty.TypeName);
        }
    }
}
