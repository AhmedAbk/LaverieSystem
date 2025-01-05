using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laverie.Domain.DTOS
{
    public class MachineCreationDTO
    {
        public bool status { get; set; }
        public string type { get; set; }
        public string LaverieId { get; set; }
    }
}
