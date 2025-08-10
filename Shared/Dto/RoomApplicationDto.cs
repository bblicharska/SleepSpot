using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto
{
    public class RoomApplicationDto
    {
        public Guid Id { get; set; }
        public Guid ListingId { get; set; }
        public Guid ApplicantUserId { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public UserDto? Applicant { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
