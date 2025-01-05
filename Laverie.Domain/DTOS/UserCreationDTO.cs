using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laverie.Domain.DTOS
{
    public class UserCreationDTO
    {
        // User's full name
        public string Name { get; set; }

        // User's password (should be hashed before storing)
        public string Password { get; set; }

        // User's email address
        public string Email { get; set; }

        // User's age
        public int Age { get; set; }
    }
}

