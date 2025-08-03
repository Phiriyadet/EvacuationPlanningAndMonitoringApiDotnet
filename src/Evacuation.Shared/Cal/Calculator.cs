namespace Evacuation.Shared.Cal
{
    public static class Calculator
    {
        public static double CalDistanceKm(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            const double EarthRadiusKm = 6371.0;
            double dLat = DegreesToRadians(latitude2 - latitude1);
            double dLon = DegreesToRadians(longitude2 - longitude1);
            latitude1 = DegreesToRadians(latitude1);
            latitude2 = DegreesToRadians(latitude2);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(latitude1) * Math.Cos(latitude2) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        public static string CalETA(double distanceKm, double speedKmh)
        {
            if (speedKmh <= 0)
            {
                throw new ArgumentException("Speed must be greater than zero.");
            }
            double timeHours = distanceKm / speedKmh;
            int hours = (int)timeHours;
            int minutes = (int)((timeHours - hours) * 60);
            
            if (hours == 0 && minutes == 0)
            {
                return "less than a minute";
            }
            else if (hours == 0)
            {
                return $"{minutes} minutes";
            }
            else if (minutes == 0)
            {
                return $"{hours} hours";
            }
            else
            {
                return $"{hours} hours {minutes} minutes";
            }
        }
    }
}
