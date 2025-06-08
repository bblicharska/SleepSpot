using System;
using System.Collections.Generic;
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
        public string Address { get; set; }

        public decimal PricePerMonth { get; set; }
        public decimal AreaInSquareMeters { get; set; }

        public bool IsEntirePlaceRentable { get; set; }

        public List<string> Images { get; set; } = new(); // or use List<PropertyImageDto>
        public List<RoomDto> Rooms { get; set; } = new();

        public Guid OwnerId { get; set; }
    }
}
