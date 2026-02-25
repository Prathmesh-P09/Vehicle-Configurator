using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_vc_.Models;

[Table("invoice_detail")]
public class InvoiceDetail
{
    [Key]
    [Column("inv_dtl_id")]
    public int Id { get; set; }

    [Column("comp_id")]
    public int? CompId { get; set; }

    [ForeignKey(nameof(CompId))]
    public Component? Comp { get; set; }

    [Column("inv_id")]
    public int? InvId { get; set; }

    [ForeignKey(nameof(InvId))]
    public InvoiceHeader? Inv { get; set; }

    [Column("comp_price")]
    public double CompPrice { get; set; }
}
