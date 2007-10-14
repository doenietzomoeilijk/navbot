using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace EveMarketTool.Tests
{
    [TestFixture]
    public class TradeFinderTest
    {
        TradeFinder finder;
        Map map;
        ItemDatabase database;
        Market market;
        Parameters parameters;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            map = TestObjectFactory.CreateMap();
            database = TestObjectFactory.CreateItemDatabase();
            market = TestObjectFactory.CreateMarket(database, map);
        }

        [SetUp]
        public void TestCaseSetUp()
        {
        }

        [Test]
        public void BestSingleTripLimitedByCargo()
        {
            /* the best route where money is no object but cargo space is, is:
             * 83 Kernite from Line 2 station 1 (sec 0.8) to Line 3B station 1 (sec 0.7)
             */
            parameters = new Parameters(10000000.0f, 100.0f, null, TripType.SingleTrip);
            finder = new TradeFinder(map, market, parameters);
            finder.SortByProfitPerWarp();
            Assert.Greater(finder.SingleTrips.Count, 0);
            SingleTrip best = finder.SingleTrips[0];
            Assert.AreEqual(LimitingFactor.CargoSpace, best.LimitedBy);
            Assert.AreSame(map.GetStation(21), best.Source);
            Assert.AreSame(map.GetStation(41), best.Destination);
            Assert.AreSame(database.GetItemType("Kernite"), best.ItemType);
            Assert.AreEqual(1, best.Jumps);
            Assert.AreEqual(3, best.Warps);
            Assert.AreEqual(0.7f, best.Security);
            Assert.AreEqual(83, best.Quantity);  // Kernite is 1.2 vol, so we can only fit 83 in our hold
            float buyFor = 83.0f * 305.52f;
            float sellFor = 83.0f * 352.9f;
            Assert.AreEqual(buyFor, best.Cost);
            Assert.AreEqual(sellFor - buyFor, best.Profit);
            Assert.AreEqual((sellFor - buyFor)/3.0f, best.ProfitPerWarp);
        }

        [Test]
        public void BestSingleTripLimitedByIsk()
        {
            /* the best route in which money is the limiting factor is:
             * 83 Kernite from Line 2 station 1 (sec 0.8) to Line 3B station 1 (sec 0.7)
             */
            parameters = new Parameters(1000.0f, 10000000.0f, null, TripType.SingleTrip);
            finder = new TradeFinder(map, market, parameters);
            finder.SortByProfitPerWarp();
            Assert.Greater(finder.SingleTrips.Count, 0);
            SingleTrip best = finder.SingleTrips[0];
            Assert.AreEqual(LimitingFactor.Isk, best.LimitedBy);
            Assert.AreSame(map.GetStation(21), best.Source);
            Assert.AreSame(map.GetStation(41), best.Destination);
            Assert.AreSame(database.GetItemType("Kernite"), best.ItemType);
            Assert.AreEqual(1, best.Jumps);
            Assert.AreEqual(3, best.Warps);
            Assert.AreEqual(0.7f, best.Security);
            Assert.AreEqual(3, best.Quantity);  // can only affort 3 Kernite
            float buyFor = 3.0f * 305.52f;
            float sellFor = 3.0f * 352.9f;
            Assert.AreEqual(buyFor, best.Cost);
            Assert.AreEqual(sellFor - buyFor, best.Profit);
            Assert.AreEqual((sellFor - buyFor) / 3.0f, best.ProfitPerWarp);
        }

        [Test]
        public void BestSingleTripLimitedByAvailability()
        {
            /* the best route in which money and cargo space are no object is:
             * 83 Kernite from Line 2 station 1 (sec 0.8) to Line 3B station 1 (sec 0.7)
             */
            parameters = new Parameters(10000000.0f, 10000000.0f, null, TripType.SingleTrip);
            finder = new TradeFinder(map, market, parameters);
            finder.SortByProfitPerWarp();
            Assert.Greater(finder.SingleTrips.Count, 0);
            SingleTrip best = finder.SingleTrips[0];
            Assert.AreEqual(LimitingFactor.Availability, best.LimitedBy);
            Assert.AreSame(map.GetStation(31), best.Source);
            Assert.AreSame(map.GetStation(32), best.Destination);
            Assert.AreSame(database.GetItemType("Kernite"), best.ItemType);
            Assert.AreEqual(0, best.Jumps);
            Assert.AreEqual(1, best.Warps);
            Assert.AreEqual(0.8f, best.Security);
            Assert.AreEqual(7487, best.Quantity);  // 7487 Kernite wanted at 32
            float buyFor = 7487.0f * 344.45f;
            float sellFor = 7487.0f * 352.62f;
            Assert.AreEqual(buyFor, best.Cost);
            Assert.AreEqual(sellFor - buyFor, best.Profit);
            Assert.AreEqual((sellFor - buyFor) / 1.0f, best.ProfitPerWarp);
        }

        [Test]
        public void FullCargoRoundTrip()
        {
            parameters = new Parameters(10000000.0f, 10000000.0f, null, TripType.RoundTrip);
            finder = new TradeFinder(map, market, parameters);
            finder.SortByProfitPerWarp();
            Assert.Greater(finder.RoundTrips.Count, 2);

            // There could be two trips reported in either order - from 41 to 31 and back, 
            // or from 31 to 41 and back. We don't care about the order, so just pick one
            RoundTrip one = finder.RoundTrips[1];
            RoundTrip two = finder.RoundTrips[2];
            RoundTrip trip;
            if (one.There.ItemType == database.GetItemType("Navitas"))
                trip = one;
            else
                trip = two;

            Assert.AreSame(database.GetItemType("Navitas"), trip.There.ItemType);
            Assert.AreSame(map.GetStation(41), trip.There.Source);
            Assert.AreSame(map.GetStation(31), trip.There.Destination);

            Assert.AreSame(database.GetItemType("Kernite"), trip.BackAgain.ItemType);
            Assert.AreSame(map.GetStation(31), trip.BackAgain.Source);
            Assert.AreSame(map.GetStation(41), trip.BackAgain.Destination);

            Assert.AreEqual(2, trip.There.Jumps);
            Assert.AreEqual(5, trip.There.Warps);
            Assert.AreEqual(2, trip.BackAgain.Jumps);
            Assert.AreEqual(5, trip.BackAgain.Warps);
            Assert.AreEqual(0.7f, trip.There.Security);
            Assert.AreEqual(0.7f, trip.BackAgain.Security);

            float quantityThere = 10;
            float quantityBackAgain = 7487.0f;
            float buyForThere = 112500.0f * quantityThere;
            float sellForThere = 118450.0f * quantityThere;
            float buyForBackAgain = 344.45f * quantityBackAgain;
            float sellForBackAgain = 352.9f * quantityBackAgain;

            Assert.AreEqual(buyForThere, trip.There.Cost);
            Assert.AreEqual(buyForBackAgain, trip.BackAgain.Cost);

            float profit = sellForThere + sellForBackAgain - buyForThere - buyForBackAgain;
            Assert.AreEqual(profit, trip.Profit);
            Assert.AreEqual(profit / 10.0f, trip.ProfitPerWarp);
        }

        [Test]
        public void BestHighSecTrip()
        {
            parameters = new Parameters(10000000.0f, 10000000.0f, null, TripType.SingleTrip);
            finder = new TradeFinder(map, market, parameters);
            finder.SortByProfitPerWarp();
            Assert.Greater(finder.SingleTrips.Count, 0);
            SingleTrip best = finder.BestHighSecTrip();
            Assert.Greater(best.Security, 0.5);
        }

        [Test]
        public void BestLowSecTrip()
        {
            parameters = new Parameters(10000000.0f, 10000000.0f, null, TripType.SingleTrip);
            finder = new TradeFinder(map, market, parameters);
            finder.SortByProfitPerWarp();
            Assert.Greater(finder.SingleTrips.Count, 0);
            SingleTrip best = finder.BestLowSecTrip();
            Assert.Less(best.Security, 0.501);
        }

        [Test]
        public void SortByProfitPerWarpFromHere()
        {
            parameters = new Parameters(10000000.0f, 10000000.0f, "Line3B", TripType.SingleTrip);
            finder = new TradeFinder(map, market, parameters);
            finder.SortByProfitPerWarpFromStartingSystem();
            Assert.Greater(finder.SingleTrips.Count, 3);
            SingleTrip one = finder.SingleTrips[1];
            SingleTrip two = finder.SingleTrips[2];
            Assert.Greater(one.ProfitPerWarpFromStartingSystem, two.ProfitPerWarpFromStartingSystem);
            Assert.Less(one.ProfitPerWarp, two.ProfitPerWarp);
        }

        [Test]
        public void SetParameters()
        {
            finder = new TradeFinder(map, market);
            Assert.AreEqual(0, finder.SingleTrips.Count);
            Assert.AreEqual(0, finder.RoundTrips.Count);

            finder.Parameters = new Parameters(10000000.0f, 10000000.0f, null, TripType.RoundTrip);
            Assert.AreEqual(19, finder.RoundTrips.Count);

            finder.Parameters = new Parameters(1.0f, 1.0f, null, TripType.RoundTrip);
            Assert.AreEqual(0, finder.RoundTrips.Count);

            finder.Parameters = new Parameters(10000000.0f, 10000000.0f, null, TripType.SingleTrip);
            Assert.AreEqual(19, finder.SingleTrips.Count);

            finder.Parameters = null; ;
            Assert.AreEqual(0, finder.SingleTrips.Count);
            Assert.AreEqual(0, finder.RoundTrips.Count);
        }

        [Test]
        public void ItemWithZeroVolume()
        {
            parameters = new Parameters(10000000.0f, 1.0f, null, TripType.SingleTrip);
            finder = new TradeFinder(map, market, parameters);

            Station source = map.StationList[0];
            Station destination = source;

            ItemType blueprint = new ItemType(12297, "Blueprint", 0.0f);
            TradeList buyList = new TradeList();
            buyList.Add(new Trade(blueprint, 100.0f, 1000));
            source.ItemsWanted.Add(blueprint, buyList);
            TradeList sellList = new TradeList();
            sellList.Add(new Trade(blueprint, 90.0f, 1000));
            source.ItemsForSale.Add(blueprint, sellList);

            SingleTrip trip = finder.GetBestTradeBetween(source, destination, blueprint);
            Assert.AreEqual(1000, trip.Quantity);
            Assert.AreEqual(10 * 1000, trip.Profit);
        }
    }
}
