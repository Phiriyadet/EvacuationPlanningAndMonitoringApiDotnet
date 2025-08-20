using Evacuation.Application.DTOs.Plan;
using Evacuation.Domain.Entities;

namespace Evacuation.Application.Mappers
{
    public static class PlanMapper
    {
        public static PlanDto ToDto(this Plan plan, string zoneId, string vehicleId)
        {
            return new PlanDto
            {
                PlanId = plan.BusinessId,
                ZoneId = zoneId,
                VehicleId = vehicleId,
                NumberOfEvacuatedPeople = plan.NumberOfEvacuatedPeople,
                ETA = plan.ETA
            };
        }

        public static IEnumerable<PlanDto> ToDto(this IEnumerable<Plan> plans, 
            IDictionary<int, string> zoneIdMap, 
            IDictionary<int, string> vehicleIdMap)
        {
            return plans.Select(p => p.ToDto(
                zoneIdMap[p.ZoneId],
                vehicleIdMap[p.VehicleId]
                ));
        }

        public static Plan CreateToEntity(this CreatePlanDto createDto)
        {
            return new Plan
            (
                createDto.ZoneId,
                createDto.VehicleId,
                createDto.NumberOfEvacuatedPeople,
                createDto.ETA
            );
        }

    }
}
