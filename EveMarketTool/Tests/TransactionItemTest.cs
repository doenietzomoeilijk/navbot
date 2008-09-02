using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace EveMarketTool.Tests
{
    [TestFixture]
    public class TransactionItemTest
    {
        ItemType sunshine;
        Trade t1;
        Trade t2;
        TransactionItem trans;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            sunshine = new ItemType(1, "Sunshine", 1.0f);
            t1 = new Trade(sunshine, 100.0f, 100);
            t2 = new Trade(sunshine, 100.0f, 100);
        }

        [SetUp]
        public void TestCaseSetUp()
        {
            trans = new TransactionItem(t1);
        }

        [Test]
        public void TestPassThroughValues()
        {
            Assert.AreEqual(t1.Quantity, trans.Quantity);
            Assert.AreEqual(t1.MinQuantity, trans.MinQuantity);
            Assert.AreEqual(t1.Type, trans.Type);
            Assert.AreEqual(t1.UnitPrice, trans.UnitPrice);
        }

        [Test]
        public void TestTradeItem()
        {
            Assert.AreEqual(t1, trans.TradeItem);
            Assert.AreNotEqual(t2, trans.TradeItem);
        }

        [Test]
        public void TestQuantity()
        {
            Assert.AreEqual(100, trans.Quantity);
            trans.Quantity += 50;
            Assert.AreEqual(100, trans.Quantity);
            trans.Quantity -= 50;
            Assert.AreEqual(50, trans.Quantity);
            trans.Quantity = 100;
            Assert.AreEqual(100, trans.Quantity);
        }

        [Test]
        public void TestConstructor()
        {
            Trade trade = new Trade(sunshine, 100.0f, 100);
            TransactionItem t1 = new TransactionItem(trade);
            TransactionItem t2 = new TransactionItem(trade, 50);
            TransactionItem t3 = new TransactionItem(trade, 150);
            Assert.AreEqual(100, t1.Quantity);
            Assert.AreEqual(50, t2.Quantity);
            Assert.AreEqual(100, t3.Quantity);
        }
    }
}
