using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laverie.Domain.DTOS
{
    public class UserLoginDTO
    {
        // User's email address
        public string Email { get; set; }

        // User's password
        public string Password { get; set; }
    }

}