using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public class StationReader : EveExportReader
    {
        Map map;

        private Dictionary<int, Station> stationsById = new Dictionary<int, Station>();
        public Dictionary<int, Station> StationsById
        {
            get { return stationsById; }
        }

        /// <summary>
        /// If you're wondering what this is for, it's for testing - when a superclass doesn't want to read the data file!
        /// </summary>
        protected StationReader()
        {
        }

        public StationReader(Map map)
        {
            this.map = map;
            ReadFromResource("Data.dbo_staStations.csv", "\r\n");
        }

        public StationReader(Map map, string fullPath)
        {
            this.map = map;
            ReadFromFullPath(fullPath);
        }

        protected override void InterpretRow(Dictionary<string, string> fields)
        {
            int stationId = ParseId(fields["stationID"]);
            int stationTypeId = ParseId(fields["stationTypeID"]);
            string name = fields["stationName"];
            int systemId = ParseId(fields["solarSystemID"]);

            SolarSystem system = map.GetSystem(systemId);
            if (system != null)
            {
                Station station = new Station(stationId, stationTypeId, name, system);
                stationsById.Add(stationId, station);
            }
        }
    }
}
