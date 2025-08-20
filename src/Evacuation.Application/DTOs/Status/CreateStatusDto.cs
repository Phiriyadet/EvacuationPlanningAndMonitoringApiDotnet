namespace Evacuation.Application.DTOs.Status
{
    public class CreateStatusDto
    {
        public int ZoneId { get; set; }

        public int TotalEvacuatedPeople { get; set; }
        public int RemainingPeople { get; set; }

        public int LastVehicleUsedId { get; set; }
    }
}
