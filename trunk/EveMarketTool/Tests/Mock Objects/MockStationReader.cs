using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool.Tests.Mock_Objects
{
    class MockStationReader : StationReader
    {
        public MockStationReader()
        {
        }

        public void AddStation(Station s)
        {
            StationsById.Add(s.Id, s);
        }
    }
}
