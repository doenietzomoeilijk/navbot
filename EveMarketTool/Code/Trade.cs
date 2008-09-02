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

        private int minQuantity;
        public int MinQuantity
        {
            get { return minQuantity; }
        }

        private ItemType type;
        public ItemType Type
        {
            get { return type; }
        }

        public Trade(ItemType type, float unitPrice, int quantity) : 
            this(type, unitPrice, quantity, 0)
        {
        }

        public Trade(ItemType type, float unitPrice, int quantity, int minQuantity)
        {
            this.type = type;
            this.unitPrice = unitPrice;
            this.quantity = quantity;
            this.minQuantity = minQuantity;
        }

        public override string ToString()
        {
            return quantity + " " + type.Name + " at " + unitPrice + " each";
        }
    }
}
