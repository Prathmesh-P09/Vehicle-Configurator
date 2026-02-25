using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_vc_.DTOs;
using project_vc_.Services;
using System.Security.Claims;

namespace project_vc_.Controllers;

[ApiController]
[Route("api/invoice")]
[Authorize]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _service;

    public InvoiceController(IInvoiceService service)
    {
        _service = service;
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmOrder([FromBody] InvoiceRequestDTO dto)
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        await _service.GenerateInvoiceAsync(dto, username);
        return Ok("Invoice generated");
    }
}
