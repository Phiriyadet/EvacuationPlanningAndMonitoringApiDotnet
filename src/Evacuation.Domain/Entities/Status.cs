namespace Evacuation.Domain.Entities
{
    public class Status
    {
        public string ZoneId { get; set; } = string.Empty;

        public int TotalPeopleEvacuated { get; set; }
        public int RemainingPeople { get; set; }

        public string? LastVehicleId { get; set; }

        private Status() { } // cannot be instantiated directly

    }
}
