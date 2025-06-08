using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupService.Application.Dto
{
    public class GroupMemberDto
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
        public string Role { get; set; } // lub GroupRole enum, jeśli chcesz używać typowanego enuma
        public DateTime JoinedAt { get; set; }

        public UserDto? User { get; set; }  // <- pełne info o użytkowniku


    }

}
