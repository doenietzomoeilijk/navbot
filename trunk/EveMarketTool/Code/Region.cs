using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public class Region
    {
        private int id;
        public int Id
        {
            get { return id; }
        }

        private List<SolarSystem> systems = new List<SolarSystem>();
        public List<SolarSystem> Systems
        {
            get { return systems; }
        }

        private Dictionary<ItemType, TradeList> itemsWanted = new Dictionary<ItemType, TradeList>();
        public Dictionary<ItemType, TradeList> ItemsWanted
        {
            get 
            {
                return itemsWanted; 
            }
        }

        public Region(int itemId)
        {
            id = itemId;
        }

        public override string ToString()
        {
            return id.ToString();
        }

        public void AddItemWanted(Trade t)
        {
            if (!itemsWanted.ContainsKey(t.Type))
                itemsWanted.Add(t.Type, new TradeList());

            itemsWanted[t.Type].Add(t);
            itemsWanted[t.Type].Sort(TradeList.DecreasingUnitPrice);
        }

        public void ClearMarketData()
        {
            itemsWanted.Clear();
        }
    }
}
