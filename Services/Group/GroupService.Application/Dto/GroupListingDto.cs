using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupService.Application.Dto
{
    public class GroupListingDto
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public int DesiredRoommatesCount { get; set; }
        public string Status { get; set; } // lub ListingStatus enum
        public Guid? PropertyId { get; set; }
        public string PreferredCity { get; set; }
        public decimal? MaxBudgetPerPerson { get; set; }

        public DateTime CreatedAt { get; set; }

        public PropertyDto? Property { get; set; }  // <- tutaj pełne dane nieruchomości

        public List<RoomApplicationDto> Applications { get; set; } = new();
    }

}
