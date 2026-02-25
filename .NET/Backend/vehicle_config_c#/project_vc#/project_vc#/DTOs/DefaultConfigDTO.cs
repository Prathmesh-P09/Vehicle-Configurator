using System.Collections.Generic;

namespace project_vc_.DTOs;

public class DefaultConfigDTO
{
    public int? ModelId { get; set; }
    public string? ModelName { get; set; }
    public string? SegmentName { get; set; }
    public string? ManufacturerName { get; set; }
    public double? BasePrice { get; set; }
    public int? MinQuantity { get; set; }
    public double? TotalPrice { get; set; }

    public DefaultConfigDTO() { }
    public DefaultConfigDTO(int? modelId, string? modelName, string? segmentName, string? manufacturerName, double? basePrice, int? minQuantity, double? totalPrice)
    {
        ModelId = modelId;
        ModelName = modelName;
        SegmentName = segmentName;
        ManufacturerName = manufacturerName;
        BasePrice = basePrice;
        MinQuantity = minQuantity;
        TotalPrice = totalPrice;
    }
}
