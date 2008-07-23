using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public class Map
    {
        Dictionary<string, SolarSystem> solarSystemsByName;
        Dictionary<int, SolarSystem> solarSystemsById;
        Dictionary<int, Station> stationsById;

        public Map() : this(false)
        {
        }

        public Map(bool highSecOnly)
        {
            SolarSystemsReader systemsReader = new SolarSystemsReader();
            solarSystemsByName = systemsReader.SolarSystemsByName;
            solarSystemsById = systemsReader.SolarSystemsById;

            SolarSystemJumpsReader jumpsReader = new SolarSystemJumpsReader(this);
            StationReader stationReader = new StationReader(this);
            stationsById = stationReader.StationsById;

            if(highSecOnly)
                RemoveLowSecSystems();
        }

        /// <summary>
        /// If you're wondering why this is useful, it's for unit testing Map. The readers passed in will be dummys.
        /// </summary>
        public Map(SolarSystemsReader systemsReader, StationReader stationReader)
        {
            solarSystemsByName = systemsReader.SolarSystemsByName;
            solarSystemsById = systemsReader.SolarSystemsById;
            stationsById = stationReader.StationsById;
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

        public int DistanceBetween(SolarSystem source, SolarSystem destination)
        {
            UpdateSignpostsBetween(source, destination);
            if (source.Signpost.ContainsKey(destination))
                return source.Signpost[destination].Distance;
            else
            {
                return int.MaxValue;
            }
        }

        void UpdateSignpostsBetween(SolarSystem source, SolarSystem destination)
        {
            bool stillExpanding = true; // maybe the route's impossible (Jove) in which case we need to know when to give up!
            while (stillExpanding && !source.Signpost.ContainsKey(destination))
            {
                stillExpanding = false; // assume false until proven otherwise
                foreach (SolarSystem s in solarSystemsById.Values)
                {
                    bool expanded = s.UpdateSignpostTo(destination);
                    stillExpanding = stillExpanding || expanded;
                }
            }
        }

        void RemoveLowSecSystems()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
