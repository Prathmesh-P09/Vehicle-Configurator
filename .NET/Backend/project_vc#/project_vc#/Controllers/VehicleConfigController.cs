using Microsoft.AspNetCore.Mvc;
using project_vc_.DTOs;
using project_vc_.Services;

namespace project_vc_.Controllers;

[ApiController]
[Route("vehicle")]
public class VehicleConfigController : ControllerBase
{
    private readonly IVehicleService _service;

    public VehicleConfigController(IVehicleService service)
    {
        _service = service;
    }

    [HttpGet("{modelId}/standard")]
    public async Task<ActionResult<List<ComponentDropdownDTO>>> GetStandardComponents(int modelId)
    {
        return Ok(await _service.GetConfigurableComponentsAsync(modelId, "S"));
    }

    [HttpGet("{modelId}/interior")]
    public async Task<ActionResult<List<ComponentDropdownDTO>>> GetInteriorComponents(int modelId)
    {
        return Ok(await _service.GetConfigurableComponentsAsync(modelId, "I"));
    }

    [HttpGet("{modelId}/exterior")]
    public async Task<ActionResult<List<ComponentDropdownDTO>>> GetExteriorComponents(int modelId)
    {
        return Ok(await _service.GetConfigurableComponentsAsync(modelId, "E"));
    }

    [HttpGet("{modelId}/accessories")]
    public async Task<ActionResult<List<ComponentDropdownDTO>>> GetAccessoryComponents(int modelId)
    {
        return Ok(await _service.GetConfigurableComponentsAsync(modelId, "C"));
    }
}
