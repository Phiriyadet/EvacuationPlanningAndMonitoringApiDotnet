namespace Evacuation.Domain.Entities
{
    public class EvacuationStatus
    {
        public string ZoneId { get; set; } = string.Empty;

        public int TotalPeopleEvacuated { get; set; }
        public int RemainingPeople { get; set; }

        public string? LastVehicleId { get; set; }

    }
}
