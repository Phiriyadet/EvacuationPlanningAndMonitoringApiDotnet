using Evacuation.Domain.ValueObjects;

namespace Evacuation.Domain.Entities
{
    public class Zone : BaseEntityWithPrefix
    {
        public override string Prefix => "Z";

        public int NumberOfPeople { get; private set; }
        public int UrgencyLevel { get; private set; }
        public LocationCoordinates LocationCoordinates { get; private set; } = null!;

        protected Zone() { }
        public Zone(int numberOfPeople, int urgencyLevel, LocationCoordinates locationCoordinates)
        {
            ValidateZone(numberOfPeople, urgencyLevel, locationCoordinates);
            NumberOfPeople = numberOfPeople;
            UrgencyLevel = urgencyLevel;
            LocationCoordinates = locationCoordinates;
        }

        public void Update(int numberOfPeople, int urgencyLevel, LocationCoordinates locationCoordinates)
        {
            ValidateZone(numberOfPeople, urgencyLevel, locationCoordinates);
            NumberOfPeople = numberOfPeople;
            UrgencyLevel = urgencyLevel;
            LocationCoordinates = locationCoordinates;
            SetUpdateAt();
        }

        private static void ValidateZone(int numberOfPeople, int urgencyLevel, LocationCoordinates locationCoordinates)
        {
            if (numberOfPeople < 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfPeople), "Number of people cannot be negative.");
            if (urgencyLevel < 1 || urgencyLevel > 5)
                throw new ArgumentOutOfRangeException(nameof(urgencyLevel), "Urgency level must be between 1 and 5.");
            if (locationCoordinates == null)
                throw new ArgumentNullException(nameof(locationCoordinates), "Location coordinates cannot be null.");
        }
    }
}
