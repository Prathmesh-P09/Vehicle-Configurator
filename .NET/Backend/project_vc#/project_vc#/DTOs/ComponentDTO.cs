namespace project_vc_.DTOs;

public class ComponentDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public ComponentDTO() { }
    public ComponentDTO(int id, string? name)
    {
        Id = id;
        Name = name;
    }
}
