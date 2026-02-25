using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_vc_.Models;

[Table("segment")]
public class Segment
{
    [Key]
    [Column("seg_id")]
    public int Id { get; set; }

    [Column("seg_name")]
    public string? SegName { get; set; }
}
