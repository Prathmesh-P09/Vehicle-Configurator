using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleConfigurator.ConsoleApp.Models
{
    [Table("manufacturer")]
    public class Manufacturer
    {
        [Key]
        [Column("mfg_id")]
        public int Id { get; set; }

        [Column("mfg_name")]
        public string MfgName { get; set; } = string.Empty;

        // Navigation
        // public virtual ICollection<Model> Models { get; set; } = new List<Model>();
        // public virtual ICollection<SgMfgMaster> SgMfgMasters { get; set; } = new List<SgMfgMaster>();
    }
}
