namespace Evacuation.Application.DTOs.Status
{
    public class UpdateStatusDto
    {
        public int TotalEvacuatedPeople { get; set; }
        public int RemainingPeople { get; set; }

        public int LastVehicleUsedId { get; set; }
    }
}
