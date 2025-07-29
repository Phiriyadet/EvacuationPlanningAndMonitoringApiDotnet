using Evacuation.Application.DTOs.Location;
using System.ComponentModel.DataAnnotations;

namespace Evacuation.Application.DTOs.Zone
{
    public class CreateZoneDto
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Number of people must be a non-negative integer.")]
        public int NumberOfPeople { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Urgency level must be between 1 and 5.")]
        public int UrgencyLevel { get; set; }

        [Required]
        public LocationCoordinatesDto Location { get; set; } = new LocationCoordinatesDto();
    }
}
