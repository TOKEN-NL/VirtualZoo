using System.ComponentModel.DataAnnotations;
using VirtualZoo.Enums;

namespace VirtualZoo.Models
{
    public class Enclosure
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Animal> Animals { get; set; }

        public Climate Climate { get; set; }

        public HabitatType HabitatType { get; set; }

        public SecurityLevel SecurityLevel { get; set; }

        public double Size { get; set; } // in square meters
    }
}
