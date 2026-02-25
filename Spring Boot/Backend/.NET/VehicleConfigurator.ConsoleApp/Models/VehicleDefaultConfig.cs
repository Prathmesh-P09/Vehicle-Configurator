using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleConfigurator.ConsoleApp.Models
{
    [Table("vehicle_default_config")]
    public class VehicleDefaultConfig
    {
        [Key]
        [Column("config_id")]
        public int Id { get; set; }

        [Column("model_id")]
        public int ModelId { get; set; }

        [ForeignKey(nameof(ModelId))]
        public virtual Model Model { get; set; } = null!;

        [Column("comp_id")]
        public int CompId { get; set; }

        [ForeignKey(nameof(CompId))]
        public virtual Component Comp { get; set; } = null!;

        [Column("comp_type")]
        public string CompType { get; set; } = string.Empty;
    }
}
