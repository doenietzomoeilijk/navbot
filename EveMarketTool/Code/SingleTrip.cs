using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public enum LimitingFactor { Availability, Isk, CargoSpace };

    public class SingleTrip
    {
        private TradeList purchases = new TradeList();
        public void AddPurchase(Trade t)
        {
            purchases.Add(t);
        }

        public ItemType Type
        {
            get { return purchases[0].Type; }
        }

        private float profitPerWarpFromStartingSystem;
        public SolarSystem StartingSystem
        {
            set { profitPerWarpFromStartingSystem = ProfitPerWarpFrom(value); }
        }

        public float ProfitPerWarpFromStartingSystem
        {
            get { return profitPerWarpFromStartingSystem; }
        }

        public float Profit
        {
            get
            {
                if (purchases.Count == 0) return 0.0f; // no trades = no profit

                ItemType type = purchases[0].Type;
                float saleValue = destination.GetSaleValueOf(type, Quantity);
                return saleValue - Cost;
            }
        }

        public float Cost
        {
            get
            {
                if (purchases.Count == 0) return 0.0f; // no trades = no cost

                float cost = 0.0f;
                ItemType type = purchases[0].Type;
                foreach (Trade t in purchases)
                {
                    cost += t.UnitPrice * t.Quantity;
                }
                return cost;
            }
        }

        public int Quantity
        {
            get
            {
                if (purchases.Count == 0) return 0; // no trades = no quantity

                int quantity = 0;
                ItemType type = purchases[0].Type;
                foreach (Trade t in purchases)
                {
                    quantity += t.Quantity;
                }
                return quantity;
            }
        }

        public int Warps
        {
            get
            {
                // if Jumps is effectively infinite, report this for Warps too
                if (Jumps == int.MaxValue)
                    return int.MaxValue;

                // each jump is: warp to gate, then jump.
                if (Jumps > 0)
                    return Jumps * 2 + 1;
                else
                {
                    if (source != destination)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        public float ProfitPerWarp
        {
            get
            {
                // #warps calulcated as: jump to the source system, jump to the source station 
                // (skip this if we're already in the system, although we may be in the wrong station - never mind)
                // then jump to the destination system and warp to the destination station
                if (Warps == 0 && Profit > 0)
                    return Profit;
                else
                    return Profit / Warps;
            }
        }

        public float ProfitPerWarpFrom(SolarSystem startingSystem)
        {
            // distance from a system is #jumps * 2 to warp there, and +1 to get to the right station
            int warps = Warps + map.DistanceBetween(startingSystem, source.System)*2+1;
            return Profit / warps;
        }

        public ItemType ItemType
        {
            get
            {
                if (purchases.Count == 0) return null;
                return purchases[0].Type;
            }
        }

        private int jumps;
        public int Jumps
        {
            get { return jumps; }
        }

        public float Security
        {
            get
            {
                return Math.Min(destination.System.Security, source.System.Security);
            }
        }

        Map map;
        Station source;
        public Station Source
        {
            get { return source; }
        }

        Station destination;
        public Station Destination
        {
            get { return destination; }
        }

        private LimitingFactor limitedBy;

        public LimitingFactor LimitedBy
        {
            get { return limitedBy; }
            set { limitedBy = value; }
        }

        public SingleTrip(Map map, Station source, Station destination)
        {
            this.map = map;
            this.source = source;
            this.destination = destination;
            jumps = map.DistanceBetween(source.System, destination.System);
        }

        public string TypeName
        {
            get
            {
                ItemType type = this.ItemType;
                string typeName;
                if (type == null)
                    typeName = "Nothing";
                else
                    typeName = type.Name;
                return typeName;
            }
        }

        public override string ToString()
        {
            return TypeName + " from " + source.Name + " to " + destination.Name + "(" + Jumps + " jumps)" + " for " + Profit + " isk profit (" + ProfitPerWarp + " isk/warp)";
        }

        public static int CompareByProfitPerWarpFromStartingSystem(SingleTrip a, SingleTrip b)
        {
            return b.profitPerWarpFromStartingSystem.CompareTo(a.profitPerWarpFromStartingSystem); // highest profit per warp comes first
        }

        public static int CompareByProfitPerWarp(SingleTrip a, SingleTrip b)
        {
            return b.ProfitPerWarp.CompareTo(a.ProfitPerWarp); // highest profit per warp comes first
        }

        public static int CompareByProfit(SingleTrip a, SingleTrip b)
        {
            return b.Profit.CompareTo(a.Profit); // highest profit comes first
        }
    }
}
