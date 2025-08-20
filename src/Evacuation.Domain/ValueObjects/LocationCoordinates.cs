namespace Evacuation.Domain.ValueObjects
{
    public class LocationCoordinates
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        protected LocationCoordinates() { } // For EF Core

        public LocationCoordinates(double latitude, double longitude)
        {
            ValidateLocation(latitude, longitude);
            Latitude = latitude;
            Longitude = longitude;
        }

        public void SetLocation(double latitude, double longitude)
        {
            ValidateLocation(latitude, longitude);
            Latitude = latitude;
            Longitude = longitude;
        }

        private static void ValidateLocation(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90)
                throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90.");
            if (longitude < -180 || longitude > 180)
                throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180.");
        }

    }


}
