using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public class Trade
    {
        private float unitPrice;
        public float UnitPrice
        {
            get { return unitPrice; }
        }

        private int quantity;
        public int Quantity
        {
            get { return quantity; }
        }

        private ItemType type;
        public ItemType Type
        {
            get { return type; }
        }

        public Trade(ItemType type, float unitPrice, int quantity)
        {
            this.type = type;
            this.unitPrice = unitPrice;
            this.quantity = quantity;
        }

        public override string ToString()
        {
            return quantity + " " + type.Name + " at " + unitPrice + " each";
        }
    }
}
