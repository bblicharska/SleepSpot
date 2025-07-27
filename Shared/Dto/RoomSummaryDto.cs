using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto
{
    public class RoomSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal PricePerMonth { get; set; }
        public decimal AreaInSquareMeters { get; set; }
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }
        public string? MainImage { get; set; }
    }
}
