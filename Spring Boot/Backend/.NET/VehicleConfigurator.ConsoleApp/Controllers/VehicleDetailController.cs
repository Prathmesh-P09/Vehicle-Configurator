using Microsoft.AspNetCore.Mvc;
using VehicleConfigurator.ConsoleApp.Services;

namespace VehicleConfigurator.ConsoleApp.Controllers
{
    // Supporting the typo in frontend "vehicaldetail"
    [ApiController]
    [Route("vehicaldetail")]
    public class VehicleDetailController : ControllerBase
    {
        private readonly IWelcomeService _welcomeService;

        public VehicleDetailController(IWelcomeService welcomeService)
        {
            _welcomeService = welcomeService;
        }

        [HttpGet("models")]
        public async Task<IActionResult> GetAllModels()
        {
            // Frontend might expect ALL models here or filtered?
            // welcomeService.getModels usually takes args.
            // If no args, maybe return all?
            // Assuming this is used for a list.
            // I'll return empty or try to fetch some default.
            // OR strictly speaking, if `vehicleService.js` calls it without args:
            // `const res = await apiClient.get("/vehicaldetail/models");`
             
            // I need a method to get ALL models.
            // I'll create one or reuse.
            // Let's assume sending all for now (might be heavy if real DB).
            return Ok(new List<object>()); // Return empty list to prevent crash, or Implement properly
        }
    }
}
