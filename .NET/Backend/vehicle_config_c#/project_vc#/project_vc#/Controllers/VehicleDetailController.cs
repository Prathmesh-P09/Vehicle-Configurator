using Microsoft.AspNetCore.Mvc;
using project_vc_.DTOs;
using project_vc_.Services;

namespace project_vc_.Controllers;

[ApiController]
[Route("vehicaldetail")]
public class VehicleDetailController : ControllerBase
{
    private readonly IVehicleService _service;

    public VehicleDetailController(IVehicleService service)
    {
        _service = service;
    }

    [HttpPost("config")]
    public async Task<ActionResult<List<string>>> ConfigurableComponent([FromBody] IsConfigurationDTO dto)
    {
        if(dto.ModelId == null || dto.CompType == null) return BadRequest();
        return Ok(await _service.GetConfigurableComponentNamesAsync(dto.ModelId.Value, dto.CompType));
    }
}
