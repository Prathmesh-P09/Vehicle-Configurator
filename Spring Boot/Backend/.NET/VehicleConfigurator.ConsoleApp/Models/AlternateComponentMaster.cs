using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleConfigurator.ConsoleApp.Models
{
    [Table("alternate_component_master")]
    public class AlternateComponentMaster
    {
        [Key]
        [Column("alt_id")]
        public int Id { get; set; }

        [Column("delta_price")]
        public double DeltaPrice { get; set; }

        [Column("model_id")]
        public int ModelId { get; set; }

        [ForeignKey(nameof(ModelId))]
        public virtual Model Model { get; set; } = null!;

        [Column("comp_id")]
        public int CompId { get; set; } // Base Component

        [ForeignKey(nameof(CompId))]
        public virtual Component Comp { get; set; } = null!;

        [Column("alt_comp_id")]
        public int AltCompId { get; set; } // Alternate Component

        [ForeignKey(nameof(AltCompId))]
        public virtual Component AltComp { get; set; } = null!;
    }
}
