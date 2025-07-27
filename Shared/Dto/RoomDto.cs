using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto
{
    public class RoomDto
    {
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }
        public string? DetailedDescription { get; set; }
        public decimal PricePerMonth { get; set; }
        public decimal AreaInSquareMeters { get; set; }
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PropertyImageDto> Images { get; set; } = new();
        public IEnumerable<ReviewDto>? Reviews { get; set; }
    }
}
