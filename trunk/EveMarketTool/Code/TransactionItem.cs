using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public class TransactionItem
    {
        private Trade tradeItem;
        private int quantity;

        public Trade TradeItem
        {
            get
            {
                return tradeItem;
            }
            set
            {
                tradeItem = value;
            }
        }

        public int Quantity
        {
            get
            {
                if (tradeItem.Quantity < quantity)
                {
                    quantity = tradeItem.Quantity;
                }
                return quantity;
            }
            set
            {
                quantity = Math.Min(value, tradeItem.Quantity);
            }
        }

        public float UnitPrice
        {
            get
            {
                if (tradeItem != null)
                {
                    return tradeItem.UnitPrice;
                }
                else
                {
                    return 0.0f;
                }
            }
        }

        public int MinQuantity
        {
            get
            {
                if (tradeItem != null)
                {
                    return tradeItem.MinQuantity;
                }
                else
                {
                    return 0;
                }
            }
        }

        public ItemType Type
        {
            get
            {
                if (tradeItem != null)
                {
                    return tradeItem.Type;
                }
                else
                {
                    return null;
                }
            }
        }

        public TransactionItem(Trade tradeItem) : this(tradeItem, tradeItem.Quantity)
        {
        }

        public TransactionItem(Trade tradeItem, int quantity)
        {
            this.tradeItem = tradeItem;
            this.Quantity = quantity;
        }

        public override string ToString()
        {
            return 
                string.Format("<a href=\"showinfo:{0}\">{1}</a>", Type.Id, Type.Name) +
                string.Format(CultureInfo.InvariantCulture, ": {0}/{1} at {2} isk/ea - Total: {3:N1}", Quantity, tradeItem.Quantity, UnitPrice, Quantity * UnitPrice);
        }
    }
}
