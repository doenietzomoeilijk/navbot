using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using EveMarketTool.Tests.Mock_Objects;

namespace EveMarketTool.Tests
{
    [TestFixture]
    public class MapTest
    {
        Map map;
        SolarSystem line1;
        SolarSystem line2;
        SolarSystem line3A;
        SolarSystem line3B;
        SolarSystem alone;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
        }

        [SetUp]
        public void TestCaseSetUp()
        {
            map = TestObjectFactory.CreateMap();
            line1 = map.GetSystem("Line1");
            line2 = map.GetSystem("Line2");
            line3A = map.GetSystem("Line3A");
            line3B = map.GetSystem("Line3B");
            alone = map.GetSystem("Alone");
        }

        [Test]
        public void TestGetSystem()
        {
            Assert.AreEqual(line1, map.GetSystem("Line1"));
            Assert.IsNull(map.GetSystem("no such name"));
            Assert.IsNull(map.GetSystem(null));
        }

        [Test]
        public void TestDistanceBetween()
        {
            Assert.AreEqual(0, map.DistanceBetween(line1, line1));
            Assert.AreEqual(0, map.DistanceBetween(alone, alone));
            Assert.AreEqual(1, map.DistanceBetween(line1, line2));
            Assert.AreEqual(1, map.DistanceBetween(line2, line3A));
            Assert.AreEqual(1, map.DistanceBetween(line2, line3B));
            Assert.AreEqual(2, map.DistanceBetween(line1, line3A));
            Assert.AreEqual(2, map.DistanceBetween(line1, line3B));
            Assert.AreEqual(2, map.DistanceBetween(line3A, line3B));
            Assert.AreEqual(int.MaxValue, map.DistanceBetween(alone, line1));
        }
    }
}
