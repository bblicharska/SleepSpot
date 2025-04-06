using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyService.Domain.Models
{
    public class Property
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }

        // Właściciel nieruchomości (Host)
        public Guid OwnerId { get; set; }
        // Navigation property
        public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();

    }

    public class PropertyImage
    {
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public string ImageUrl { get; set; }

        public Property Property { get; set; }
    }
}
