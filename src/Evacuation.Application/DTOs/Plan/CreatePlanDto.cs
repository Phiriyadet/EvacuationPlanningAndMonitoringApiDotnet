namespace Evacuation.Application.DTOs.Plan
{
    public class CreatePlanDto
    {
        public string ZoneId { get; set; } = string.Empty;
        public string VehicleId { get; set; } = string.Empty;
        public int NumberOfEvacuatedPeople { get; set; }
        public string ETA { get; set; } = string.Empty; // Estimated Time of Arrival
    }
}
