﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using VirtualZooShared.Enums;

namespace VirtualZooShared.Models
{
    public class Animal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Species { get; set; }

        public int? CategoryId { get; set; }

        public Size Size { get; set; }

        public DietaryClass DietaryClass { get; set; }

        public ActivityPattern ActivityPattern { get; set; }

        public string Prey { get; set; }

        public int? EnclosureId { get; set; }

        public double SpaceRequirement { get; set; } //square meters/Animal

        public SecurityLevel SecurityRequirement { get; set; }
    }
}
