using Microsoft.AspNetCore.Mvc;
using VehicleConfigurator.ConsoleApp.Services;

namespace VehicleConfigurator.ConsoleApp.Controllers
{
    // Mapped from vehicleService.js
    // Routes: /vehicle/{id}/standard, /interior, etc.
    
    [ApiController]
    [Route("vehicle")]
    public class VehicleConfigController : ControllerBase
    {
        private readonly IVehicleManager _vehicleManager;

        public VehicleConfigController(IVehicleManager vehicleManager)
        {
            _vehicleManager = vehicleManager;
        }

        [HttpGet("{modelId}/standard")]
        public async Task<IActionResult> GetStandard(int modelId)
        {
            var data = await _vehicleManager.GetConfigurableComponentsAsync(modelId, "S");
            return Ok(data);
        }

        [HttpGet("{modelId}/interior")]
        public async Task<IActionResult> GetInterior(int modelId)
        {
            var data = await _vehicleManager.GetConfigurableComponentsAsync(modelId, "I");
            return Ok(data);
        }

        [HttpGet("{modelId}/exterior")]
        public async Task<IActionResult> GetExterior(int modelId)
        {
            var data = await _vehicleManager.GetConfigurableComponentsAsync(modelId, "E");
            return Ok(data);
        }

        [HttpGet("{modelId}/accessories")]
        public async Task<IActionResult> GetAccessories(int modelId)
        {
            var data = await _vehicleManager.GetConfigurableComponentsAsync(modelId, "C");
            return Ok(data);
        }
    }

    [ApiController]
    [Route("api/default-config")]
    public class DefaultConfigController : ControllerBase
    {
        private readonly IDefaultConfigService _service;

        public DefaultConfigController(IDefaultConfigService service)
        {
            _service = service;
        }

        [HttpGet("{modelId}")]
        public async Task<IActionResult> GetDefaults(int modelId)
        {
            var data = await _service.GetDefaultConfigAsync(modelId);
            return Ok(data);
        }
    }
}
