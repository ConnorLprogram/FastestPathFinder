using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PathFinder.Models
{
    public class Location
    {
        public Guid Id { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string name { get; set; }
        public bool possibleLocation { get; set; }
        public bool closePlace { get; set; }
        public double distanceFromLocation { get; set; }
        
    }
    public class Pathlist
    {
        public Guid Id { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string name { get; set; }
    }
}
