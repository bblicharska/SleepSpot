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
        public string? DetailedDescription { get; set; }
        public string Address { get; set; }
        public bool isAvailable { get; set; } = true;
        public decimal PricePerMonth { get; set; }
        public bool IsEntirePlaceRentable { get; set; } = true;
        public decimal AreaInSquareMeters { get; set; }
        public DateTime AvailableSince { get; set; } = DateTime.UtcNow;
        public Guid OwnerId { get; set; }
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
