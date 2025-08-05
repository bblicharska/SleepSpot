using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto
{
    public class GroupListingDto
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DesiredRoommatesCount { get; set; }
        public string Status { get; set; } 
        public Guid? PropertyId { get; set; }
        public Guid? RoomId { get; set; }
        public string PreferredCity { get; set; }
        public bool PropertyAlreadyRented { get; set; } = false; 
        public decimal? MaxBudgetPerPerson { get; set; }
        public DateTime CreatedAt { get; set; }
        public GroupDto Group { get; set; } 
        public PropertyDto? Property { get; set; } 
        public RoomWithPropertyDetailsDto? Room { get; set; }
        public List<RoomApplicationDto> Applications { get; set; } = new();
    }

}
