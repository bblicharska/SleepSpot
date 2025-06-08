using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyService.Domain.Models
{
    public class Room
    {
        public Guid Id { get; set; }

        public Guid PropertyId { get; set; }
        public Property Property { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public decimal PricePerMonth { get; set; }
        public decimal AreaInSquareMeters { get; set; } // Dodane

        public int Capacity { get; set; } = 1;
        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
