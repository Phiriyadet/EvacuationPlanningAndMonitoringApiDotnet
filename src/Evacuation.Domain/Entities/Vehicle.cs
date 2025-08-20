using Evacuation.Domain.Enums;
using Evacuation.Domain.ValueObjects;

namespace Evacuation.Domain.Entities
{
    public class Vehicle : BaseEntityWithPrefix
    {
        public override string Prefix => "V";

        public VehicleType Type { get; private set; }
        public int Capacity { get; private set; }
        public int Speed { get; private set; } // Speed in km/h
        public bool IsAvailable { get; private set; } = true;
        public LocationCoordinates LocationCoordinates { get; private set; } = null!;

        protected Vehicle() { }

        public Vehicle(VehicleType type, int capacity, int speed, bool isAvailable, LocationCoordinates locationCoordinates)
        {
            ValidateVehicle(type, capacity, speed, locationCoordinates);
            Type = type;
            Capacity = capacity;
            Speed = speed;
            IsAvailable = isAvailable;
            LocationCoordinates = locationCoordinates;
        }

        public void Update(VehicleType type, int capacity, int speed, LocationCoordinates locationCoordinates)
        {
            ValidateVehicle(type, capacity, speed, locationCoordinates);
            Type = type;
            Capacity = capacity;
            Speed = speed;
            LocationCoordinates = locationCoordinates;
            SetUpdateAt();
        }

        public void SetAvailability(bool isAvailable)
        {
            IsAvailable = isAvailable;
            SetUpdateAt();
        }

        private static void ValidateVehicle(VehicleType type, int capacity, int speed, LocationCoordinates locationCoordinates)
        {
            if (!Enum.IsDefined(typeof(VehicleType), type))
                throw new ArgumentOutOfRangeException(nameof(type), "Invalid vehicle type.");
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero.");
            if (speed <= 0)
                throw new ArgumentOutOfRangeException(nameof(speed), "Speed must be greater than zero.");
            if (locationCoordinates == null)
                throw new ArgumentNullException(nameof(locationCoordinates), "Location coordinates cannot be null.");
        }
    }
}
