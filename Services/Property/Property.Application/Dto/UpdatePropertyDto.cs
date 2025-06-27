using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyService.Application.Dto
{
    public class UpdatePropertyDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }

        public decimal PricePerMonth { get; set; }
        public decimal AreaInSquareMeters { get; set; }

        public bool IsEntirePlaceRentable { get; set; }

        public List<ImageUpdateDto> Images { get; set; } = new();
        public List<UpdateRoomDto> Rooms { get; set; } = new();
    }
}
