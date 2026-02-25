using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_vc_.Models;

[Table("component")]
public class Component
{
    [Key]
    [Column("comp_id")]
    public int CompId { get; set; }

    [Column("comp_name")]
    public string? CompName { get; set; }

    [Column("comp_type")]
    public string? Type { get; set; }

    [Column("price")]
    public double Price { get; set; }

    public ICollection<AlternateComponentMaster> AlternateComponentMasters { get; set; } = new List<AlternateComponentMaster>();
    public ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
    public ICollection<VehicleDetail> VehicleDetails { get; set; } = new List<VehicleDetail>();
}
