namespace project_vc_.DTOs;

public class DefaultConfigurationDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? CompType { get; set; }

    public DefaultConfigurationDTO() { }
    public DefaultConfigurationDTO(int id, string? name, string? compType)
    {
        Id = id;
        Name = name;
        CompType = compType;
    }
}
