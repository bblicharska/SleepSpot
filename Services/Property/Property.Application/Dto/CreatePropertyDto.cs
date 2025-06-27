using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyService.Application.Dto
{
    public class CreatePropertyDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }

        public decimal PricePerMonth { get; set; }
        public decimal AreaInSquareMeters { get; set; }

        public bool IsEntirePlaceRentable { get; set; } = true;

        public List<PropertyImageDto> Images { get; set; } = new();
        public List<CreateRoomDto> Rooms { get; set; } = new();

        public Guid OwnerId { get; set; }
    }
}
