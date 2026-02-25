namespace project_vc_.DTOs;

public class ModelDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public double Price { get; set; }
    public int MinQty { get; set; }
    public string? ImagePath { get; set; }

    public ModelDTO() { }
    public ModelDTO(int id, string? name, double price, int minQty, string? imagePath)
    {
        Id = id;
        Name = name;
        Price = price;
        MinQty = minQty;
        ImagePath = imagePath;
    }
}
