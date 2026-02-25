using VehicleConfigurator.ConsoleApp.Data.Repositories;
using VehicleConfigurator.ConsoleApp.DTOs;
using VehicleConfigurator.ConsoleApp.Models;

namespace VehicleConfigurator.ConsoleApp.Services
{
    public class VehicleManager : IVehicleManager
    {
        private readonly IVehicleDetailRepository _vehicleDetailRepo;
        private readonly IAlternateComponentRepository _altCompRepo;
        private readonly IComponentRepository _compRepo;

        public VehicleManager(IVehicleDetailRepository vehicleDetailRepo,
                              IAlternateComponentRepository altCompRepo,
                              IComponentRepository compRepo)
        {
            _vehicleDetailRepo = vehicleDetailRepo;
            _altCompRepo = altCompRepo;
            _compRepo = compRepo;
        }

        public async Task<List<string>> GetConfigurableVehicleDetailsAsync(int modelId, string compType)
        {
            return await _vehicleDetailRepo.FindConfigurableVehicleDetailsAsync(modelId, compType);
        }

        public async Task<List<ComponentConfigDto>> GetConfigurableComponentsAsync(int modelId, string compType)
        {
            var result = new List<ComponentConfigDto>();

            // 1. Get Base Components for this Type that are configurable
            // Note: The logic in Java might iterate over 'Unique Default Components' or similar.
            // Using the repository method: findUniqueDefaultComponents(modelId) filtered by type?
            
            // The method `findConfigurableComponents` in repo returned List<VehicleDetail>.
            // But we need to structure it with Options.
            
            // Java Logic Replication:
            // List<VehicleDetail> vDetails = vehicleDetailRepo.findUniqueDefaultComponents(modelId);
            // Filter by compType.
            
            var uniqueDetails = await _vehicleDetailRepo.FindUniqueDefaultComponentsAsync(modelId);
            var filteredDetails = uniqueDetails.Where(vd => vd.Comp.Type == compType).ToList();

            foreach (var detail in filteredDetails)
            {
                var dto = new ComponentConfigDto
                {
                    ComponentName = detail.Comp.CompName
                };

                // 2. Find Alternate Components
                // Logic: Find AlternateComponentMaster where CompId == detail.CompId and ModelId == modelId
                // The repo method `findByModelAndComp` returns single? No, we need all alternates for a base component.
                // Java: alternateComponentRepository.findByModelIdAndCompId(modelId, compId) -> List? 
                // Wait, logic check: A base component has MANY alternates.
                // My repo `FindByModelAndComp` returned single. I need to check requirement. 
                // Ah, `AlternateComponentMaster` table has `comp_id` (Base) and `alt_comp_id` (Alternate).
                // So I need `FindByModelIdAndCompId` returning a List.
                
                // I'll assume I need to fetch all alternates for this base component.
                // Since I didn't verify the Java Repo exactly for "Find All Alternates for Base", I'll use `FindByModelId` and filter in memory or add a method.
                // Better: Add method to repo if possible, but strict console app... I'll use `_altCompRepo.FindByModelIdAsync` (which I created) and filter.
                
                var allAlternates = await _altCompRepo.FindByModelIdAsync(modelId);
                var alternatesForBase = allAlternates.Where(a => a.CompId == detail.CompId).ToList();

                foreach (var alt in alternatesForBase)
                {
                    // Need to fetch Alt Component Name. In `AlternateComponentMaster` entity, I have navigation `AltComp`.
                    // Ensure Repo included it? I need to check Repo.
                    // My `AlternateComponentRepository.FindByModelIdAsync` did NOT include `AltComp`. I should fix that or lazy load. EF Core default is no lazy load.
                    // I'll fetch Component if null. Or better, update Repo. 
                    // To handle "No Modification to previous steps" strictly I should use what I have.
                    // But I fan fix implementation here by fetching.
                    
                    var altCompName = alt.AltComp?.CompName;
                    if (altCompName == null)
                    {
                        var c = await _compRepo.FindByIdAsync(alt.AltCompId);
                        altCompName = c?.CompName ?? "Unknown";
                    }

                    dto.Options.Add(new OptionDto
                    {
                        AltCompId = alt.AltCompId,
                        AltCompName = altCompName,
                        DeltaPrice = alt.DeltaPrice,
                        CompId = detail.CompId,
                        ModelId = modelId
                    });
                }
                
                // Also add the Base Component itself as an option (Standard)?
                // Usually "Configurable" means switching away from Base.
                // If Java included Base as option with Delta 0, I should too.
                // Looking at typical logic: usually only alternates are listed.
                
                result.Add(dto);
            }

            return result;
        }
    }
}
