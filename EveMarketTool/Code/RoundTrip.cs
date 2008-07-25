using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public class RoundTrip
    {
        private SingleTrip there;
        public SingleTrip There
        {
            get { return there; }
        }

        private SingleTrip backAgain;
        public SingleTrip BackAgain
        {
            get { return backAgain; }
        }

        public SecurityStatus.Level Security
        {
            get
            {
                return SecurityStatus.Min(there.Security, backAgain.Security);
            }
        }

        public float ProfitPerWarpFromStartingSystemSecure(bool secure)
        {
            return there.ProfitPerWarpFromStartingSystem(secure) + backAgain.ProfitPerWarp(secure);
        }

        public float ProfitPerWarp(bool secure)
        {
            int warps = there.Warps(secure) + backAgain.Warps(secure);
            return Profit / warps;
        }

        public float Profit
        {
            get
            {
                return there.Profit + backAgain.Profit;
            }
        }

        public float Cost
        {
            get
            {
                return there.Cost + backAgain.Cost;
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


        public RoundTrip(SingleTrip there, SingleTrip backAgain)
        {
            this.there = there;
            this.backAgain = backAgain;
        }

        public static int CompareByProfitPerWarpFromStartingSystemSecure(RoundTrip a, RoundTrip b)
        {
            return b.ProfitPerWarpFromStartingSystemSecure(true).CompareTo(a.ProfitPerWarpFromStartingSystemSecure(true)); // highest profit per warp comes first
        }

        public static int CompareByProfitPerWarpFromStartingSystemShortest(RoundTrip a, RoundTrip b)
        {
            return b.ProfitPerWarpFromStartingSystemSecure(false).CompareTo(a.ProfitPerWarpFromStartingSystemSecure(false)); // highest profit per warp comes first
        }

        public static int CompareByProfitPerWarpSecure(RoundTrip a, RoundTrip b)
        {
            return b.ProfitPerWarp(true).CompareTo(a.ProfitPerWarp(true)); // highest profit per warp comes first
        }

        public static int CompareByProfitPerWarpShortest(RoundTrip a, RoundTrip b)
        {
            return b.ProfitPerWarp(false).CompareTo(a.ProfitPerWarp(false)); // highest profit per warp comes first
        }

        public static int CompareByProfit(RoundTrip a, RoundTrip b)
        {
            return b.Profit.CompareTo(a.Profit); // highest profit comes first
        }
    }
}
