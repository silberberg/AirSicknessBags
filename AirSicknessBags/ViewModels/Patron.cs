using AirSicknessBags.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirSicknessBags.ViewModels
{
    public class Patron
    {
        public Peoplemvc Person { get; set; }
        public List<Bagsmvc> Bags { get; set; }
    }


}
