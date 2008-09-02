using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public class TransactionItemList : List<TransactionItem>
    {
        public static int IncreasingUnitPrice(TransactionItem a, TransactionItem b)
        {
            return a.TradeItem.UnitPrice.CompareTo(b.TradeItem.UnitPrice);
        }

        public static int DecreasingUnitPrice(TransactionItem a, TransactionItem b)
        {
            return -IncreasingUnitPrice(a, b);
        }
    }
}
