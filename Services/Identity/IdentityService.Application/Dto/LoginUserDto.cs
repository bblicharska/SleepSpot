using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Dto
{
    public class LoginUserDto
    {
        public string UsernameOrEmail { get; set; } // Możliwość logowania przez nazwę użytkownika lub email
        public string Password { get; set; }
    }
}
