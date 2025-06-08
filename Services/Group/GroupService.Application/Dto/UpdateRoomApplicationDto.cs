using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupService.Application.Dto
{
    public class UpdateRoomApplicationDto
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public ApplicationStatus Status { get; set; }
    }
}
