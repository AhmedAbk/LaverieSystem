using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laverie.Domain.Entities
{
    public class Action
    {
        public int Id { get; set; } 
        public int CycleId { get; set; } 
        public DateTime StartTime { get; set; } 
        
    }
}
