using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Dto
{
    public class TokenDto
    {
        public string AccessToken { get; set; } 
        public DateTime ExpiresAt { get; set; } 
    }
}
