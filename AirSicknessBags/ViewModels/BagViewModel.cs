using AirSicknessBags.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirSicknessBags.ViewModels
{
    public class BagViewModel
    {
        public Bagsmvc Bag { get; set; }
        public List<Peoplemvc> People { get; set; }
        public List<Bagtypes> TypeOfBag { get; set; }
    }

}
