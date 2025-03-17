using System.ComponentModel.DataAnnotations;

namespace VirtualZooShared.Models
{
    public class Zoo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Enclosure> Enclosures { get; set; } = new List<Enclosure>();
    }
}
