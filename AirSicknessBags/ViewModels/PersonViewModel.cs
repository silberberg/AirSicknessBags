using AirSicknessBags.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirSicknessBags.ViewModels
{
    public class PersonViewModel
    {
        public Peoplemvc Person { get; set; }
        public List<Country> Countries { get; set; }
    }


}
