using Evacuation.Application.DTOs.Plan;
using Evacuation.Application.DTOs.Status;
using Evacuation.Application.Mappers;
using Evacuation.Application.Services.Interfaces;
using Evacuation.Domain.Entities;
using Evacuation.Infrastructure.Cache.Interfaces;
using Evacuation.Infrastructure.Repositories.Interfaces;
using Evacuation.Shared.CacheKeys;
using Evacuation.Shared.Cal;
using Evacuation.Shared.Result;
using Microsoft.EntityFrameworkCore;

namespace Evacuation.Application.Services
{
    public class EvacuationService : IEvacuationService
    {
        private readonly IPlanRepository _planRepo;
        private readonly IStatusRepository _statusRepo;
        private readonly IZoneRepository _zoneRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly ICacheService _cacheService;

        public EvacuationService(IPlanRepository planRepo, IStatusRepository statusRepo,
            IZoneRepository zoneRepo, IVehicleRepository vehicleRepo, ICacheService cacheService)
        {
            _planRepo = planRepo;
            _statusRepo = statusRepo;
            _zoneRepo = zoneRepo;
            _vehicleRepo = vehicleRepo;
            _cacheService = cacheService;
        }

        public async Task<OperationResult<bool>> ClearAllPlanAndStatusAsync()
        {
            try 
            {
                // Clear all plans
                var plans = await _planRepo.GetAllAsync();
                if (plans != null && plans.Any())
                {
                    foreach (var plan in plans)
                    {
                        await _planRepo.DeleteAsync(plan.Id);
                    }
                }
                // Clear all statuses
                var statuses = await _statusRepo.GetAllAsync();
                if (statuses != null && statuses.Any())
                {
                    foreach (var status in statuses)
                    {
                        await _statusRepo.DeleteAsync(status.ZoneId);
                        // Clear cache
                        await _cacheService.RemoveAsync(CreateCacheKeyForStatus(status.ZoneId));
                    }
                }
                
                return OperationResult<bool>.Ok(true, "All plans and statuses cleared successfully.");
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail("An error occurred while clearing all plans and statuses.", false, ex);
            }
        }

        private string CreateCacheKeyForStatus(int zoneId)
        {
            return $"{RedisCacheKays.StatusCacheKey}:{zoneId}";
        }

        public async Task<OperationResult<IEnumerable<PlanDto>>> CreatePlanAsync(double distanceKm)
        {
            try 
            {
                // Validate distance
                if (distanceKm <= 0)
                {
                    return OperationResult<IEnumerable<PlanDto>>.Fail("Distance must be greater than zero.", null);
                }

                var sortedZones = await _zoneRepo.GetQuery().OrderByDescending(z => z.UrgencyLevel)
                    .ThenByDescending(z => z.NumberOfPeople).ToListAsync();
                if (sortedZones.Count == 0) 
                {
                    return OperationResult<IEnumerable<PlanDto>>.Fail("No zones available to create a plan.", null);
                }

                var vehicles = await _vehicleRepo.GetQuery().Where(v => v.IsAvailable == true).ToListAsync();
                if (vehicles.Count == 0)
                {
                    return OperationResult<IEnumerable<PlanDto>>.Fail("No available vehicles to create a plan.", null);
                }

                var planList = new List<PlanDto>();
                var usedVehicleIds = new HashSet<int>();

                foreach (var zone in sortedZones)
                {
                    var remainingPeople = await GetRemainingPeopleFromCacheOrZone(zone);
                    if (remainingPeople <= 0)
                    {
                        continue; // Skip this zone if no people left
                    }

                    var candidateVehicles = FindCandidateVehicleForZone(zone, vehicles, distanceKm); // Implement logic to find vehicles for the zone
                    if (candidateVehicles.Count == 0)
                    {
                        continue; // Skip this zone if no vehicles found
                    }

                    while (remainingPeople > 0)
                    {
                        var bestVehicle = FindBestVehicle(candidateVehicles, usedVehicleIds, remainingPeople); // Find the best vehicle for the remaining people
                        if (bestVehicle == null)
                        {
                            break; // No suitable vehicle found, exit the loop
                        }
                        var (vehicle, distance) = bestVehicle.Value;

                        vehicle.SetAvailability(false); // Set vehicle as unavailable
                        var resultUpdate = await _vehicleRepo.UpdateAsync( vehicle.Id, vehicle); // Update vehicle status in the repository
                        if (resultUpdate == null)
                        {
                            vehicle.SetAvailability(true); // Rollback vehicle availability if update failed

                            continue;
                        }
                        usedVehicleIds.Add(vehicle.Id); // Add to used vehicle IDs

                        var eta = Calculator.CalETA(distance, vehicle.Speed);
                        var evacuatedPeople = Math.Min(vehicle.Capacity, remainingPeople);
                        remainingPeople -= evacuatedPeople;

                        var cretePlanDto = new CreatePlanDto
                        { 
                            ZoneId = zone.Id,
                            VehicleId = vehicle.Id,
                            NumberOfEvacuatedPeople = evacuatedPeople,
                            ETA = eta
                        };

                        var newPlan = await CreateNewPlan(cretePlanDto);
                        if (newPlan == null)
                        {
                            return OperationResult<IEnumerable<PlanDto>>.Fail("Failed to create a new plan.", null);
                        }
                        var zondId = zone.BusinessId;
                        var vehicleId = vehicle.BusinessId;

                        planList.Add(newPlan.ToDto(zondId, vehicleId));
                    }
                } // End of zone loop
                if (planList.Count == 0)
                {
                    return OperationResult<IEnumerable<PlanDto>>.Fail("No plans were created.", null);
                }

                return OperationResult<IEnumerable<PlanDto>>.Ok(planList, "Plans created successfully.");

            }
            catch (DbUpdateException dbEx)
            {
                return OperationResult<IEnumerable<PlanDto>>.Fail("Database update error occurred while creating the plan.", null, dbEx);
            }
            catch (ArgumentException argEx)
            {
                return OperationResult<IEnumerable<PlanDto>>.Fail(argEx.Message, null);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<PlanDto>>.Fail("An error occurred while creating the plan.", null, ex);
            }
        }

        private async Task<Plan> CreateNewPlan(CreatePlanDto createPlanDto)
        {

            var newPlan = createPlanDto.CreateToEntity();
            var result = await _planRepo.AddAsync(newPlan);
            return result;
        }

        private (Vehicle vehicle, double distance)? FindBestVehicle(Dictionary<Vehicle, double> candidateVehicles,HashSet<int> usedVehicleIds, int remainingPeople)
        {
            // กรณีที่ 1: รถที่ "บรรทุกได้ <= จำนวนที่เหลือ" → พยายามใช้รถเล็กก่อน
            var bestVehicle = candidateVehicles
                .Where(v => v.Key.Capacity <= remainingPeople && !usedVehicleIds.Contains(v.Key.Id))
                .OrderBy(v => v.Value) // ใกล้สุด
                .ThenByDescending(v => v.Key.Capacity) // ความจุมากที่สุดที่ยัง <= remaining
                .FirstOrDefault();

            if (bestVehicle.Key != null)
                return (bestVehicle.Key, bestVehicle.Value);

            // กรณีที่ 2: รถที่ "บรรทุกได้มากเกิน" → ถ้าข้างบนหาไม่ได้
            bestVehicle = candidateVehicles
                .Where(v => v.Key.Capacity > remainingPeople && !usedVehicleIds.Contains(v.Key.Id))
                .OrderBy(v => v.Value) // ใกล้สุด
                .ThenBy(v => v.Key.Capacity) // ใช้รถใหญ่ที่สุดที่ไม่ใหญ่เกินไป
                .FirstOrDefault();

            if (bestVehicle.Key != null)
                return (bestVehicle.Key, bestVehicle.Value);

            // ไม่เจอเลย
            return null;
        }

        private async Task<int> GetRemainingPeopleFromCacheOrZone(Zone zone)
        {
            var cacheKey = CreateCacheKeyForStatus(zone.Id);
            var cachedValue = await _cacheService.GetAsync<Zone?>(cacheKey);
            if (cachedValue != null)
            {
                // If found in cache, return the remaining people
                return cachedValue.NumberOfPeople; // Assuming NumberOfPeople is the total people in the zone
            }
            // If not in cache, get from the zone
            var remainingPeople = zone.NumberOfPeople; // Assuming NumberOfPeople is the total people in the zone
            await _cacheService.SetAsync(cacheKey, zone);
            return remainingPeople;
        }

        private Dictionary<Vehicle,double> FindCandidateVehicleForZone(Zone zone, List<Vehicle> vehicles, double maxDistance)
        {
            var candidateVehicles = vehicles
                .Where( v=> v.IsAvailable)
                .Select(v => new { Vehicle = v, Distance = 
                Calculator.CalDistanceKm(
                    zone.LocationCoordinates.Latitude,
                    zone.LocationCoordinates.Longitude,
                    v.LocationCoordinates.Latitude,
                    v.LocationCoordinates.Longitude) })
                .Where(v => v.Distance <= maxDistance)
                .OrderBy(v => v.Distance)
                .ToDictionary(v => v.Vehicle, v => v.Distance);

            return candidateVehicles;

        }

        private async Task<(Dictionary<int, string> ZoneMap, Dictionary<int, string> VehicleMap)> GetZoneAndVehicleMapsAsync()
        {
            var zoneTask = _zoneRepo.GetIdMapAsync();
            var vehicleTask = _vehicleRepo.GetIdMapAsync();

            await Task.WhenAll(zoneTask, vehicleTask);

            return (zoneTask.Result, vehicleTask.Result);
        }


        public async Task<OperationResult<IEnumerable<PlanDto>>> GetAllPlansAsync()
        {
            try 
            {
                var plans = await _planRepo.GetAllAsync();
                var (zoneIdMap, vehicleIdMap) = await GetZoneAndVehicleMapsAsync();
                if (zoneIdMap == null || vehicleIdMap == null)
                {
                    return OperationResult<IEnumerable<PlanDto>>.Fail("Zone or Vehicle ID map not found.", null);
                }
                var planDtos = plans.ToDto(zoneIdMap, vehicleIdMap);

                if (plans == null || !plans.Any())
                {
                    return OperationResult<IEnumerable<PlanDto>>.Fail("No plans found.", null);
                }
                return OperationResult<IEnumerable<PlanDto>>.Ok(planDtos, "Plans retrieved successfully.");
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<PlanDto>>.Fail("An error occurred while retrieving plans.", null, ex);
            }
        }

        public async Task<OperationResult<IEnumerable<StatusDto>>> GetAllStatusAsync()
        {
            try
            {
                var (zoneIdMap, vehicleIdMap) = await GetZoneAndVehicleMapsAsync();
                var statusCacheKeys = await _cacheService.GetAsync<HashSet<int>>(RedisCacheKays.StatusHashKey);
                if (statusCacheKeys != null && statusCacheKeys.Any())
                {
                    var statusList = new List<StatusDto>();
                    foreach (var cacheKey in statusCacheKeys)
                    {
                        var cachedStatus = await _cacheService.GetAsync<Status>(CreateCacheKeyForStatus(cacheKey));
                        if (cachedStatus != null)
                        {

                            statusList.Add(cachedStatus.ToDto
                                (
                                zoneIdMap[cachedStatus.ZoneId], 
                                vehicleIdMap[cachedStatus.LastVehicleIdUsed]
                                ));
                        }
                        else
                        {
                            return OperationResult<IEnumerable<StatusDto>>.Fail($"No status found for cache key {cacheKey}.", null);
                        }
                    }
                    return OperationResult<IEnumerable<StatusDto>>.Ok(statusList, "Statuses retrieved from cache successfully.");
                }
                else
                {
                    var statuses = await _statusRepo.GetAllAsync();
                    if (statuses == null || !statuses.Any())
                    {
                        return OperationResult<IEnumerable<StatusDto>>.Fail("No statuses found.", null);
                    }
                    
                    // If no cache keys, retrieve from repository
                    var zoneIds = statuses.Select(s => s.ZoneId).ToHashSet();
                    await _cacheService.SetAsync(RedisCacheKays.StatusHashKey, zoneIds);


                    // Update cache with all statuses
                    foreach (var status in statuses)
                    {
                        var cacheKey = CreateCacheKeyForStatus(status.ZoneId);
                        await _cacheService.SetAsync(cacheKey, status);
                    
                    }

                    return OperationResult<IEnumerable<StatusDto>>.Ok(statuses.ToDto(zoneIdMap, vehicleIdMap), "Statuses retrieved successfully.");
                }
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<StatusDto>>.Fail("An error occurred while retrieving statuses.", null, ex);
            }
        }

        public async Task<OperationResult<IEnumerable<PlanDto>>> GetPlanByZoneIdAsync(int zoneId)
        {
            try 
            {
                var (zoneIdMap, vehicleIdMap) = await GetZoneAndVehicleMapsAsync();
                var plans = await _planRepo.GetQuery().Where(p => p.ZoneId == zoneId).ToListAsync();
                if (plans == null || !plans.Any())
                {
                    return OperationResult<IEnumerable<PlanDto>>.Fail($"No plans found for zone ID {zoneId}.", null);
                }
                return OperationResult<IEnumerable<PlanDto>>.Ok(plans.ToDto(zoneIdMap, vehicleIdMap), "Plans retrieved successfully.");
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<PlanDto>>.Fail($"An error occurred while retrieving plans for zone ID {zoneId}.", null, ex);
            }
        }

        public async Task<OperationResult<StatusDto>> GetStatusByIdAsync(int statusId)
        {
            try
            {
                var (zoneIdMap, vehicleIdMap) = await GetZoneAndVehicleMapsAsync();
                var statusCache = await _cacheService.GetAsync<Status>(CreateCacheKeyForStatus(statusId));
                if (statusCache != null)
                {
                    return OperationResult<StatusDto>.Ok(statusCache.ToDto
                        (
                        zoneIdMap[statusCache.ZoneId],
                        vehicleIdMap[statusCache.LastVehicleIdUsed]
                        ), "Status retrieved from cache successfully.");
                }

                var status = await _statusRepo.GetByIdAsync(statusId);
                if (status == null)
                {
                    return OperationResult<StatusDto>.Fail($"Status with ID {statusId} not found.", null);
                }
                // Update cache
                await _cacheService.SetAsync(CreateCacheKeyForStatus(statusId), status);
                return OperationResult<StatusDto>.Ok
                    (status.ToDto
                    (
                        zoneIdMap[status.ZoneId],
                        vehicleIdMap[status.LastVehicleIdUsed]
                    ), "Status retrieved successfully.");
            }
            catch (Exception ex) 
            {
                return OperationResult<StatusDto>.Fail($"An error occurred while retrieving status with ID {statusId}.", null, ex);
            }
        }

        public async Task<OperationResult<IEnumerable<StatusDto>>> UpdateStatusByPlanAsync()
        {
            try 
            {
                var plans = await _planRepo.GetAllAsync();
                var planGroups = plans
                    .GroupBy(p => p.ZoneId)
                    .ToDictionary(g => g.Key, g => g.ToList());
                if (planGroups == null || !planGroups.Any())
                {
                    return OperationResult<IEnumerable<StatusDto>>.Fail("No plans found to update status.", null);
                }

                var statusList = new List<StatusDto>();

                var zoneIds = planGroups.Keys.ToList();


                foreach (var planGroup in planGroups)
                {
                    var zoneId = planGroup.Key;
                    var zone = await _zoneRepo.GetByIdAsync(zoneId);
                    if (zone == null)
                    {
                        return OperationResult<IEnumerable<StatusDto>>.Fail($"Zone with ID {zoneId} not found.", null);
                    }
                    var plansForZone = planGroup.Value;
                    // Calculate total evacuated people and ETA
                    var totalEvacuatedPeople = plansForZone.Sum(p => p.NumberOfEvacuatedPeople);
                    var remainingPeople = Math.Max(0, zone.NumberOfPeople - totalEvacuatedPeople);

                    // Replace this block in UpdateStatusByPlanAsync method

                    var lastVehicleId = plansForZone
                        .OrderByDescending(p => p.UpdatedAt)
                        .FirstOrDefault()?.VehicleId ?? 0;

                    var newStatus = new Status
                    (
                        zoneId,
                        totalEvacuatedPeople,
                        remainingPeople,
                        lastVehicleId
                    );


                    var status = await CreateOrUpdateStatus(newStatus);
                    if (status == null)
                    {
                        return OperationResult<IEnumerable<StatusDto>>.Fail($"Failed to create or update status for zone ID {zoneId}.", null);
                    }
                    var (zoneIdMap, vehicleIdMap) = await GetZoneAndVehicleMapsAsync();
                    
                    statusList.Add(status.ToDto
                        (
                            zoneIdMap[status.ZoneId],
                            vehicleIdMap[status.LastVehicleIdUsed]
                        ));

                    var cacheKey = CreateCacheKeyForStatus(zoneId);
                    // Update cache
                    await _cacheService.SetAsync(cacheKey, status);
                }// End of plan group loop

                if (statusList.Count == 0)
                {
                    return OperationResult<IEnumerable<StatusDto>>.Fail("No status updates were created.", null);
                }
                // Update cache zone IDs
                await _cacheService.SetAsync(RedisCacheKays.StatusHashKey, zoneIds);

                return OperationResult<IEnumerable<StatusDto>>.Ok(statusList, "Status updated successfully by plan.");

            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<StatusDto>>.Fail("An error occurred while updating status by plan.", null, ex);
            }
        }

        private async Task<Status?> CreateOrUpdateStatus(Status status)
        {
            var existingStatus = await _statusRepo.GetByIdAsync(status.ZoneId);
            if (existingStatus == null)
            {
                var result = await _statusRepo.AddAsync(status);
                return result;
            }
            else
            {
                existingStatus.Update
                    (
                        status.TotalEvacuatedPeople,
                        status.RemainingPeople,
                        status.LastVehicleIdUsed
                    );
                
                var result = await _statusRepo.UpdateAsync(existingStatus.ZoneId, existingStatus);
                return result;
            }

        }
    }
}
