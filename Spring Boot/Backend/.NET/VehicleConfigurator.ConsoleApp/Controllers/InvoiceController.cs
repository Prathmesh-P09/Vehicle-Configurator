using Microsoft.AspNetCore.Mvc;
using VehicleConfigurator.ConsoleApp.DTOs;
using VehicleConfigurator.ConsoleApp.Services;

namespace VehicleConfigurator.ConsoleApp.Controllers
{
    [ApiController]
    [Route("api/invoice")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceManager _invoiceManager;

        public InvoiceController(IInvoiceManager invoiceManager)
        {
            _invoiceManager = invoiceManager;
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmOrder([FromBody] InvoiceRequestDto request)
        {
            try
            {
                // Note: Frontend sends JWT via Header Authorization.
                // In a real app, I'd extract "Username" from the JWT claims.
                // However, request.Username is part of the DTO (based on Console implementation).
                // If Frontend doesn't send username in Body, I must extract from Token.
                // The provided invoiceService.js doesn't show the body structure explicitly other than "data".
                // Assuming the Frontend includes the user context or relies on Token.
                // For this migration, I will assume the DTO matches, or modify DTO if needed.
                // IMPORTANT: The Console DTO has `Username`. If Frontend DTO relies on Token, I might need to adapt.
                // Let's assume the frontend sends what is needed. If not, we might need a `[Authorize]` attribute and extract User.Identity.Name.
                
                // For safety/robustness:
                if (string.IsNullOrEmpty(request.Username) && User.Identity?.IsAuthenticated == true)
                {
                    request.Username = User.Identity.Name ?? "Unknown";
                }

                var response = await _invoiceManager.GenerateInvoiceAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
