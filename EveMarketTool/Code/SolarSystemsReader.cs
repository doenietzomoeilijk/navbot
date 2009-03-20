using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public class SolarSystemsReader : EveExportReader
    {
        /// <summary>
        /// Useful for test cases - mock objects can inherit and pass a new data file, or an empty string to load no data
        /// </summary>
        protected SolarSystemsReader(string fullPath)
        {
            if(fullPath.Length>0)
                ReadFromFullPath(fullPath);
        }

        public SolarSystemsReader()
    	{
            ReadFromResource("Data.dbo_mapSolarSystems.csv", "\r\n");
    	}

        Dictionary<string, SolarSystem> solarSystemsByName = new Dictionary<string, SolarSystem>();
        public Dictionary<string, SolarSystem> SolarSystemsByName
        {
            get { return solarSystemsByName; }
        }

        Dictionary<int, Region> regions = new Dictionary<int, Region>();
        public Dictionary<int, Region> RegionsById
        {
            get { return regions; }
        }

        Dictionary<int, SolarSystem> solarSystems = new Dictionary<int, SolarSystem>();
        public Dictionary<int, SolarSystem> SolarSystemsById
        {
            get { return solarSystems; }
        }

        protected override void InterpretRow(Dictionary<string, string> fields)
        {
            int itemId = ParseId(fields["solarSystemID"]);
            string itemName = fields["solarSystemName"];
            float security = ParseNumber(fields["security"]);
            int regionId = ParseId(fields["regionID"]);
            Region r = null;

            if (regions.ContainsKey(regionId))
            {
                r = regions[regionId];
            }
            if (r == null)
            {
                r = new Region(regionId);
                regions.Add(regionId, r);
            }

            SolarSystem s = new SolarSystem(itemId, itemName, r, security);
            r.Systems.Add(s);
            solarSystems[itemId] = s;
            solarSystemsByName[itemName] = s;

        }
    }
}
