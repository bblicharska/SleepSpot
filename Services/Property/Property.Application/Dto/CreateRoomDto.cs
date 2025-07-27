using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyService.Application.Dto
{
    public class CreateRoomDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string? DetailedDescription { get; set; }

        public decimal PricePerMonth { get; set; }
        public decimal AreaInSquareMeters { get; set; }

        public int Capacity { get; set; }
        public bool IsAvailable { get; set; } = true;
        public List<UploadPropertyImageDto> Images { get; set; } = new();

    }
}
