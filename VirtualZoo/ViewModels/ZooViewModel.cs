using VirtualZooShared.Models;
using System.Collections.Generic;

namespace VirtualZoo.ViewModels
{
    public class ZooViewModel
    {
        public List<Animal> Animals { get; set; } = new();
        public List<Enclosure> Enclosures { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
    }
}
