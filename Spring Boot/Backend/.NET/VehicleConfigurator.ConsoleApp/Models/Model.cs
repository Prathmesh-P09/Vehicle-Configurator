using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleConfigurator.ConsoleApp.Models
{
    [Table("model")]
    public class Model
    {
        [Key]
        [Column("model_id")]
        public int Id { get; set; }

        [Column("model_name")]
        public string ModelName { get; set; } = string.Empty;

        [Column("price")]
        public double Price { get; set; }

        [Column("min_qty")]
        public int MinQty { get; set; }

        [Column("img_path")]
        public string? ImgPath { get; set; } // Nullable in Java? Check. Assigned String.

        [Column("mfg_id")]
        public int MfgId { get; set; }

        [ForeignKey(nameof(MfgId))]
        public virtual Manufacturer Mfg { get; set; } = null!;

        [Column("seg_id")]
        public int SegId { get; set; }

        [ForeignKey(nameof(SegId))]
        public virtual Segment Seg { get; set; } = null!;
    }
}
