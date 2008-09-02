using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public class Map
    {
        Dictionary<string, SolarSystem> solarSystemsByName;
        Dictionary<int, SolarSystem> solarSystemsById;
        Dictionary<int, Region> regionsById;
        Dictionary<int, Station> stationsById;

        public Map()
        {
            SolarSystemsReader systemsReader = new SolarSystemsReader();
            solarSystemsByName = systemsReader.SolarSystemsByName;
            solarSystemsById = systemsReader.SolarSystemsById;
            regionsById = systemsReader.RegionsById;

            SolarSystemJumpsReader jumpsReader = new SolarSystemJumpsReader(this);
            StationReader stationReader = new StationReader(this);
            stationsById = stationReader.StationsById;
        }

        /// <summary>
        /// If you're wondering why this is useful, it's for unit testing Map. The readers passed in will be dummys.
        /// </summary>
        public Map(SolarSystemsReader systemsReader, StationReader stationReader)
        {
            solarSystemsByName = systemsReader.SolarSystemsByName;
            solarSystemsById = systemsReader.SolarSystemsById;
            stationsById = stationReader.StationsById;
            regionsById = systemsReader.RegionsById;
        }

        public Region GetRegion(int id)
        {
            if (regionsById.ContainsKey(id))
                return regionsById[id];
            else
                return null;
        }

        public SolarSystem GetSystem(int id)
        {
            if (solarSystemsById.ContainsKey(id))
                return solarSystemsById[id];
            else
                return null;
        }

        public Station GetStation(int id)
        {
            if (stationsById.ContainsKey(id))
                return stationsById[id];
            else
                return null;
        }

        public SolarSystem GetSystem(string name)
        {
            if (name != null && solarSystemsByName.ContainsKey(name))
                return solarSystemsByName[name];
            else
                return null;
        }

        public List<SolarSystem> SystemList
        {
            get 
            {
                return new List<SolarSystem>(solarSystemsByName.Values);
            }
        }

        public List<Station> StationList
        {
            get
            {
                return new List<Station>(stationsById.Values);
            }
        }

        public void ClearMarketData()
        {
            foreach (Region r in regionsById.Values)
            {
                r.ClearMarketData();
            }

            foreach (SolarSystem s in SystemList)
            {
                s.ClearMarketData();
            }

            foreach (Station st in StationList)
            {
                st.ClearMarketData();
            }
        }

        public SecurityStatus.Level RouteSecurity(SolarSystem source, SolarSystem destination)
        {
            SecurityStatus.Level securityFromStartingSystem = SecurityStatus.Level.Isolated;

            int jumpsFromStartSecure = DistanceBetween(source, destination, true);
            int jumpsFromStartShortest = DistanceBetween(source, destination, false);

            if (jumpsFromStartSecure == jumpsFromStartShortest)
            {
                securityFromStartingSystem = SecurityStatus.Level.HighSec;
            }
            else if ((jumpsFromStartSecure > jumpsFromStartShortest) && (jumpsFromStartSecure != int.MaxValue))
            {
                securityFromStartingSystem = SecurityStatus.Level.LowSecShortcut;
            }
            else if ((jumpsFromStartSecure == int.MaxValue) && (jumpsFromStartShortest != int.MaxValue))
            {
                securityFromStartingSystem = SecurityStatus.Level.LowSecOnly;
            }

            return securityFromStartingSystem;
        }

        public int DistanceBetween(SolarSystem source, SolarSystem destination, bool secure)
        {
            UpdateSignpostsBetween(source, destination);
            if ((secure) && (source.SignpostSecure.ContainsKey(destination)))
            {
                return source.SignpostSecure[destination].Distance;
            }
            else if ((!secure) && (source.SignpostShortest.ContainsKey(destination)))
            {
                return source.SignpostShortest[destination].Distance;
            }
            else
            {
                return int.MaxValue;
            }
        }

        void UpdateSignpostsBetween(SolarSystem source, SolarSystem destination)
        {
            bool stillExpanding = true; // maybe the route's impossible (Jove) in which case we need to know when to give up!
            while (stillExpanding && !(source.SignpostShortest.ContainsKey(destination) && source.SignpostSecure.ContainsKey(destination)))
            {
                stillExpanding = false; // assume false until proven otherwise
                foreach (SolarSystem s in solarSystemsById.Values)
                {
                    bool expanded = s.UpdateSignpostTo(destination);
                    stillExpanding = stillExpanding || expanded;
                }
            }
            if (!source.SignpostSecure.ContainsKey(destination))
            {
                // If we didn't find a secure route, drop a marker here to indicate that
                source.SignpostSecure[destination] = new SignpostEntry(null, int.MaxValue);
            }
        }
    }
}
