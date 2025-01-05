using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laverie.Domain.DTOS
{
    public class LaundryUpdateDTO
    {
        public string nomLaverie { get; set; }
        public string address { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }

        public List<string> Services { get; set; } = new List<string>();
    }
}
