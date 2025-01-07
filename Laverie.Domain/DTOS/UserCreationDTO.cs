using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laverie.Domain.DTOS
{
    public class UserCreationDTO
    {
       
        public string Name { get; set; }

        
        public string Password { get; set; }


        public string Email { get; set; }

     
        public int Age { get; set; }
    }
}

