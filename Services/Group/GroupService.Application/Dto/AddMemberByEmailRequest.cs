using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupService.Application.Dto
{
    public class AddMemberByEmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public GroupRole Role { get; set; } = GroupRole.Member;
        public Guid? GroupId { get; set; }
    }
}
