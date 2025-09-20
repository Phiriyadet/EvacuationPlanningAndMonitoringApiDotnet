
namespace Evacuation.Domain.Entities
{
    public class Status : BaseEntityWithPrefix
    {
        public override string Prefix => "S";

        public int ZoneId { get; private set; }
        public int TotalEvacuatedPeople { get; private set; }
        public int RemainingPeople { get; private set; }
        public int LastVehicleUsedId { get; private set; }

        //Navigation properties
        public Zone Zone { get; private set; } = null!;
        public Vehicle Vehicle { get; private set; } = null!;

        protected Status() { }

        public Status(int zoneId, int totalEvacuatedPeople, int remainingPeople, int lastVehicleUsedId)
        {
            ValidateStatus(zoneId, totalEvacuatedPeople, remainingPeople, lastVehicleUsedId);
            ZoneId = zoneId;
            TotalEvacuatedPeople = totalEvacuatedPeople;
            RemainingPeople = remainingPeople;
            LastVehicleUsedId = lastVehicleUsedId;
        }

        public void Update(int totalEvacuatedPeople, int remainingPeople, int lastVehicleUsedId)
        {
            ValidateStatus(ZoneId, totalEvacuatedPeople, remainingPeople, lastVehicleUsedId);
            TotalEvacuatedPeople = totalEvacuatedPeople;
            RemainingPeople = remainingPeople;
            LastVehicleUsedId = lastVehicleUsedId;
            SetUpdateAt();
        }

        private static void ValidateStatus(int zoneId, int totalEvacuatedPeople, int remainingPeople, int lastVehicleUsedId)
        {
            if (zoneId <= 0)
                throw new ArgumentOutOfRangeException(nameof(zoneId), "Zone ID must be greater than zero.");
            if (totalEvacuatedPeople < 0)
                throw new ArgumentOutOfRangeException(nameof(totalEvacuatedPeople), "Total evacuated people cannot be negative.");
            if (remainingPeople < 0)
                throw new ArgumentOutOfRangeException(nameof(remainingPeople), "Remaining people cannot be negative.");
            if (lastVehicleUsedId <= 0)
                throw new ArgumentOutOfRangeException(nameof(lastVehicleUsedId), "Last vehicle ID used must be greater than zero.");
        }
    }
}
