using Evacuation.Domain.ValueObjects;

namespace Evacuation.Domain.Entities
{
    public class EvacuationZone
    {
        public string ZoneId { get; set; } = string.Empty;

        public int NumberOfPeople { get; set; }
        public int UrgencyLevel { get; set; }

        public LocationCoordinates LocationCoordinates { get; set; } = new LocationCoordinates();

        private EvacuationZone() { } // cannot be instantiated directly

    }
}
