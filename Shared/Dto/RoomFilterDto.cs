using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto
{
    public class RoomFilterDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal PricePerMonth { get; set; }
        public decimal AreaInSquareMeters { get; set; }
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime AvailableSince { get; set; } = DateTime.UtcNow;
        public List<PropertyImageDto> Images { get; set; } = new();

        public Guid PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyAddress { get; set; }
        public Guid PropertyOwnerId { get; set; }
    }
}
