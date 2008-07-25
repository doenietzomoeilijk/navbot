using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public class CannotFindStartingSystem : Exception
    {
    }

    public class TradeFinder
    {
        public Map map;
        Market market;
        Parameters parameters;
        SolarSystem startingSystem;

        public Parameters Parameters
        {
            get { return parameters; }
            set 
            { 
                parameters = value;
                singleTrips.Clear();
                roundTrips.Clear();
                if (parameters != null)
                {
                    AddProfitableTrades();
                    if(parameters.Trip == TripType.RoundTrip)
                        AddRoundTrips();
                }
            }
        }

        List<SingleTrip> singleTrips = new List<SingleTrip>();
        public List<SingleTrip> SingleTrips
        {
            get { return singleTrips; }
        }

        private List<RoundTrip> roundTrips = new List<RoundTrip>();
        public List<RoundTrip> RoundTrips
        {
            get { return roundTrips; }
        }

        public TradeFinder(Map map, Market market, Parameters parameters)
        {
            this.map = map;
            this.market = market;
            this.Parameters = parameters;
        }

        public TradeFinder(Map map, Market market)
        {
            this.map = map;
            this.market = market;
        }

        public void SortByProfitPerWarp(bool secure)
        {
            if (secure)
            {
                singleTrips.Sort(SingleTrip.CompareByProfitPerWarpSecure);
                roundTrips.Sort(RoundTrip.CompareByProfitPerWarpSecure);
            }
            else
            {
                singleTrips.Sort(SingleTrip.CompareByProfitPerWarpShortest);
                roundTrips.Sort(RoundTrip.CompareByProfitPerWarpSecure);
            }
        }

        public void SortByProfitPerWarpFromStartingSystem(bool secure)
        {
            SolarSystem system = map.GetSystem(parameters.StartingSystem);
            if (system == null) throw new CannotFindStartingSystem();

            if ((startingSystem == null) || (startingSystem != system))
            {
                foreach (SingleTrip s in singleTrips)
                {
                    s.StartingSystem = system;
                }

                startingSystem = system;
            }

            if (secure)
            {
                singleTrips.Sort(SingleTrip.CompareByProfitPerWarpFromStartingSystemSecure);
            }
            else
            {
                singleTrips.Sort(SingleTrip.CompareByProfitPerWarpFromStartingSystemShortest);
            }
        }

        public void SortByProfit()
        {
            singleTrips.Sort(SingleTrip.CompareByProfit);
            roundTrips.Sort(RoundTrip.CompareByProfit);
        }

        public List<SingleTrip> BestHighSecTrips(int top)
        {
            List<SingleTrip> list = new List<SingleTrip>();
            int count = 0;

            foreach (SingleTrip trip in singleTrips)
            {
                bool addRoute = true;
                if (trip.Security != SecurityStatus.Level.HighSec)
                {
                    addRoute = false;
                }
                if ((startingSystem != null) && (map.RouteSecurity(startingSystem, trip.Source.System) != SecurityStatus.Level.HighSec))
                {
                    addRoute = false;
                }

                if (addRoute)
                {
                    list.Add(trip);
                    count++;
                }

                if (count >= top)
                {
                    break;
                }
            }
            return list;
        }

        public List<SingleTrip> BestTrips(int top)
        {
            List<SingleTrip> list = new List<SingleTrip>();
            int count = 0;

            foreach (SingleTrip trip in singleTrips)
            {
                list.Add(trip);
                count++;

                if (count >= top)
                {
                    break;
                }
            }
            return list;
        }

        private void AddProfitableTrades()
        {
            foreach(KeyValuePair<ItemType, StationList> element in market.StationsWithItemForSale)
            {
                ItemType type = element.Key;
                StationList stationList = element.Value;
                foreach (Station source in stationList)
                {
                    foreach (Station destination in market.StationsWithItemWanted[type])
                    {
                        SingleTrip trade = GetBestTradeBetween(source, destination, type);
                        if (trade.Profit > 0.0f)
                            singleTrips.Add(trade);
                    }
                }
            }

            //singleTrips.Sort(SingleTrip.CompareByProfitPerWarp);
        }

        internal SingleTrip GetBestTradeBetween(Station source, Station destination, ItemType type)
        {
            SingleTrip trade = new SingleTrip(map, source, destination);
            float iskLeft = parameters.Isk;
            float cargoSpaceLeft = parameters.CargoSpace;

            Trade[] forSale = source.ItemsForSale[type].ToArray();
            Trade[] wanted = destination.ItemsWanted[type].ToArray();

            int si = 0;
            int quantityBought = 1; // just to get us into the loop; it's reset after that
            int quantityBoughtInTotal = 0;

            trade.LimitedBy = LimitingFactor.Availability;
            while (quantityBought > 0 && si < forSale.Length)
            {
                int quantityForSale = forSale[si].Quantity;
                int quantityCanAfford = (int)Math.Truncate(iskLeft/forSale[si].UnitPrice);
                int quantityCanTransport = (int)Math.Truncate(cargoSpaceLeft/forSale[si].Type.Volume);
                if (quantityCanAfford == 0 || quantityCanTransport == 0)
                    break;
                int quantityCanProfitablySell = GetProfitableSaleQuantity(forSale[si].UnitPrice, wanted, quantityBoughtInTotal);
                quantityBought = 0; // reset to zero for clarity
                quantityBought = Math.Min(quantityForSale, quantityCanAfford);
                if(forSale[si].Type.Volume>0)
                    quantityBought = Math.Min(quantityBought, quantityCanTransport);
                quantityBought = Math.Min(quantityBought, quantityCanProfitablySell);

                if (quantityBought == quantityCanAfford)
                {
                    trade.LimitedBy = LimitingFactor.Isk;
                }
                else if (quantityBought == quantityCanTransport)
                {
                    trade.LimitedBy = LimitingFactor.CargoSpace;
                }

                trade.AddPurchase(new Trade(type, forSale[si].UnitPrice, quantityBought));
                iskLeft -= quantityBought * forSale[si].UnitPrice;
                cargoSpaceLeft -= quantityBought * forSale[si].Type.Volume;
                quantityBoughtInTotal += quantityBought;

                ++si; // we've bought all we can from this for sale item; move to the next
            }

            return trade;
        }

        int GetProfitableSaleQuantity(float unitPricePaid, Trade[] wanted, int quantityAlreadyBought)
        {
            int quantityToIgnore = quantityAlreadyBought;
            int profitableQuantity = 0;
            for (int i = 0; i < wanted.Length; ++i)
            {
                int quantityRemoved = Math.Min(quantityToIgnore, wanted[i].Quantity);
                quantityToIgnore -= quantityRemoved;

                if (quantityToIgnore == 0) // not strictly necessary, but helpful for clarity
                {
                    if (wanted[i].UnitPrice > unitPricePaid)
                    {
                        profitableQuantity += wanted[i].Quantity - quantityRemoved;
                    }
                }
            }
            return profitableQuantity;
        }

        public SingleTrip GetBestSingleTrip(Station source, Station destination)
        {
            // single trips must be sorted (and iterated) in order of decreasing profit per warp
            foreach (SingleTrip trip in singleTrips)
            {
                if (trip.Source == source && trip.Destination==destination)
                {
                    return trip;
                }
            }

            return new SingleTrip(map, source, destination);
        }

        private void AddRoundTrips()
        {
            foreach (SingleTrip trade in singleTrips)
            {
                SingleTrip there = trade;
                SingleTrip backAgain = GetBestSingleTrip(there.Destination, there.Source);
                RoundTrip trip = new RoundTrip(there, backAgain);
                
                roundTrips.Add(trip);
            }
            roundTrips.Sort(RoundTrip.CompareByProfitPerWarpSecure);
        }
    }
}
