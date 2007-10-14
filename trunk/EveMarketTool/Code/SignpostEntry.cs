using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public class SignpostEntry
    {
        public SolarSystem Direction;
        public int Distance;

        public SignpostEntry(SolarSystem direction, int distance)
        {
            Direction = direction;
            Distance = distance;
        }
    }
}
