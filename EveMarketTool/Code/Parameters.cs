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

        static private float taxRate = 0.01f;
        static public float TaxRate
        {
            get { return taxRate; }
        }

        public Parameters(float isk, float cargoSpace, string startingSystem, TripType trip): this(isk, cargoSpace, startingSystem, trip, 0)
        {
        }

        public Parameters(float isk, float cargoSpace, string startingSystem, TripType trip, int accountingLevel)
        {
            this.isk = isk;
            this.cargoSpace = cargoSpace;
            this.startingSystem = startingSystem;
            this.trip = trip;
            taxRate = 0.01f - (accountingLevel * 0.001f);
        }
    }
}
