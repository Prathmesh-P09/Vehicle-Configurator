using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleConfigurator.ConsoleApp.Models
{
    [Table("invoice_header")]
    public class InvoiceHeader
    {
        [Key]
        [Column("inv_id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        [Column("model_id")]
        public int ModelId { get; set; }

        [ForeignKey(nameof(ModelId))]
        public virtual Model Model { get; set; } = null!;

        [Column("qty")]
        public int Qty { get; set; }

        [Column("base_amt")]
        public double BaseAmt { get; set; }

        [Column("tax")]
        public double Tax { get; set; }

        [Column("total_amt")]
        public double TotalAmt { get; set; }

        [Column("inv_date")]
        public DateOnly InvDate { get; set; } // .NET 8 DateOnly maps to SQL DATE

        [Column("status")]
        [EnumDataType(typeof(InvoiceStatus))]
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;

        [Column("customer_detail")]
        public string CustomerDetail { get; set; } = string.Empty;
    }
}
