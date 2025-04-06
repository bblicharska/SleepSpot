using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Services
{
    public interface IPasswordHasher
    {
        string HashPassword(string password); // Tworzy hash z podanego hasła
        bool VerifyPassword(string hashedPassword, string password); // Weryfikuje, czy hasło jest zgodne z hashem
    }
}
