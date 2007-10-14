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

        public void SortByProfitPerWarp()
        {
            singleTrips.Sort(SingleTrip.CompareByProfitPerWarp);
            roundTrips.Sort(RoundTrip.CompareByProfitPerWarp);
        }

        public void SortByProfitPerWarpFromStartingSystem()
        {
            SolarSystem system = map.GetSystem(parameters.StartingSystem);
            if (system == null) throw new CannotFindStartingSystem();

            foreach (SingleTrip s in singleTrips)
            {
                s.StartingSystem = system;
            }

            singleTrips.Sort(SingleTrip.CompareByProfitPerWarpFromStartingSystem);
        }

        public void SortByProfit()
        {
            singleTrips.Sort(SingleTrip.CompareByProfit);
            roundTrips.Sort(RoundTrip.CompareByProfit);
        }

        public SingleTrip BestHighSecTrip()
        {
            foreach (SingleTrip trip in singleTrips)
            {
                if (trip.Security > 0.5)
                    return trip;
            }
            return null;
        }

        public SingleTrip BestLowSecTrip()
        {
            foreach (SingleTrip trip in singleTrips)
            {
                if (trip.Security <= 0.5)
                    return trip;
            }
            return null;
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

            singleTrips.Sort(SingleTrip.CompareByProfitPerWarp);
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
            roundTrips.Sort(RoundTrip.CompareByProfitPerWarp);
        }
    }
}
