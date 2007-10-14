using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace EveMarketTool.Tests
{
    [TestFixture]
    public class TradeListTest
    {
        ItemType sunshine;
        Trade cheap;
        Trade expensive;
        Trade oneCheap;
        Trade oneExpensive;
        Trade zero;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            sunshine = new ItemType(1, "Sunshine", 1.0f);
            cheap = new Trade(sunshine, 1.0f, 1000);
            expensive = new Trade(sunshine, 1000.0f, 1000);
            oneCheap = new Trade(sunshine, 1.0f, 1);
            oneExpensive = new Trade(sunshine, 1000.0f, 1);
            zero = new Trade(null, 0.0f, 0);
        }

        [SetUp]
        public void TestCaseSetUp()
        {
        }

        [Test]
        public void TestIncreasingUnitPrice()
        {
            Assert.AreEqual(-1, TradeList.IncreasingUnitPrice(cheap, expensive));
            Assert.AreEqual(0, TradeList.IncreasingUnitPrice(cheap, cheap));
            Assert.AreEqual(1, TradeList.IncreasingUnitPrice(expensive, cheap));
            Assert.AreEqual(-1, TradeList.IncreasingUnitPrice(zero, cheap));
            Assert.AreEqual(0, TradeList.IncreasingUnitPrice(zero, zero));
            Assert.AreEqual(1, TradeList.IncreasingUnitPrice(expensive, zero));
            Assert.AreEqual(-1, TradeList.IncreasingUnitPrice(cheap, oneExpensive));
            Assert.AreEqual(0, TradeList.IncreasingUnitPrice(oneExpensive, expensive));
            Assert.AreEqual(1, TradeList.IncreasingUnitPrice(expensive, oneCheap));
        }

        [Test]
        public void TestDecreasingUnitPrice()
        {
            Assert.AreEqual(1, TradeList.DecreasingUnitPrice(cheap, expensive));
            Assert.AreEqual(0, TradeList.DecreasingUnitPrice(cheap, cheap));
            Assert.AreEqual(-1, TradeList.DecreasingUnitPrice(expensive, cheap));
            Assert.AreEqual(1, TradeList.DecreasingUnitPrice(zero, cheap));
            Assert.AreEqual(0, TradeList.DecreasingUnitPrice(zero, zero));
            Assert.AreEqual(-1, TradeList.DecreasingUnitPrice(expensive, zero));
            Assert.AreEqual(1, TradeList.DecreasingUnitPrice(cheap, oneExpensive));
            Assert.AreEqual(0, TradeList.DecreasingUnitPrice(oneExpensive, expensive));
            Assert.AreEqual(-1, TradeList.DecreasingUnitPrice(expensive, oneCheap));
        }
    }
}
