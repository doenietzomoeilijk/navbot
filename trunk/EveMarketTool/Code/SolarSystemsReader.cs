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
            ReadFromResource("Data.dbo_mapSolarSystems.csv");
    	}

        Dictionary<int, SolarSystem> solarSystems = new Dictionary<int, SolarSystem>();
        public Dictionary<string, SolarSystem> SolarSystemsByName
        {
            get { return solarSystemsByName; }
        }

        Dictionary<string, SolarSystem> solarSystemsByName = new Dictionary<string, SolarSystem>();
        public Dictionary<int, SolarSystem> SolarSystemsById
        {
            get { return solarSystems; }
        }

        protected override void InterpretRow(Dictionary<string, string> fields)
        {
            int itemId = ParseId(fields["solarSystemID"]);
            string itemName = fields["solarSystemName"];
            float security = ParseNumber(fields["security"]);
            SolarSystem s = new SolarSystem(itemId, itemName, security);
            solarSystems[itemId] = s;
            solarSystemsByName[itemName] = s;
        }
    }
}
