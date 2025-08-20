namespace Evacuation.Application.DTOs.Status
{
    public class StatusDto
    {
        public string StatusId { get; set; } = string.Empty;
        public string ZoneId { get; set; } = string.Empty;

        public int TotalEvacuatedPeople { get; set; }
        public int RemainingPeople { get; set; }

        public string LastVehicleUsedId { get; set; } = string.Empty;
    }
}
