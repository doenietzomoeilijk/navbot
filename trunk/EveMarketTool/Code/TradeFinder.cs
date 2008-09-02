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
            foreach (Station origin in market.StationsWithItemsForSale)
            {
                foreach (Station destination in market.StationsWithItemsWanted)
                {
                    TransactionList profitableTransactions = GetProfitableTransactions(origin, destination);

                    if (profitableTransactions.Count > 0)
                    {
                        profitableTransactions.Sort(TransactionList.DecreasingProfitPerCargo);

                        SingleTrip trip = GetBestTrade(origin, destination, profitableTransactions);
                        if (trip.Profit > 0.0f)
                        {
                            singleTrips.Add(trip);
                        }
                    }
                }
            }

            //singleTrips.Sort(SingleTrip.CompareByProfitPerWarp);
        }

        internal TransactionList GetProfitableTransactions(Station source, Station destination)
        {
            TransactionList list = new TransactionList();

            foreach (KeyValuePair<ItemType, TradeList> element in source.ItemsForSale)
            {
                if (destination.ItemsWanted.ContainsKey(element.Key))
                {
                    // Element 0 should have the highest buy price and lowest sell price respectively
                    if (destination.ItemsWanted[element.Key][0].UnitPrice > source.ItemsForSale[element.Key][0].UnitPrice)
                    {
                        // Get all the transactions for this item and station pair
                        list.AddRange(GetItemTransactionList(source, destination, element.Key));
                    }
                }
            }

            return list;
        }

        internal TransactionList GetItemTransactionList(Station source, Station destination, ItemType type)
        {
            TransactionList list = new TransactionList();

            Trade[] forSale = source.ItemsForSale[type].ToArray();
            Trade[] wanted = destination.ItemsWanted[type].ToArray();

            int buyIndex = 0;
            int sellIndex = 0;
            int buyAmount = 0;
            int sellAmount = 0;
            TransactionItem purchase = null;
            TransactionItem sale = null;
            Transaction currentTransaction = null;
            bool finished = false;
            int minQtyNeeded = 0;

            while (!finished)
            {
                // Source station has more than destination wants
                if ((forSale[buyIndex].Quantity - buyAmount) > (wanted[sellIndex].Quantity - sellAmount))
                {
                    // Set the amount (qty) of the transaction
                    int amount = (wanted[sellIndex].Quantity - sellAmount); 

                    // Create trades
                    purchase = new TransactionItem(forSale[buyIndex], amount);
                    if (minQtyNeeded > 0)
                    {
                        minQtyNeeded -= amount;
                    }
                    else
                    {
                        if (wanted[sellIndex].MinQuantity > amount)
                        {
                            sale = new TransactionItem(wanted[sellIndex]);
                            minQtyNeeded = wanted[sellIndex].MinQuantity - amount; 
                        }
                        else
                        {
                            sale = new TransactionItem(wanted[sellIndex], amount);
                        }
                    }

                    // Set the buy amount up by the amount that can be sold
                    buyAmount += amount;
                    // reset the sell amount
                    sellAmount = 0;
                    sellIndex++;
                }
                    // Source station has less than destination wants
                else if ((forSale[buyIndex].Quantity - buyAmount) < (wanted[sellIndex].Quantity - sellAmount))
                {
                    // Set the amount (qty) of the transaction
                    int amount = (forSale[buyIndex].Quantity - buyAmount);

                    // Create trades
                    purchase = new TransactionItem(forSale[buyIndex], amount);
                    if (minQtyNeeded > 0)
                    {
                        minQtyNeeded -= amount;
                    }
                    else
                    {
                        if (wanted[sellIndex].MinQuantity > amount)
                        {
                            sale = new TransactionItem(wanted[sellIndex]);
                            minQtyNeeded = wanted[sellIndex].MinQuantity - amount; 
                        }
                        else
                        {
                            sale = new TransactionItem(wanted[sellIndex], amount);
                        }
                    }

                    // Set the buy amount up by the amount that can be sold
                    sellAmount += amount;
                    // reset the buy amount
                    buyAmount = 0;
                    buyIndex++;
                }
                else
                {
                    // Set the amount (qty) of the transaction
                    int amount = (wanted[sellIndex].Quantity - sellAmount);

                    // Create trades
                    purchase = new TransactionItem(forSale[buyIndex], amount);
                    if (minQtyNeeded > 0)
                    {
                        minQtyNeeded -= amount;
                    }
                    else
                    {
                        if (wanted[sellIndex].MinQuantity > amount)
                        {
                            sale = new TransactionItem(wanted[sellIndex]);
                            minQtyNeeded = wanted[sellIndex].MinQuantity - amount;
                        }
                        else
                        {
                            sale = new TransactionItem(wanted[sellIndex], amount);
                        }
                    }


                    // Reset both buy and sell amount
                    buyAmount = 0;
                    sellAmount = 0;
                    buyIndex++;
                    sellIndex++;
                }

                if (currentTransaction == null)
                {
                    currentTransaction = new Transaction(purchase, sale);
                }
                else
                {
                    currentTransaction.AddPurchase(purchase);
                    if (sale != null)
                    {
                        currentTransaction.AddSale(sale);
                    }
                }

                purchase = null;
                sale = null;

                if ((wanted.Length <= sellIndex) ||
                    (forSale.Length <= buyIndex) ||
                    (wanted[sellIndex].UnitPrice <= forSale[buyIndex].UnitPrice))
                {
                    finished = true;
                }

                // If minimum quantity is achieved, add the transaction
                if ((finished) ||
                    ((minQtyNeeded <= 0) && 
                    (!((wanted[sellIndex].UnitPrice == wanted[Math.Max(sellIndex-1, 0)].UnitPrice) && 
                    (forSale[buyIndex].UnitPrice == forSale[Math.Max(buyIndex-1, 0)].UnitPrice)))
                    ))
                {
                    if (currentTransaction.Profit >= 0.0f)
                    {
                        list.Add(currentTransaction);
                    }
                    currentTransaction = null;
                    minQtyNeeded = 0;
                }

            }

            return list;
        }

        internal SingleTrip GetBestTrade(Station source, Station destination, TransactionList tradeList)
        {
            SingleTrip trade = new SingleTrip(map, source, destination);
            float iskLeft = parameters.Isk;
            float cargoSpaceLeft = parameters.CargoSpace;

            foreach (Transaction t in tradeList)
            {
                Transaction addTransaction = t.GetTransactionByLimits(iskLeft, cargoSpaceLeft);

                if (addTransaction != null)
                {
                    iskLeft -= addTransaction.Cost;
                    cargoSpaceLeft -= addTransaction.Volume;

                    trade.AddPurchase(addTransaction);
                }

                // We'll break out when isk or cargo is low (but not zero), to prevent looking for filler cargo.
                if ((cargoSpaceLeft == 3.0f) || (iskLeft == 10.0f))
                {
                    break;
                }
            }

            trade.Compress();

            return trade;
        }
        /*
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
         * */

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
