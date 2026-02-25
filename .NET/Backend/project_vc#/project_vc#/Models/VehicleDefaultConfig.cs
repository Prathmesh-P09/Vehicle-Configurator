using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_vc_.Models;

[Table("vehicle_default_config")]
public class VehicleDefaultConfig
{
    [Key]
    [Column("config_id")]
    public int Id { get; set; }

    [Column("model_id")]
    public int ModelId { get; set; }

    [ForeignKey(nameof(ModelId))]
    public VehicleModel? Model { get; set; }

    [Column("comp_id")]
    public int CompId { get; set; }

    [ForeignKey(nameof(CompId))]
    public Component? Comp { get; set; }

    [Column("comp_type")]
    public string? CompType { get; set; }
}
