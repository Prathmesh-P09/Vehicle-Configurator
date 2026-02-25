using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_vc_.Models;

[Table("manufacturer")]
public class Manufacturer
{
    [Key]
    [Column("mfg_id")]
    public int Id { get; set; }

    [Column("mfg_name")]
    public string? MfgName { get; set; }
}
