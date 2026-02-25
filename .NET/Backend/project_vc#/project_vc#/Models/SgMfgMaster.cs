using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_vc_.Models;

[Table("sg_mfg_master")]
public class SgMfgMaster
{
    [Key]
    [Column("sgmf_id")]
    public int Id { get; set; }

    [Column("mfg_id")]
    public int MfgId { get; set; }

    [ForeignKey(nameof(MfgId))]
    public Manufacturer? Mfg { get; set; }

    [Column("seg_id")]
    public int SegId { get; set; }

    [ForeignKey(nameof(SegId))]
    public Segment? Seg { get; set; }
}
