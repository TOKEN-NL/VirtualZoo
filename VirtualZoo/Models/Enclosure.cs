using System.ComponentModel.DataAnnotations;

namespace VirtualZoo.Models
{
    public class Enclosure
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Animal> Animals { get; set; } = new List<Animal>();
    }
}
