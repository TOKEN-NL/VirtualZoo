using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtualZoo.Enums;

namespace VirtualZoo.Models
{
    public class Animal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Species { get; set; }

        public int CategoryId { get; set; }

        // De Category-eigenschap is een navigatie-eigenschap. Navigatie-eigenschappen bevatten andere entiteiten die gerelateerd zijn aan deze entiteit.
        public Category Category { get; set; }

        public Size Size { get; set; }

        public DietaryClass DietaryClass { get; set; }

        public ActivityPattern ActivityPattern { get; set; }

        public string Prey { get; set; }

        public int EnclosureId { get; set; }

        // De Enclosure-eigenschap is een navigatie-eigenschap. Navigatie-eigenschappen bevatten andere entiteiten die gerelateerd zijn aan deze entiteit.
        public Enclosure Enclosure { get; set; }

        public double SpaceRequirement { get; set; } //square meters/Animal

        public SecurityLevel SecurityRequirement { get; set; }
    }
}
