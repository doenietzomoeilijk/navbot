using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public class TradeList : List<Trade>
    {
        public static int IncreasingUnitPrice(Trade a, Trade b)
        {
            return a.UnitPrice.CompareTo(b.UnitPrice);
        }

        public static int DecreasingUnitPrice(Trade a, Trade b)
        {
            return -IncreasingUnitPrice(a, b);
        }
    }
}
