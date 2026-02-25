using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_vc_.Models;

[Table("alternate_component_master")]
public class AlternateComponentMaster
{
    [Key]
    [Column("alt_id")]
    public int Id { get; set; }

    [Column("delta_price")]
    public double? DeltaPrice { get; set; }

    [Column("alt_comp_id")]
    public int? AltCompId { get; set; }

    [ForeignKey(nameof(AltCompId))]
    public Component? AltComp { get; set; }

    [Column("comp_id")]
    public int? CompId { get; set; }

    [ForeignKey(nameof(CompId))]
    public Component? Comp { get; set; }

    [Column("model_id")]
    public int? ModelId { get; set; }

    [ForeignKey(nameof(ModelId))]
    public VehicleModel? Model { get; set; }
}
