using Evacuation.Application.DTOs.Plan;
using Evacuation.Domain.Entities;

namespace Evacuation.Application.Mappers
{
    public static class PlanMapper
    {
        public static PlanDto ToDto(this Plan plan)
        {
            return new PlanDto
            {
                PlanId = plan.PlanId,
                ZoneId = plan.ZoneId,
                VehicleId = plan.VehicleId,
                NumberOfEvacuatedPeople = plan.NumberOfEvacuatedPeople,
                ETA = plan.ETA
            };
        }

        public static IEnumerable<PlanDto> ToDto(this IEnumerable<Plan> plans)
        {
            return plans.Select(p => p.ToDto());
        }

        public static Plan CreateToEntity(this CreatePlanDto createDto)
        {
            return new Plan
            {
                ZoneId = createDto.ZoneId,
                VehicleId = createDto.VehicleId,
                NumberOfEvacuatedPeople = createDto.NumberOfEvacuatedPeople,
                ETA = createDto.ETA
            };
        }

    }
}
