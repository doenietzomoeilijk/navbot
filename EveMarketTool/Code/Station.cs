using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public class Station
    {
        private int id;
        public int Id
        {
            get { return id; }
        }

        private int typeId;
        public int TypeId
        {
            get { return typeId; }
        }

        private string name;
        public string Name
        {
            get { return name; }
        }

        private SolarSystem system;
        public SolarSystem System
        {
            get { return system; }
        }

        private Dictionary<ItemType, TradeList> itemsForSale = new Dictionary<ItemType, TradeList>();
        public Dictionary<ItemType, TradeList> ItemsForSale
        {
            get { return itemsForSale; }
        }

        private Dictionary<ItemType, TradeList> itemsWanted = new Dictionary<ItemType, TradeList>();
        private Dictionary<ItemType, TradeList> itemsWantedReturn = null;
        public Dictionary<ItemType, TradeList> ItemsWanted
        {
            get 
            {
                if (itemsWantedReturn == null)
                {
                    itemsWantedReturn = new Dictionary<ItemType, TradeList>();
                    foreach (KeyValuePair<ItemType, TradeList> element in itemsWanted)
                    {
                        itemsWantedReturn.Add(element.Key, element.Value);
                    }

                    if (system != null)
                    {
                        foreach (KeyValuePair<ItemType, TradeList> element in system.ItemsWanted)
                        {
                            if (!itemsWantedReturn.ContainsKey(element.Key))
                            {
                                itemsWantedReturn.Add(element.Key, element.Value);
                            }
                            else
                            {
                                itemsWantedReturn[element.Key].AddRange(element.Value);
                                itemsWantedReturn[element.Key].Sort(TradeList.DecreasingUnitPrice);
                            }
                        }
                    }
                }

                return itemsWantedReturn; 
            }
        }

        public Station(int id, int typeId, string name, SolarSystem system)
        {
            this.id = id;
            this.typeId = typeId;
            this.name = name;
            this.system = system;
            if (system != null)
            {
                system.Stations.Add(this);
            }
        }

        public void AddItemForSale(Trade t)
        {
            if (!itemsForSale.ContainsKey(t.Type))
                itemsForSale.Add(t.Type, new TradeList());

            itemsForSale[t.Type].Add(t);
            itemsForSale[t.Type].Sort(TradeList.IncreasingUnitPrice);
        }

        public void AddItemWanted(Trade t)
        {
            if (!itemsWanted.ContainsKey(t.Type))
            {
                itemsWanted.Add(t.Type, new TradeList());
            }

            itemsWanted[t.Type].Add(t);
            itemsWanted[t.Type].Sort(TradeList.DecreasingUnitPrice);
        }

        public float GetSaleValueOf(ItemType type, int quantity)
        {
            float runningTotal = 0.0f;
            int quantityToSell = quantity;
            foreach (Trade trade in ItemsWanted[type]) // this had better iterate in sorted order
            {
                int quantitySold = Math.Min(quantityToSell, trade.Quantity);
                runningTotal += trade.UnitPrice * quantitySold;
                quantityToSell -= quantitySold;
                if (quantityToSell == 0) break;
            }
            return runningTotal;
        }

        public void ClearMarketData()
        {
            itemsWanted.Clear();
            itemsForSale.Clear();
            itemsWantedReturn = null;
        }
    }
}
