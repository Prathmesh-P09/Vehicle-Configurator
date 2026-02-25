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

    [HttpGet("anonymous-confirm")]
    [AllowAnonymous]
    public IActionResult AnonymousConfirm()
    {
        return Ok("AnonymousConfirm route reached - routing is working for InvoiceController");
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmOrder([FromBody] InvoiceRequestDTO dto)
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username)) 
        {
            var claims = User.Claims.Select(c => $"{c.Type}: {c.Value}");
            return Unauthorized(new { message = "Username claim not found in InvoiceController", claims });
        }

        await _service.GenerateInvoiceAsync(dto, username);
        return Ok("Invoice generated");
    }
}
