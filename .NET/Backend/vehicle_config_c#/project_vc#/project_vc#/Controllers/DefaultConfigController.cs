using Microsoft.AspNetCore.Mvc;
using project_vc_.DTOs;
using project_vc_.Services;

namespace project_vc_.Controllers;

[ApiController]
[Route("api/default-config")]
public class DefaultConfigController : ControllerBase
{
    private readonly IVehicleService _service;

    public DefaultConfigController(IVehicleService service)
    {
        _service = service;
    }

    [HttpGet("{modelId}")]
    public async Task<ActionResult<DefaultConfigResponseDTO>> GetDefaultConfig(int modelId, [FromQuery] int qty = 1)
    {
        try
        {
            var result = await _service.GetDefaultConfigurationAsync(modelId, qty);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("conf/{modelId}")]
    public IActionResult GetDefault(int modelId)
    {
        // TODO: Implement GetDefaultConfiguration returning List<DefaultConfigurationDTO>
        return Ok(new List<DefaultConfigurationDTO>()); 
    }
}
