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

        public TradeList GetPurchases()
        {
            return purchases;
        }

        public ItemType Type
        {
            get { return purchases[0].Type; }
        }

        private float profitPerWarpFromStartingSystemSecure;
        private float profitPerWarpFromStartingSystemShortest;
        private int jumpsFromStartSecure;
        private int jumpsFromStartShortest;
        private SecurityStatus.Level securityFromStartingSystem;
        public SolarSystem StartingSystem
        {
            set 
            {
                jumpsFromStartSecure = map.DistanceBetween(value, source.System, true);
                jumpsFromStartShortest = map.DistanceBetween(value, source.System, false);

                profitPerWarpFromStartingSystemSecure = ProfitPerWarpCalc(jumpsFromStartSecure, Warps(true));
                profitPerWarpFromStartingSystemShortest = ProfitPerWarpCalc(jumpsFromStartShortest, Warps(false));

                securityFromStartingSystem = map.RouteSecurity(value, source.System);
            }
        }

        public float ProfitPerWarpCalc(int jumpsFromStart, int warpsForTrade)
        {
            // distance from a system is #jumps * 2 to warp there, and +1 to get to the right station
            int warps = warpsForTrade + (jumpsFromStart * 2 + 1);
            return Profit / warps;
        }

        public float ProfitPerWarpFrom(SolarSystem startingSystem, bool secure)
        {
            // distance from a system is #jumps * 2 to warp there, and +1 to get to the right station
            int warps = Warps(secure) + map.DistanceBetween(startingSystem, source.System, secure) * 2 + 1;
            return Profit / warps;
        }


        public float ProfitPerWarpFromStartingSystem(bool secure)
        {
            if (secure)
            {
                return profitPerWarpFromStartingSystemSecure;
            }
            else
            {
                return profitPerWarpFromStartingSystemShortest;
            }
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

        public float ProfitMargin
        {
            get
            {
                float profit = Profit;
                if ((Cost + profit) > 0)
                {
                    return (profit / (Cost + profit));
                }
                else
                {
                    return 0.0f;
                }
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

        public int Warps(bool secure)
        {
            int jumps = Jumps(secure);
            // if Jumps is effectively infinite, report this for Warps too
            if (jumps == int.MaxValue)
                return int.MaxValue;

            // each jump is: warp to gate, then jump.
            if (jumps > 0)
                return jumps * 2 + 1;
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

        public float ProfitPerWarp(bool secure)
        {
            int warps = Warps(secure);

            // #warps calulcated as: jump to the source system, jump to the source station 
            // (skip this if we're already in the system, although we may be in the wrong station - never mind)
            // then jump to the destination system and warp to the destination station
            if (warps == 0 && Profit > 0)
                return Profit;
            else
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

        private int jumpsSecure;
        private int jumpsShortest;
        public int Jumps(bool secure)
        {
            if (secure)
            {
                return jumpsSecure;
            }
            else
            {
                return jumpsShortest;
            }
        }

        private SecurityStatus.Level security;
        public SecurityStatus.Level Security
        {
            get
            {
                return security;
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

            jumpsSecure = map.DistanceBetween(source.System, destination.System, true);
            jumpsShortest = map.DistanceBetween(source.System, destination.System, false);

            if (jumpsSecure == jumpsShortest)
            {
                security = SecurityStatus.Level.HighSec;
            }
            else if ((jumpsSecure > jumpsShortest) && (jumpsSecure != int.MaxValue))
            {
                security = SecurityStatus.Level.LowSecShortcut;
            }
            else if ((jumpsSecure == int.MaxValue) && (jumpsShortest != int.MaxValue))
            {
                security = SecurityStatus.Level.LowSecOnly;
            }
            else
            {
                security = SecurityStatus.Level.LowSecOnly;
            }
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

        public string ToString(bool secure)
        {
            return TypeName + " from " + source.Name + " to " + destination.Name + "(" + Jumps(secure) + " jumps)" + " for " + Profit + " isk profit (" + ProfitPerWarp(secure) + " isk/warp)";
        }

        public static int CompareByProfitPerWarpFromStartingSystemSecure(SingleTrip a, SingleTrip b)
        {
            return b.profitPerWarpFromStartingSystemSecure.CompareTo(a.profitPerWarpFromStartingSystemSecure); // highest profit per warp comes first
        }

        public static int CompareByProfitPerWarpFromStartingSystemShortest(SingleTrip a, SingleTrip b)
        {
            return b.profitPerWarpFromStartingSystemShortest.CompareTo(a.profitPerWarpFromStartingSystemShortest); // highest profit per warp comes first
        }

        public static int CompareByProfitPerWarpSecure(SingleTrip a, SingleTrip b)
        {
            return b.ProfitPerWarp(true).CompareTo(a.ProfitPerWarp(true)); // highest profit per warp comes first
        }

        public static int CompareByProfitPerWarpShortest(SingleTrip a, SingleTrip b)
        {
            return b.ProfitPerWarp(false).CompareTo(a.ProfitPerWarp(false)); // highest profit per warp comes first
        }

        public static int CompareByProfit(SingleTrip a, SingleTrip b)
        {
            return b.Profit.CompareTo(a.Profit); // highest profit comes first
        }
    }
}
