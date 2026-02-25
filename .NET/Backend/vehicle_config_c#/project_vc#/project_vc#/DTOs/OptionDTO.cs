namespace project_vc_.DTOs;

public class OptionDTO
{
    public int? CompId { get; set; }
    public string? SubType { get; set; }
    public double? Price { get; set; }

    public OptionDTO() { }
    public OptionDTO(int? compId, string? subType, double? price)
    {
        CompId = compId;
        SubType = subType;
        Price = price;
    }
}
