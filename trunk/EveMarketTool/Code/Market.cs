using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public class Market : CSVFileReader
    {
        Map map;
        ItemDatabase itemDatabase;


        private StationList stationsWithItemsForSale = new StationList();
        public StationList StationsWithItemsForSale
        {
            get
            {
                return stationsWithItemsForSale;
            }
        }

        private StationList stationsWithItemsWanted = new StationList();
        public StationList StationsWithItemsWanted
        {
            get
            {
                return stationsWithItemsWanted;
            }
        }
        /*
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

        private Dictionary<Station, ItemList> itemsForSaleAtStation = new Dictionary<Station, ItemList>();
        public Dictionary<Station, ItemList> ItemsForSaleAtStation
        {
            get { return itemsForSaleAtStation; }
        }

        private Dictionary<Station, ItemList> itemsWantedAtStation = new Dictionary<Station, ItemList>();
        public Dictionary<Station, ItemList> ItemsWantedAtStation
        {
            get { return itemsWantedAtStation; }
        }
         * */

        public Market(ItemDatabase itemDatabase, Map map)
        {
            this.itemDatabase = itemDatabase;
            this.map = map;
        }

        protected override void InterpretRow(Dictionary<string, string> fields)
        {
            float price = float.Parse(fields["price"], System.Globalization.CultureInfo.InvariantCulture);
            int quantity = (int)float.Parse(fields["volRemaining"], System.Globalization.CultureInfo.InvariantCulture);
            int minQuantity = (int)float.Parse(fields["minVolume"], System.Globalization.CultureInfo.InvariantCulture);
            int typeId = int.Parse(fields["typeID"]);
            int range = int.Parse(fields["range"]);
            int regionId = int.Parse(fields["regionID"]);
            int stationId = int.Parse(fields["stationID"]);
            int systemId = int.Parse(fields["solarSystemID"]);
            string isWanted = fields["bid"];
            SolarSystem s = map.GetSystem(systemId);
            Station station = map.GetStation(stationId);
            ItemType type = itemDatabase.GetItemType(typeId);
            if (s != null && type != null && station != null)
            {
                Trade trade = new Trade(type, price, quantity, minQuantity);
                if (isWanted == "True")
                {
                    // Range is in station only
                    if (range == -1)
                    {
                        station.AddItemWanted(trade);
                        if (!stationsWithItemsWanted.Contains(station))
                        {
                            stationsWithItemsWanted.Add(station);
                        }
                    }
                    // Range it either system or greater
                    else if ((range > -1) & (range < 32767))
                    {
                        foreach (SolarSystem sAtRange in s.Region.Systems)
                        {
                            if (map.DistanceBetween(s, sAtRange, false) <= range)
                            {
                                sAtRange.AddItemWanted(trade);
                                foreach (Station remoteStation in sAtRange.Stations)
                                {
                                    if (!stationsWithItemsWanted.Contains(remoteStation))
                                    {
                                        stationsWithItemsWanted.Add(remoteStation);
                                    }
                                }
                            }
                        }
                    }
                    // Range is regional
                    else if (range == 32767)
                    {
                        s.Region.AddItemWanted(trade);
                        foreach (SolarSystem system in s.Region.Systems)
                        {
                            foreach (Station remoteStation in system.Stations)
                            {
                                if (!stationsWithItemsWanted.Contains(remoteStation))
                                {
                                    stationsWithItemsWanted.Add(remoteStation);
                                }
                            }
                        }
                    }
                }
                else
                {
                    station.AddItemForSale(trade);
                    if (!stationsWithItemsForSale.Contains(station))
                    {
                        stationsWithItemsForSale.Add(station);
                    }
                }
            }
        }
    }
}
