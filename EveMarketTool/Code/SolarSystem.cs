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

        private float security;

        public float Security
        {
            get { return security; }
            set { security = value; }
        }

        private List<SolarSystem> gates = new List<SolarSystem>();
        public List<SolarSystem> Gates
        {
            get { return gates; }
        }

        private Signpost signpost = new Signpost();
        public Signpost Signpost
        {
            get { return signpost; }
        }

        public SolarSystem(int itemId, string itemName)
        {
            id = itemId;
            name = itemName;
            signpost[this] = new SignpostEntry(null, 0);
        }

        public void ClearMarketData()
        {
        }

        public override string ToString()
        {
            return name;
        }

        public void AddGateTo(SolarSystem destination)
        {
            gates.Add(destination);
            signpost[destination] = new SignpostEntry(destination, 1);
        }

        public bool UpdateSignpostTo(SolarSystem destination) // returns true if the destination has already been found
        {
            if (signpost.ContainsKey(destination))
            {
                return false;
            }
            else
            {
                SolarSystem s = GetNeighbourWithShortestDistanceTo(destination);
                if (s == null)
                {
                    return false;
                }
                else
                {
                    signpost[destination] = new SignpostEntry(s, s.Signpost[destination].Distance+1);
                    return true;
                }
            }
        }

        private SolarSystem GetNeighbourWithShortestDistanceTo(SolarSystem destination)
        {
            SolarSystem nearest = null;
            int shortestFound = int.MaxValue;
            foreach (SolarSystem s in gates)
            {
                if (s.Signpost.ContainsKey(destination))
                {
                    if (s.Signpost[destination].Distance < shortestFound)
                    {
                        nearest = s;
                        shortestFound = s.Signpost[destination].Distance;
                    }
                }
            }
            return nearest;
        }
    }
}
