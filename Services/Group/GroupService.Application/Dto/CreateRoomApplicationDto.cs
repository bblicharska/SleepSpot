using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupService.Application.Dto
{
    public class CreateRoomApplicationDto
    {
        public Guid ListingId { get; set; }
        public Guid ApplicantUserId { get; set; }
        public string Message { get; set; }
    }

}
