using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public class Market : CSVFileReader
    {
        Map map;
        ItemDatabase itemDatabase;

        private Dictionary<ItemType, StationList> stationsWithItemForSale = new Dictionary<ItemType, StationList>();
        public Dictionary<ItemType, StationList> StationsWithItemForSale
        {
            get { return stationsWithItemForSale; }
        }

        private Dictionary<ItemType, StationList> stationsWithItemWanted = new Dictionary<ItemType, StationList>();
        public Dictionary<ItemType, StationList> StationsWithItemWanted
        {
            get { return stationsWithItemWanted; }
        }

        public Market(ItemDatabase itemDatabase, Map map)
        {
            this.itemDatabase = itemDatabase;
            this.map = map;
            foreach (ItemType type in itemDatabase.AllItems)
            {
                stationsWithItemWanted[type] = new StationList();
                stationsWithItemForSale[type] = new StationList();
            }
        }

        protected override void InterpretRow(Dictionary<string, string> fields)
        {
            float price = float.Parse(fields["price"], System.Globalization.CultureInfo.InvariantCulture);
            int quantity = (int)float.Parse(fields["volRemaining"], System.Globalization.CultureInfo.InvariantCulture);
            int typeId = int.Parse(fields["typeID"]);
            int stationId = int.Parse(fields["stationID"]);
            int systemId = int.Parse(fields["solarSystemID"]);
            string isWanted = fields["bid"];
            SolarSystem s = map.GetSystem(systemId);
            Station station = map.GetStation(stationId);
            ItemType type = itemDatabase.GetItemType(typeId);
            if (s != null && type != null && station != null)
            {
                Trade trade = new Trade(type, price, quantity);
                if (isWanted == "True")
                {
                    station.AddItemWanted(trade);
                    if (!stationsWithItemWanted[type].Contains(station))
                        stationsWithItemWanted[type].Add(station);
                }
                else
                {
                    station.AddItemForSale(trade);
                    if (!stationsWithItemForSale[type].Contains(station))
                        stationsWithItemForSale[type].Add(station);
                }
            }
        }
    }
}
