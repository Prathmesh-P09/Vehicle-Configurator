using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleConfigurator.ConsoleApp.Models
{
    [Table("sg_mfg_master")]
    public class SgMfgMaster
    {
        [Key]
        [Column("sgmf_id")]
        public int Id { get; set; }

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
