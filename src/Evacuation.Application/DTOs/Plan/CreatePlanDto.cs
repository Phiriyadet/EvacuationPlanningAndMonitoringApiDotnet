using System.ComponentModel.DataAnnotations;

namespace Evacuation.Application.DTOs.Plan
{
    public class CreatePlanDto
    {
        public string ZoneId { get; set; } = string.Empty;
        public string VehicleId { get; set; } = string.Empty;
        [Range(0, int.MaxValue, ErrorMessage = "Number of evacuated people must be a non-negative integer.")]
        public int NumberOfEvacuatedPeople { get; set; }
        public string ETA { get; set; } = string.Empty; // Estimated Time of Arrival
    }
}
