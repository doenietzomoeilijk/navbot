using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace EveMarketTool.Tests
{
    [TestFixture]
    public class ItemDatabaseTest
    {
        ItemDatabase database;
        ItemType kernite;
        ItemType navitas;

        [SetUp]
        public void CreateObjects()
        {
            database = TestObjectFactory.CreateItemDatabase();
            kernite = database.GetItemType("Kernite");
            navitas = database.GetItemType("Navitas");
        }

        [Test]
        public void DefaultConstructor()
        {
            database = new ItemDatabase();
            Assert.Greater(database.AllItems.Count, 0);
        }

        [Test]
        public void TestAllItems()
        {
            Assert.AreEqual(2, database.AllItems.Count);
        }

        [Test]
        public void TestKerniteById()
        {
            ItemType item = database.GetItemType(kernite.Id);
            AssertItemEqual(kernite, item);
        }

        [Test]
        public void TestKerniteByName()
        {
            ItemType item = database.GetItemType(kernite.Name);
            AssertItemEqual(kernite, item);
        }

        [Test]
        public void TestNavitasById()
        {
            ItemType item = database.GetItemType(navitas.Id);
            AssertItemEqual(navitas, item);
        }

        [Test]
        public void TestNavitasByName()
        {
            ItemType item = database.GetItemType(navitas.Name);
            AssertItemEqual(navitas, item);
        }

        [Test]
        public void TestItemNotFoundById()
        {
            ItemType item = database.GetItemType(-4144);
            Assert.IsNull(item);
        }

        [Test]
        public void TestItemNotFoundByName()
        {
            ItemType item = database.GetItemType("Not an item");
            Assert.IsNull(item);
        }

        void AssertItemEqual(ItemType expected, ItemType observed)
        {
            Assert.AreEqual(expected.Id, observed.Id);
            Assert.AreEqual(expected.Name, observed.Name);
            Assert.AreEqual(expected.Volume, observed.Volume);
        }
    }
}
