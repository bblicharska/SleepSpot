using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto
{
    public class RoomWithPropertyDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? DetailedDescription { get; set; }
        public decimal PricePerMonth { get; set; }
        public decimal AreaInSquareMeters { get; set; }
        public DateTime AvailableSince { get; set; } = DateTime.UtcNow;
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }
        public List<PropertyImageDto> Images { get; set; } = new List<PropertyImageDto>();
        public Guid PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyAddress { get; set; }
        public Guid? OwnerId { get; set; } 

        public List<RoomSummaryDto> OtherRoomsInProperty { get; set; } = new List<RoomSummaryDto>();
        public OwnerInfoDto Owner { get; set; } 
        public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
    }

}
