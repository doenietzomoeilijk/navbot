using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public class TransactionList : List<Transaction>
    {
        public static int DecreasingProfitPerCargo(Transaction a, Transaction b)
        {
            return -(a.ProfitPerCargo.CompareTo(b.ProfitPerCargo));
        }

        /*
        public static int DecreasingUnitPrice(Trade a, Trade b)
        {
            return -IncreasingUnitPrice(a, b);
        }
         * */
    }
}
