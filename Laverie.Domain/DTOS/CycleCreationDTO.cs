using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laverie.Domain.DTOS
{
    public class CycleCreationDTO
    {
        public decimal price { get; set; }
        public int machineId { get; set; }
        public string cycleDuration { get; set; }
    }
}
