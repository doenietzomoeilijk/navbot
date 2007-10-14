using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public enum TripType { SingleTrip, RoundTrip };

    public class Parameters
    {
        private float cargoSpace;
        public float CargoSpace
        {
            get { return cargoSpace; }
        }

        private float isk;
        public float Isk
        {
            get { return isk; }
        }

        private string startingSystem;
        public string StartingSystem
        {
            get { return startingSystem; }
        }

        private TripType trip;
        public TripType Trip
        {
            get { return trip; }
        }

        public Parameters(float isk, float cargoSpace, string startingSystem, TripType trip)
        {
            this.isk = isk;
            this.cargoSpace = cargoSpace;
            this.startingSystem = startingSystem;
            this.trip = trip;
        }
    }
}
