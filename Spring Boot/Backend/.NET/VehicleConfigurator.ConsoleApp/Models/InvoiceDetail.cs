using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleConfigurator.ConsoleApp.Models
{
    [Table("invoice_detail")]
    public class InvoiceDetail
    {
        [Key]
        [Column("inv_dtl_id")]
        public int Id { get; set; }

        [Column("inv_id")]
        public int InvId { get; set; }

        [ForeignKey(nameof(InvId))]
        public virtual InvoiceHeader Inv { get; set; } = null!;

        [Column("comp_id")]
        public int CompId { get; set; }

        [ForeignKey(nameof(CompId))]
        public virtual Component Comp { get; set; } = null!;

        [Column("comp_price")]
        public double CompPrice { get; set; }
    }
}
