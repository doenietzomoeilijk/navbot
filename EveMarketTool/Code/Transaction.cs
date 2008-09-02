using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public class Transaction
    {
        private TransactionItemList purchases;
        public TransactionItemList Purchases
        {
            get 
            { 
                return purchases; 
            }
        }

        private TransactionItemList sales;
        public TransactionItemList Sales
        {
            get
            {
                return sales;
            }
        }

        private int units;
        private float salePricePerUnit;
        private float purchasePricePerUnit;
        private int minQty;
        private bool upToDate;
        private float profitPerItem;
        private float profitAmount;
        private float salePrice;
        private float purchasePrice;
        private float taxAmount;

        public int Units
        {
            get
            {
                if (!upToDate)
                {
                    UpdateValues();
                }

                return units;
            }
        }

        public float Cost
        {
            get
            {
                if (!upToDate)
                {
                    UpdateValues();
                }

                return purchasePrice;
            }
        }

        public int MinimumQuantity
        {
            get
            {
                if (!upToDate)
                {
                    UpdateValues();
                }

                return minQty;
            }
        }

        public float Profit
        {
            get
            {
                if (!upToDate)
                {
                    UpdateValues();
                }

                return profitAmount;
            }
        }

        public float Tax
        {
            get
            {
                if (!upToDate)
                {
                    UpdateValues();
                }

                return taxAmount;
            }
        }

        public float MinimumVolume
        {
            get
            {
                return type.Volume * MinimumQuantity;
            }
        }

        public float Volume
        {
            get
            {
                return type.Volume * Units;
            }
        }

        public float MinimumCost
        {
            get
            {
                return type.Volume * MinimumQuantity;
            }
        }

        public float ProfitPerCargo
        {
            get
            {
                if (!upToDate)
                {
                    UpdateValues();
                }
                
                return profitPerItem / type.Volume;
            }
        }

        private void UpdateValues()
        {
            purchases.Sort(TransactionItemList.IncreasingUnitPrice);
            sales.Sort(TransactionItemList.DecreasingUnitPrice);

            int saleUnits = 0;
            int minimumQty = 0;
            float saleAmount = 0.0f;

            int purchaseUnits = 0;
            float purchaseAmount = 0.0f;

            // Find number of units first - this will determine other values
            foreach (TransactionItem t in purchases)
            {
                purchaseUnits += t.Quantity;
            }
            foreach (TransactionItem t in sales)
            {
                saleUnits += t.Quantity;
            }

            units = Math.Min(saleUnits, purchaseUnits);
            purchaseUnits = 0;
            saleUnits = 0;
            foreach (TransactionItem t in purchases)
            {
                if ((purchaseUnits + t.Quantity) <= units)
                {
                    purchaseAmount += t.Quantity * t.TradeItem.UnitPrice;
                    purchaseUnits += t.Quantity;
                }
                else
                {
                    purchaseAmount += (units - purchaseUnits) * t.TradeItem.UnitPrice;
                    purchaseUnits = units;
                    break;
                }
            }
            foreach (TransactionItem t in sales)
            {
                if ((saleUnits + t.Quantity) <= units)
                {
                    saleAmount += t.Quantity * t.TradeItem.UnitPrice;
                    saleUnits += t.Quantity;
                    minimumQty += t.MinQuantity;
                }
                else
                {
                    saleAmount += (units - saleUnits) * t.TradeItem.UnitPrice;
                    saleUnits = units;
                    minimumQty += t.MinQuantity;
                    break;
                }
            }

            minQty = minimumQty;
            salePricePerUnit = saleAmount / units;
            purchasePricePerUnit = purchaseAmount / units;
            purchasePrice = purchaseAmount;
            salePrice = saleAmount;
            taxAmount = saleAmount * Parameters.TaxRate;
            profitAmount = saleAmount - purchaseAmount - taxAmount;
            profitPerItem = salePricePerUnit - purchasePricePerUnit;

            upToDate = true;
        }

        private ItemType type;
        public ItemType Type
        {
            get
            {
                return type;
            }
        }

        public Transaction GetTransactionByLimits(float isk, float cargo)
        {
            UpdateValues();

            Transaction returnTransaction = new Transaction(type);
            int quantity = 0;

            foreach (TransactionItem t in purchases)
            {
                if (((t.Quantity * t.UnitPrice) <= isk) && ((t.Quantity * t.Type.Volume) <= cargo))
                {
                    returnTransaction.AddPurchase(t);
                    isk -= (t.Quantity * t.UnitPrice);
                    cargo -= (t.Quantity * t.Type.Volume);
                    quantity += t.Quantity;
                }
                else
                {
                    int maxAmountByCost = (int)(isk / t.UnitPrice);
                    int maxAmountByVolume = (int)(cargo / t.Type.Volume);
                    int amount = Math.Min(maxAmountByCost, maxAmountByVolume);

                    returnTransaction.AddPurchase(t.TradeItem, amount);
                    quantity += amount;
                    break;
                }
            }
            int saleQuantity = quantity;
            int minQty = 0;

            foreach (TransactionItem t in sales)
            {
                if (t.Quantity <= saleQuantity)
                {
                    minQty += t.MinQuantity;
                    returnTransaction.AddSale(t);
                    saleQuantity -= t.Quantity;
                }
                else
                {
                    returnTransaction.AddSale(t.TradeItem, saleQuantity);
                    minQty += t.MinQuantity;
                    break;
                }
            }

            if (quantity < minQty)
            {
                returnTransaction = null;
            }

            return returnTransaction;
        }

        public void AddSale(TransactionItem item)
        {
            AddSale(item.TradeItem, item.Quantity);
        }

        public void AddSale(Trade sale)
        {
            AddSale(sale, sale.Quantity);
        }
        public void AddSale(Trade sale, int qty)
        {
            if (type != sale.Type)
            {
                throw new ArgumentException("Trade is not of the same item type as the transaction");
            }

            bool found = false;
            foreach (TransactionItem t in sales)
            {
                if (t.TradeItem == sale)
                {
                    t.Quantity += qty;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                sales.Add(new TransactionItem(sale, qty));
            }
            upToDate = false;
        }

        public void AddPurchase(TransactionItem item)
        {
            AddPurchase(item.TradeItem, item.Quantity);
        }

        public void AddPurchase(Trade purchase)
        {
            AddPurchase(purchase, purchase.Quantity);
        }

        public void AddPurchase(Trade purchase, int qty)
        {
            if (type != purchase.Type)
            {
                throw new ArgumentException("Trade is not of the same item type as the transaction");
            }

            bool found = false;
            foreach (TransactionItem t in purchases)
            {
                if (t.TradeItem == purchase)
                {
                    t.Quantity += qty;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                purchases.Add(new TransactionItem(purchase, qty));
            }
            upToDate = false;
        }

        private Transaction(ItemType itemType)
        {
            Init(itemType);
        }

        public Transaction(TransactionItem purchase, TransactionItem sale)
        {
            if (purchase.Type != sale.Type)
            {
                throw new ArgumentException("Item types of the trade are not the same");
            }
            Init(purchase.Type);

            this.AddPurchase(purchase);
            this.AddSale(sale);
        }

        public Transaction(Trade purchase, Trade sale)
        {
            if (purchase.Type != sale.Type)
            {
                throw new ArgumentException("Item types of the trade are not the same");
            }
            Init(purchase.Type);

            this.AddPurchase(purchase);
            this.AddSale(sale);
        }

        internal void Init(ItemType itemType)
        {
            type = itemType;

            sales = new TransactionItemList();
            purchases = new TransactionItemList();
        }

        public bool Combine(Transaction add)
        {
            bool done = false;

            if (this.Type == add.Type)
            {
                foreach (TransactionItem t in add.Purchases)
                {
                    AddPurchase(t);
                }
                foreach (TransactionItem t in add.Sales)
                {
                    AddSale(t);
                }

                done = true;
            }

            return done;
        }

        public override string ToString()
        {
            string returnValue = string.Empty;

            foreach (TransactionItem p in purchases)
            {
                returnValue += "Purchase: " + p.ToString() + "/r/g";
            }
            foreach (TransactionItem s in sales)
            {
                returnValue += "Sale: " + s.ToString() + "/r/g";
            }

            return returnValue;
        }

        public string ListPurchases()
        {
            string output = string.Empty;

            foreach (TransactionItem t in purchases)
            {
                output += t.ToString() + "<br>";
            }

            return output;
        }

        public string ListSales()
        {
            string output = string.Empty;

            foreach (TransactionItem t in sales)
            {
                output += t.ToString() + "<br>";
            }

            return output;
        }
    }
}
