using VehicleConfigurator.ConsoleApp.DTOs;

namespace VehicleConfigurator.ConsoleApp.Services
{
    public interface IInvoiceManager
    {
        Task<InvoiceResponseDto> GenerateInvoiceAsync(InvoiceRequestDto request);
    }
}
