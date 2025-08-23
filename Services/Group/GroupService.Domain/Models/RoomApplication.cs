using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupService.Domain.Models
{
    public class RoomApplication
    {
        public Guid Id { get; set; }
        public Guid ListingId { get; set; }
        public GroupListing Listing { get; set; }
        public Guid ApplicantUserId { get; set; }
        public string Message { get; set; }
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

public enum ApplicationStatus
{
    Pending,
    Accepted,
    Rejected
}