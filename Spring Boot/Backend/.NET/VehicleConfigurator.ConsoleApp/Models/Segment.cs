using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleConfigurator.ConsoleApp.Models
{
    [Table("segment")]
    public class Segment
    {
        [Key]
        [Column("seg_id")]
        public int Id { get; set; }

        [Column("seg_name")]
        public string SegName { get; set; } = string.Empty;

        // Navigation
        // public virtual ICollection<Model> Models { get; set; } = new List<Model>();
        // public virtual ICollection<SgMfgMaster> SgMfgMasters { get; set; } = new List<SgMfgMaster>();
    }
}
