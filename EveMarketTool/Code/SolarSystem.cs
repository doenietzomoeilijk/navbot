using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public class SolarSystem
    {
        private int id;
        public int Id
        {
            get { return id; }
        }

        private string name;
        public string Name
        {
            get { return name; }
        }

        private Region region;
        public Region Region
        {
            get { return region; }
        }

        private float securityValue;

        public float Security
        {
            get { return securityValue; }
        }

        private List<SolarSystem> gates = new List<SolarSystem>();
        public List<SolarSystem> Gates
        {
            get { return gates; }
        }

        private Signpost signpostSecure = new Signpost();
        public Signpost SignpostSecure
        {
            get { return signpostSecure; }
        }

        private Signpost signpostShortest = new Signpost();
        public Signpost SignpostShortest
        {
            get { return signpostShortest; }
        }

        private StationList stations = new StationList();
        public StationList Stations
        {
            get { return stations; }
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

                    foreach (KeyValuePair<ItemType, TradeList> element in region.ItemsWanted)
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

                return itemsWantedReturn;
            }
        }

        public SolarSystem(int itemId, string itemName, Region region, float security)
        {
            id = itemId;
            name = itemName;
            securityValue = security;
            this.region = region;
            signpostShortest[this] = new SignpostEntry(null, 0);
            if (security >= 0.45f)
            {
                signpostSecure[this] = new SignpostEntry(null, 0);
            }
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

        public void ClearMarketData()
        {
            itemsWantedReturn = null;
            itemsWanted.Clear();
        }

        public override string ToString()
        {
            return name;
        }

        public void AddGateTo(SolarSystem destination)
        {
            gates.Add(destination);
            if (SecurityStatus.IsSecure(Math.Min(this.Security, destination.Security)))
            {
                signpostSecure[destination] = new SignpostEntry(destination, 1);
            }
            signpostShortest[destination] = new SignpostEntry(destination, 1);
        }

        public bool UpdateSignpostTo(SolarSystem destination) // returns true if the destination has already been found
        {
            bool updated = false;

            SolarSystem sSecure = GetNeighbourWithShortestDistanceTo(destination, true);
            SolarSystem sShort = GetNeighbourWithShortestDistanceTo(destination, false);

            if ((sSecure != null) && (SecurityStatus.IsSecure(this.Security)))
            {
                if (signpostSecure.ContainsKey(destination)) 
                {
                    if (signpostSecure[destination].Distance > (sSecure.SignpostSecure[destination].Distance + 1))
                    {
                        signpostSecure[destination] = new SignpostEntry(sSecure, sSecure.SignpostSecure[destination].Distance + 1);
                        updated = true;
                    }
                }
                else
                {
                    signpostSecure[destination] = new SignpostEntry(sSecure, sSecure.SignpostSecure[destination].Distance + 1);
                    updated = true;
                }
            }

            if (sShort != null)
            {
                if (signpostShortest.ContainsKey(destination))
                {
                    if (signpostShortest[destination].Distance > (sShort.SignpostShortest[destination].Distance + 1))
                    {
                        signpostShortest[destination] = new SignpostEntry(sShort, sShort.SignpostShortest[destination].Distance + 1);
                        updated = true;
                    }
                }
                else
                {
                    signpostShortest[destination] = new SignpostEntry(sShort, sShort.SignpostShortest[destination].Distance + 1);
                    updated = true;
                }
            }

            return updated;
        }

        private SolarSystem GetNeighbourWithShortestDistanceTo(SolarSystem destination, bool secure)
        {
            SolarSystem nearest = null;
            int shortestFound = int.MaxValue;
            foreach (SolarSystem s in gates)
            {
                if ((secure) && (s.SignpostSecure.ContainsKey(destination)))
                {
                    if (s.SignpostSecure[destination].Distance < shortestFound)
                    {
                        nearest = s;
                        shortestFound = s.SignpostSecure[destination].Distance;
                    }
                }
                else if ((!secure) && (s.SignpostShortest.ContainsKey(destination)))
                {
                    if (s.SignpostShortest[destination].Distance < shortestFound)
                    {
                        nearest = s;
                        shortestFound = s.SignpostShortest[destination].Distance;
                    }
                }
            }
            return nearest;
        }

    }
}
