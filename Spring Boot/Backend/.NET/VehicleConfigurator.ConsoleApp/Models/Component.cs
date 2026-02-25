using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleConfigurator.ConsoleApp.Models
{
    [Table("component")]
    public class Component
    {
        [Key]
        [Column("comp_id")]
        public int Id { get; set; }

        [Column("comp_name")]
        public string CompName { get; set; } = string.Empty;

        [Column("comp_type")]
        public string Type { get; set; } = string.Empty; // S, I, E, C

        [Column("price")]
        public double Price { get; set; }
    }
}
