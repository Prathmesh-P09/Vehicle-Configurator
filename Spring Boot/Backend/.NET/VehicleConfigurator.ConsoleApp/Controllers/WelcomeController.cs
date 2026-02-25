using Microsoft.AspNetCore.Mvc;
using VehicleConfigurator.ConsoleApp.Services;

namespace VehicleConfigurator.ConsoleApp.Controllers
{
    // Frontend prefixes: 
    // Segments: /api/welcome/segments
    // Models: /vehicaldetail/models ?? No, service says: /vehicaldetail/models
    // But WelcomeService.js says: /api/welcome/models?mfgId=...
    // I will support BOTH if needed, or stick to what WelcomeService.js uses.
    // WelcomeService.js: API = "http://localhost:8080/api/welcome"
    // Methods: ${API}/segments, ${API}/models
    
    [ApiController]
    [Route("api/welcome")]
    public class WelcomeController : ControllerBase
    {
        private readonly IWelcomeService _welcomeService;

        public WelcomeController(IWelcomeService welcomeService)
        {
            _welcomeService = welcomeService;
        }

        [HttpGet("segments")]
        public async Task<IActionResult> GetSegments()
        {
            var segments = await _welcomeService.GetAllSegmentsAsync();
            return Ok(segments);
        }

        [HttpGet("manufacturers/{segId}")]
        public async Task<IActionResult> GetManufacturers(int segId)
        {
            var mfgs = await _welcomeService.GetManufacturersAsync(segId);
            return Ok(mfgs);
        }

        [HttpGet("models")]
        public async Task<IActionResult> GetModels([FromQuery] int segId, [FromQuery] int mfgId)
        {
            var models = await _welcomeService.GetModelsAsync(segId, mfgId);
            return Ok(models);
        }
    }

    // Additional controller for VehicleDetails if referenced elsewhere
    [ApiController]
    [Route("api/models")]
    public class ModelController : ControllerBase
    {
        private readonly IWelcomeService _welcomeService;

        public ModelController(IWelcomeService welcomeService)
        {
            _welcomeService = welcomeService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetModelDetails(int id)
        {
             var model = await _welcomeService.GetModelByIdAsync(id);
             if(model == null) return NotFound();
             return Ok(model);
        }
    }
}
