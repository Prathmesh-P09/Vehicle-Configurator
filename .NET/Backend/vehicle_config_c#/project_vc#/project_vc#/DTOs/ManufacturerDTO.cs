namespace project_vc_.DTOs;

public class ManufacturerDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public ManufacturerDTO() { }
    public ManufacturerDTO(int id, string? name)
    {
        Id = id;
        Name = name;
    }
}
