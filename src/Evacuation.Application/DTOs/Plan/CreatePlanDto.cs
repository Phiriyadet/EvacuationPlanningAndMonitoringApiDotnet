using System.ComponentModel.DataAnnotations;

namespace Evacuation.Application.DTOs.Plan
{
    public class CreatePlanDto
    {
        public int ZoneId { get; set; }
        public int VehicleId { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Number of evacuated people must be a non-negative integer.")]
        public int NumberOfEvacuatedPeople { get; set; }
        public string ETA { get; set; } = string.Empty; // Estimated Time of Arrival
    }
}
