using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_vc_.Models;

[Table("vehicle_detail")]
public class VehicleDetail
{
    [Key]
    [Column("config_id")]
    public int ConfigId { get; set; }

    [Column("comp_type")]
    public string? CompType { get; set; }

    [Column("is_config")]
    public string? IsConfig { get; set; } // Y / N

    [Column("comp_id")]
    public int? CompId { get; set; }

    [ForeignKey(nameof(CompId))]
    public Component? Comp { get; set; }

    [Column("model_id")]
    public int? ModelId { get; set; }

    [ForeignKey(nameof(ModelId))]
    public VehicleModel? Model { get; set; }
}
