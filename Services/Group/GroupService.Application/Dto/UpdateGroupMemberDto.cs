using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupService.Application.Dto
{
    public class UpdateGroupMemberDto
    {
        public Guid Id { get; set; }
        public GroupRole Role { get; set; }
    }

}
