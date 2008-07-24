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
        SolarSystem highSec1;
        SolarSystem highSec2;
        SolarSystem highSec3;
        SolarSystem highSec4;
        SolarSystem highSec5;
        SolarSystem highSec6;
        SolarSystem deadEnd1;
        SolarSystem lowSec1;
        SolarSystem lowSec2;
        SolarSystem highSecIsolated1;
        SolarSystem isolated1;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
        }

        [SetUp]
        public void TestCaseSetUp()
        {
            map = TestObjectFactory.CreateMap();
            highSec1 = map.GetSystem("HighSec1");
            highSec2 = map.GetSystem("HighSec2");
            highSec3 = map.GetSystem("HighSec3");
            highSec4 = map.GetSystem("HighSec4");
            highSec5 = map.GetSystem("HighSec5");
            highSec6 = map.GetSystem("HighSec6");
            deadEnd1 = map.GetSystem("DeadEnd1");
            lowSec1 = map.GetSystem("LowSec1");
            lowSec2 = map.GetSystem("LowSec2");
            highSecIsolated1 = map.GetSystem("HighSecIsolated");
            isolated1 = map.GetSystem("Isolated1");
        }

        [Test]
        public void TestGetSystem()
        {
            Assert.AreEqual(highSec1, map.GetSystem("HighSec1"));
            Assert.IsNull(map.GetSystem("no such name"));
            Assert.IsNull(map.GetSystem(null));
        }

        [Test]
        public void TestDistanceBetween()
        {
            Assert.AreEqual(0, map.DistanceBetween(highSec1, highSec1, true));
            Assert.AreEqual(0, map.DistanceBetween(isolated1, isolated1, true));
            Assert.AreEqual(1, map.DistanceBetween(highSec1, highSec2, true));
            Assert.AreEqual(1, map.DistanceBetween(highSec2, highSec3, true));
            Assert.AreEqual(1, map.DistanceBetween(highSec2, deadEnd1, true));
            Assert.AreEqual(2, map.DistanceBetween(highSec1, highSec3, true));
            Assert.AreEqual(2, map.DistanceBetween(highSec1, deadEnd1, true));
            Assert.AreEqual(2, map.DistanceBetween(highSec3, deadEnd1, true));
            Assert.AreEqual(5, map.DistanceBetween(highSec1, highSec6, true));
            Assert.AreEqual(2, map.DistanceBetween(highSec1, highSec6, false));
            Assert.AreEqual(int.MaxValue, map.DistanceBetween(highSec1, isolated1, false));
            Assert.AreEqual(int.MaxValue, map.DistanceBetween(highSec1, isolated1, true));
        }
    }
}
