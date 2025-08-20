
namespace Evacuation.Domain.Entities
{
    public class Plan : BaseEntityWithPrefix
    {
        public override string Prefix => "P";
        public int ZoneId { get; private set; }
        public int VehicleId { get; private set; }
        public int NumberOfEvacuatedPeople { get; private set; }
        public string ETA { get; private set; } = string.Empty;

        protected Plan() { }

        public Plan(int zoneId, int vehicleId, int numberOfEvacuatedPeople, string eta)
        {
            ValidatePlan(zoneId, vehicleId, numberOfEvacuatedPeople, eta);
            ZoneId = zoneId;
            VehicleId = vehicleId;
            NumberOfEvacuatedPeople = numberOfEvacuatedPeople;
            ETA = eta;
        }

        private static void ValidatePlan(int zoneId, int vehicleId, int numberOfEvacuatedPeople, string eta)
        {
            if (zoneId <= 0)
                throw new ArgumentOutOfRangeException(nameof(zoneId), "Zone ID must be greater than zero.");
            if (vehicleId <= 0)
                throw new ArgumentOutOfRangeException(nameof(vehicleId), "Vehicle ID must be greater than zero.");
            if (numberOfEvacuatedPeople < 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfEvacuatedPeople), "Number of evacuated people cannot be negative.");
            if (string.IsNullOrWhiteSpace(eta))
                throw new ArgumentException("ETA cannot be null or empty.", nameof(eta));
        }
    }
}
