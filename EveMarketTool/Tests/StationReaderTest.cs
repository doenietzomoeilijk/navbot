using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace EveMarketTool.Tests
{
    [TestFixture]
    public class StationReaderTest
    {
        StationReader reader;
        Map map;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            map = TestObjectFactory.CreateMap();
        }

        [SetUp]
        public void TestCaseSetUp()
        {
            reader = new StationReader(map, TestObjectFactory.TestDirectory + "StationReaderTestInput.csv");
        }

        [Test]
        public void DefaultConstructor()
        {
            reader = new StationReader(new Map());
            Assert.Greater(reader.StationsById.Count, 0);
        }

        [Test]
        public void TestNumStations()
        {
            Assert.AreEqual(10, reader.StationsById.Count);
        }

        [Test]
        public void TestSystems()
        {
            Assert.AreSame(map.GetSystem(1), reader.StationsById[60000004].System);
            Assert.AreSame(map.GetSystem(2), reader.StationsById[60000007].System);
            Assert.AreSame(map.GetSystem(3), reader.StationsById[60000010].System);
            Assert.AreSame(map.GetSystem(4), reader.StationsById[60000013].System);
            Assert.AreSame(map.GetSystem(5), reader.StationsById[60000016].System);
            Assert.AreSame(map.GetSystem(1), reader.StationsById[60000019].System);
            Assert.AreSame(map.GetSystem(2), reader.StationsById[60000022].System);
            Assert.AreSame(map.GetSystem(3), reader.StationsById[60000025].System);
            Assert.AreSame(map.GetSystem(4), reader.StationsById[60000028].System);
            Assert.AreSame(map.GetSystem(5), reader.StationsById[60000031].System);
        }

        [Test]
        public void TestNames()
        {
            Assert.AreEqual("Muvolailen X - Moon 3 - CBD Corporation Storage", reader.StationsById[60000004].Name);
            Assert.AreEqual("Ono V - Moon 9 - CBD Corporation Storage", reader.StationsById[60000007].Name);
            Assert.AreEqual("Annaro I - CBD Corporation Storage", reader.StationsById[60000010].Name);
            Assert.AreEqual("Ono V - Moon 2 - CBD Corporation Storage", reader.StationsById[60000013].Name);
            Assert.AreEqual("Tasabeshi VIII - Moon 13 - CBD Corporation Storage", reader.StationsById[60000016].Name);
            Assert.AreEqual("Tasabeshi VI - Moon 1 - CBD Corporation Storage", reader.StationsById[60000019].Name);
            Assert.AreEqual("Ney X - Moon 15 - CBD Corporation Storage", reader.StationsById[60000022].Name);
            Assert.AreEqual("Ardene VIII - Moon 5 - CBD Corporation Storage", reader.StationsById[60000025].Name);
            Assert.AreEqual("Schoorasana VI - Moon 1 - CBD Corporation Storage", reader.StationsById[60000028].Name);
            Assert.AreEqual("Schoorasana VI - Moon 6 - CBD Corporation Storage", reader.StationsById[60000031].Name);
        }
    }
}
