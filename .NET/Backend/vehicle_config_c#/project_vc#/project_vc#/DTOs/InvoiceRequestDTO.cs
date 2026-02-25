using System.Collections.Generic;

namespace project_vc_.DTOs;

public class InvoiceRequestDTO
{
    public int? ModelId { get; set; }
    public int? Qty { get; set; }
    public string? CustomerDetail { get; set; }
    public List<AlternateComponentDTO>? Components { get; set; }
}
