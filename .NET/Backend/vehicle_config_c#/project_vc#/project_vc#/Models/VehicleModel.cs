using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_vc_.Models;

[Table("model")]
public class VehicleModel
{
    [Key]
    [Column("model_id")]
    public int Id { get; set; }

    [Column("img_path")]
    public string? ImgPath { get; set; }

    [Column("min_qty")]
    public int MinQty { get; set; }

    [Column("model_name")]
    public string? ModelName { get; set; }

    [Column("mfg_id")]
    public int? MfgId { get; set; }
    
    [ForeignKey(nameof(MfgId))]
    public Manufacturer? Mfg { get; set; }

    [Column("seg_id")]
    public int? SegId { get; set; }

    [ForeignKey(nameof(SegId))]
    public Segment? Seg { get; set; }

    [Column("price")]
    public double Price { get; set; }
}
