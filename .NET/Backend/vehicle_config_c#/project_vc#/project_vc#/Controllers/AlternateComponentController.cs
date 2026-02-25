using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_vc_.DTOs;
using project_vc_.Services;

namespace project_vc_.Controllers;

[ApiController]
[Route("api/alternate-component")]
public class AlternateComponentController : ControllerBase
{
    private readonly IVehicleService _service;

    public AlternateComponentController(IVehicleService service)
    {
        _service = service;
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveAlternateComponent([FromBody] AlternateComponentSaveDTO dto)
    {
        await _service.SaveAlternateComponentsAsync(dto);
        return Ok("Alternate components saved successfully");
    }
}
