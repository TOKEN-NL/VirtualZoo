using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualZoo.Models
{
    public class Animal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int CategoryId { get; set; }

        // The Category property is a navigation property. Navigation properties hold other entities that are related to this entity.
        public Category Category { get; set; }

        public int EnclosureId { get; set; }

        // The Enclosure property is a navigation property. Navigation properties hold other entities that are related to this entity.
        public Enclosure Enclosure { get; set; }
    }
}
