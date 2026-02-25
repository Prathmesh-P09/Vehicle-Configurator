namespace VehicleConfigurator.ConsoleApp.DTOs
{
    public class SegmentDto
    {
        public int Id { get; set; }
        public string SegName { get; set; } = string.Empty;
        public string Name => SegName; // Alias for frontend
    }

    public class ManufacturerDto
    {
        public int Id { get; set; }
        public string MfgName { get; set; } = string.Empty;
        public string Name => MfgName; // Alias
    }

    public class ModelDto
    {
        public int Id { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public string Name => ModelName; // Alias
        public double Price { get; set; }
        public int MinQty { get; set; }
        public string ImagePath { get; set; } = string.Empty;
    }

    public class OptionDto
    {
        public string AltCompName { get; set; } = string.Empty;
        public int AltCompId { get; set; }
        public double DeltaPrice { get; set; }
        public int CompId { get; set; } // Base Component ID associated with this option
        public int ModelId { get; set; }
    }

    public class ComponentConfigDto
    {
        public string ComponentName { get; set; } = string.Empty; // e.g. "Color", "Rim"
        public List<OptionDto> Options { get; set; } = new List<OptionDto>();
    }
}
