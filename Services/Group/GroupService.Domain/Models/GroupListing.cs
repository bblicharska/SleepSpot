using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupService.Domain.Models
{
    public class GroupListing
    {
        public Guid Id { get; set; }

        public Guid GroupId { get; set; }
        public Group Group { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public int DesiredRoommatesCount { get; set; } = 1;
        public ListingStatus Status { get; set; } = ListingStatus.Active; 

        public Guid? PropertyId { get; set; }
        public Guid? RoomId { get; set; } 

        public bool PropertyAlreadyRented { get; set; } = false;

        public string PreferredCity { get; set; }
        public decimal? MaxBudgetPerPerson { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<RoomApplication> Applications { get; set; } = new List<RoomApplication>();
    }
}

public enum ListingStatus
{
    Active,
    Closed
}