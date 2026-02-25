using System.Collections.Generic;

namespace project_vc_.DTOs;

public class WelcomeResponseDTO
{
    public List<SegmentDTO>? Segments { get; set; }
    public List<ManufacturerDTO>? Manufacturers { get; set; }
    public List<ModelDTO>? Models { get; set; }

    public WelcomeResponseDTO() { }
    public WelcomeResponseDTO(List<SegmentDTO>? segments, List<ManufacturerDTO>? manufacturers, List<ModelDTO>? models)
    {
        Segments = segments;
        Manufacturers = manufacturers;
        Models = models;
    }
}
