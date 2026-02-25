namespace project_vc_.DTOs;

public class SegmentDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int MinQty { get; set; }

    public SegmentDTO() { }
    public SegmentDTO(int id, string? name, int minQty)
    {
        Id = id;
        Name = name;
        MinQty = minQty;
    }
}
