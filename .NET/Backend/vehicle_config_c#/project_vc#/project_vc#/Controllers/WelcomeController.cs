using Microsoft.AspNetCore.Mvc;
using project_vc_.DTOs;
using project_vc_.Services;

namespace project_vc_.Controllers;

[ApiController]
[Route("api/welcome")]
public class WelcomeController : ControllerBase
{
    private readonly IVehicleService _service;

    public WelcomeController(IVehicleService service)
    {
        _service = service;
    }

    [HttpGet("segments")]
    public async Task<ActionResult<List<SegmentDTO>>> GetSegments()
    {
        return Ok(await _service.GetAllSegmentsAsync());
    }

    [HttpGet("manufacturers/{segId}")]
    public async Task<ActionResult<List<ManufacturerDTO>>> GetManufacturers(int segId)
    {
        return Ok(await _service.GetManufacturersBySegmentAsync(segId));
    }

    [HttpGet("models")]
    public async Task<ActionResult<List<ModelDTO>>> GetModels([FromQuery] int segId, [FromQuery] int mfgId)
    {
        return Ok(await _service.GetModelsAsync(segId, mfgId));
    }
}
