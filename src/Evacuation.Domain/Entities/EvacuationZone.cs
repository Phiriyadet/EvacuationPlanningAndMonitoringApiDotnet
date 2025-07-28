using Evacuation.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evacuation.Domain.Entities
{
    public class EvacuationZone
    {
        public string ZoneId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int NumberOfPeople { get; set; }
        public int UrgencyLevel { get; set; }
        public LocationCoordinates LocationCoordinates { get; set; } = new();
    }
}
