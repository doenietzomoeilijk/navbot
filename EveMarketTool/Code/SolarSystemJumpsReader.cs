using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    class SolarSystemJumpsReader : EveExportReader
    {
        Map map;

        public SolarSystemJumpsReader(Map map)
        {
            this.map = map;
            ReadFromResource("Data.dbo_mapSolarSystemJumps.csv");
        }

        protected override void InterpretRow(Dictionary<string, string> fields)
        {
            int fromSystemId = ParseId(fields["fromSolarSystemID"]);
            int toSystemId = ParseId(fields["toSolarSystemID"]);

            SolarSystem from = map.GetSystem(fromSystemId);
            SolarSystem to = map.GetSystem(toSystemId);

            if (from != null && to != null)
            {
                from.AddGateTo(to);
            }
            else
            {
                Console.WriteLine("I can't link system " + fromSystemId + " to system " + toSystemId + " because one or both are not in my map.");
            }
        }
    }
}
