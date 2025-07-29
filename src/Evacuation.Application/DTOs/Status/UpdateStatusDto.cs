namespace Evacuation.Application.DTOs.Status
{
    public class UpdateStatusDto
    {
        public int TotalPeopleEvacuated { get; set; }
        public int RemainingPeople { get; set; }

        public string? LastVehicleId { get; set; }
    }
}
