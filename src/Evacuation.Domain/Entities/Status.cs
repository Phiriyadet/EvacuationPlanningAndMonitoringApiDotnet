
namespace Evacuation.Domain.Entities
{
    public class Status : BaseEntityWithPrefix
    {
        public override string Prefix => "S";

        public int ZoneId { get; private set; }
        public int TotalEvacuatedPeople { get; private set; }
        public int RemainingPeople { get; private set; }
        public int LastVehicleIdUsed { get; private set; }

        protected Status() { }

        public Status(int zoneId, int totalEvacuatedPeople, int remainingPeople, int lastVehicleIdUsed)
        {
            ValidateStatus(zoneId, totalEvacuatedPeople, remainingPeople, lastVehicleIdUsed);
            ZoneId = zoneId;
            TotalEvacuatedPeople = totalEvacuatedPeople;
            RemainingPeople = remainingPeople;
            LastVehicleIdUsed = lastVehicleIdUsed;
        }

        public void Update(int totalEvacuatedPeople, int remainingPeople, int lastVehicleIdUsed)
        {
            ValidateStatus(ZoneId, totalEvacuatedPeople, remainingPeople, lastVehicleIdUsed);
            TotalEvacuatedPeople = totalEvacuatedPeople;
            RemainingPeople = remainingPeople;
            LastVehicleIdUsed = lastVehicleIdUsed;
            SetUpdateAt();
        }

        private static void ValidateStatus(int zoneId, int totalEvacuatedPeople, int remainingPeople, int lastVehicleIdUsed)
        {
            if (zoneId <= 0)
                throw new ArgumentOutOfRangeException(nameof(zoneId), "Zone ID must be greater than zero.");
            if (totalEvacuatedPeople < 0)
                throw new ArgumentOutOfRangeException(nameof(totalEvacuatedPeople), "Total evacuated people cannot be negative.");
            if (remainingPeople < 0)
                throw new ArgumentOutOfRangeException(nameof(remainingPeople), "Remaining people cannot be negative.");
            if (lastVehicleIdUsed <= 0)
                throw new ArgumentOutOfRangeException(nameof(lastVehicleIdUsed), "Last vehicle ID used must be greater than zero.");
        }
    }
}
