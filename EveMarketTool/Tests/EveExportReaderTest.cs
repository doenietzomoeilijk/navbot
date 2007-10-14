using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace EveMarketTool.Tests
{
    [TestFixture]
    public class EveExportReaderTest
    {
        EveExportReaderTestWrapper reader;

        [TestFixtureSetUp]
        public void CreateReader()
        {
            reader = new EveExportReaderTestWrapper("EveExportReaderItemsTestInput.csv");
        }

        [Test]
        public void SimpleFieldsCorrect()
        {
            Assert.Greater(reader.Fields.Count, 0);
            Dictionary<string, string> first = reader.Fields[0];
            Assert.AreEqual("20", first["typeID"]);
            Assert.AreEqual("Kernite", first["typeName"]);
            Assert.AreEqual("Kernite is a fairly common ore type that yields the largest amount of mexallon of any ore types in the world. Besides mexallon the Kernite also has a bit of tritanium and isogen. 400 units of ore are needed for refining.", first["description"]);
        }

        [Test]
        public void NumbersConverted()
        {
            Assert.AreEqual(1270, reader.ParseId("1.270"));
            Assert.AreEqual(1234.56, reader.ParseNumber("1.234,56"));
        }

        [Test]
        public void ComplexFieldsCorrect()
        {
            Assert.Greater(reader.Fields.Count, 1);
            Dictionary<string, string> second = reader.Fields[1];
            Assert.AreEqual("592", second["typeID"]);
            Assert.AreEqual("Navitas", second["typeName"]);
            Assert.AreEqual("The Navitas is a solid mining vessel; in wide use by independent excavators. It is also one of the best ships available for budding traders or even scavengers. The long-range scanners and sturdy outer shell of the ship help to protect the ship from harassment.\r\n\r\nSpecial Ability: 5% \"bonus\" to cargo capacity and 20% bonus to mining laser yield per level. -60% mining laser capacitor use", second["description"]);
        }

        [Test]
        public void AllItemsRead()
        {
            Assert.AreEqual(reader.Fields.Count, 2);
        }
    }

    class EveExportReaderTestWrapper : EveExportReader
    {
        public List<Dictionary<string, string>> Fields = new List<Dictionary<string, string>>();

        public EveExportReaderTestWrapper(string inputFile)
        {
            ReadFromResource("Tests." + inputFile);
        }

        protected override void InterpretRow(Dictionary<string, string> fields)
        {
            Fields.Add(new Dictionary<string, string>(fields));
        }
    }
}
