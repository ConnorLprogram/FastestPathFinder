using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapExample.Data
{
    public class Town
    {   
        public Guid Id { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }  
        public string Name { get; set; }
    }
}
