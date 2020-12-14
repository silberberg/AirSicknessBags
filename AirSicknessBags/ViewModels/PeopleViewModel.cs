using AirSicknessBags.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirSicknessBags.ViewModels
{
    public class PeopleViewModel
    {
        public List<Peoplemvc> People { get; set; }
        public List<Country> Countries { get; set; }
    }

}
