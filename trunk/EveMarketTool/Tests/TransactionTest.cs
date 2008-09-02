using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace EveMarketTool.Tests
{
    [TestFixture]
    public class TransactionTest
    {
        ItemType sunshine;
        Trade p1;
        Trade p2;
        Trade s1;
        Trade s2;
        TransactionItem pt1;
        TransactionItem pt2;
        TransactionItem st1;
        TransactionItem st2;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            sunshine = new ItemType(1, "Sunshine", 1.5f);
            p1 = new Trade(sunshine, 1.0f, 100);
            p2 = new Trade(sunshine, 10.0f, 10);
            s1 = new Trade(sunshine, 5.0f, 100);
            s2 = new Trade(sunshine, 50.0f, 100);
            pt1 = new TransactionItem(p1);
            pt2 = new TransactionItem(p2);
            st1 = new TransactionItem(s1);
            st2 = new TransactionItem(s2);
        }

        [SetUp]
        public void TestCaseSetUp()
        {
        }

        [Test]
        public void TestIncreasingUnitPrice()
        {
        }

        [Test]
        public void TestAddSales()
        {
        }

        [Test]
        public void TestAddPurchases()
        {
        }

        [Test]
        public void TestMinimumAmounts()
        {
            Trade sale = new Trade(sunshine, 10.0f, 100, 60);
            Transaction t1 = new Transaction(p1, sale);
            // Quantity
            // Cost
            // Volume
        }

        [Test]
        public void TestProfitPerCargo()
        {
            Parameters param = new Parameters(10.0f, 10.0f, "none", TripType.SingleTrip, 0);
            Transaction t1 = new Transaction(p1, s1);
            Assert.AreEqual(100, t1.Units);
            Assert.AreEqual(100.0f, t1.Cost);
            Assert.AreEqual(150.0f, t1.Volume);
            Assert.AreEqual(395.0f, t1.Profit);

            param = new Parameters(10.0f, 10.0f, "none", TripType.SingleTrip, 5);
            Transaction t2 = new Transaction(p2, s2);
            Assert.AreEqual(10, t2.Units);
            Assert.AreEqual(100.0f, t2.Cost);
            Assert.AreEqual(15.0f, t2.Volume);
            Assert.AreEqual(397.5f, t2.Profit);
        }

        [Test]
        public void TestGetTransactionByLimits()
        {
            Transaction t1 = new Transaction(p1, s1);

            Transaction test1 = t1.GetTransactionByLimits(10.0f, 10000.0f);
            Assert.AreEqual(10, test1.Units);
            Assert.AreEqual(10.0f, test1.Cost);
            Assert.AreEqual(15.0f, test1.Volume);

            Transaction test2 = t1.GetTransactionByLimits(1000000.0f, 10.0f);
            Assert.AreEqual(6, test2.Units);
            Assert.AreEqual(6.0f, test2.Cost);
            Assert.AreEqual(9.0f, test2.Volume);
        }

        [Test]
        public void TestConstructors()
        {
            Parameters param = new Parameters(10.0f, 10.0f, "none", TripType.SingleTrip, 0);
            Transaction t1 = new Transaction(p1, s1);
            Transaction t2 = new Transaction(pt1, st1);
            Assert.AreEqual(t1.Purchases[0].TradeItem, t2.Purchases[0].TradeItem);
            Assert.AreEqual(t1.Sales[0].TradeItem, t2.Sales[0].TradeItem);
        }

        [Test]
        public void TestCombine()
        {
            Parameters param = new Parameters(10.0f, 10.0f, "none", TripType.SingleTrip, 0);
            TransactionItem pt1 = new TransactionItem(p1, 35);
            TransactionItem pt2 = new TransactionItem(p1, 25);
            TransactionItem st1 = new TransactionItem(s1, 35);
            TransactionItem st2 = new TransactionItem(s1, 25);

            Transaction t1 = new Transaction(pt1, st1);
            Transaction t2 = new Transaction(pt2, st2);

            t1.Combine(t2);

            Assert.AreEqual(60, t1.Purchases[0].Quantity);
            Assert.AreEqual(60, t1.Sales[0].Quantity);
        }
    }
}
