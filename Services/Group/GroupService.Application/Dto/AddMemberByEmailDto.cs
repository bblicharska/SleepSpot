using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupService.Application.Dto
{
    public class AddMemberByEmailDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public Guid GroupId { get; set; }
        public GroupRole Role { get; set; } = GroupRole.Member;
    }
}
