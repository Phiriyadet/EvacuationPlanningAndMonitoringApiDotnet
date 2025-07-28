﻿namespace Evacuation.Domain.ValueObjects
{
    public class LocationCoordinates
    {
        public double Latitude { get; }
        public double Longitude { get; }
        public LocationCoordinates(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90)
                throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90 degrees.");
            if (longitude < -180 || longitude > 180)
                throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180 degrees.");
            Latitude = latitude;
            Longitude = longitude;
        }
        public override string ToString() => $"{Latitude}, {Longitude}";
    }
}
