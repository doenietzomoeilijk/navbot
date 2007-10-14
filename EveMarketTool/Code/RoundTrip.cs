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

        public float Security
        {
            get
            {
                return Math.Min(there.Security, backAgain.Security);
            }
        }

        public float ProfitPerWarp
        {
            get
            {
                int warps = there.Warps + backAgain.Warps;
                return Profit / warps;
            }
        }

        public float Profit
        {
            get
            {
                return there.Profit + backAgain.Profit;
            }
        }

        public RoundTrip(SingleTrip there, SingleTrip backAgain)
        {
            this.there = there;
            this.backAgain = backAgain;
        }

        public static int CompareByProfitPerWarp(RoundTrip a, RoundTrip b)
        {
            return b.ProfitPerWarp.CompareTo(a.ProfitPerWarp); // highest profit per warp comes first
        }

        public static int CompareByProfit(RoundTrip a, RoundTrip b)
        {
            return b.Profit.CompareTo(a.Profit); // highest profit comes first
        }
    }
}
