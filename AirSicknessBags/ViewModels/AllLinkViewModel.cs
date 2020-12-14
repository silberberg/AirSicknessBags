using AirSicknessBags.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirSicknessBags.ViewModels
{
    public class AllLinkViewModel
    {
        public Bagsmvc Bag { get; set; }
        public Peoplemvc Person { get; set; }
        public Linksmvccore Link { get; set; }
        public List<Peoplemvc> People { get; set; }
        public List<Bagsmvc> Bags { get; set; }
        public List<Linksmvccore> Links { get; set; }

        public AllLinkViewModel()
        {
            Bag = new Bagsmvc();
            Person = new Peoplemvc();
            Link = new Linksmvccore();
            People = new List<Peoplemvc>();
            Bags = new List<Bagsmvc>();
            Links = new List<Linksmvccore>();
        }
    }

}
