using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto
{
    public class PropertyDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string? DetailedDescription { get; set; }
        public bool isAvailable { get; set; } = true;
        public string Address { get; set; }

        public decimal PricePerMonth { get; set; }
        public decimal AreaInSquareMeters { get; set; }

        public bool IsEntirePlaceRentable { get; set; }
        public DateTime CreatedAt { get; set; } 

        public List<PropertyImageDto> Images { get; set; } = new();
        public List<RoomDto> Rooms { get; set; } = new();

        public Guid OwnerId { get; set; }
        public IEnumerable<ReviewDto>? Reviews { get; set; }
        public UserDto Owner { get; set; } = new UserDto();
    }
}
