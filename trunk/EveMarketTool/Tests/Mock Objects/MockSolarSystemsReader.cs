using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool.Tests.Mock_Objects
{
    class MockSolarSystemsReader : SolarSystemsReader
    {
        public MockSolarSystemsReader() : base("")
        {
        }

        public void AddSystem(SolarSystem s)
        {
            SolarSystemsById.Add(s.Id, s);
            SolarSystemsByName.Add(s.Name, s);
            if (!RegionsById.ContainsKey(s.Region.Id))
            {
                RegionsById.Add(s.Region.Id, s.Region);
            }
            s.Region.Systems.Add(s);
        }
    }
}
