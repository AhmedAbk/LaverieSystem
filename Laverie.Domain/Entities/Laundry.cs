using System;
using System.Collections.Generic;

namespace Laverie.Domain.Entities
{
    public class Laundry
    {
        public int id { get; set; }
        public string nomLaverie { get; set; }
        public List<Machine> machines { get; set; } = new List<Machine>(); 
        public string address { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public List<string> Services { get; set; } = new List<string>();   
    }
}
