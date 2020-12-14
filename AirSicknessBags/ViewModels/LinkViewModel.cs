using AirSicknessBags.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirSicknessBags.ViewModels
{
    public class LinkViewModel
    {
        public Bagsmvc Bag { get; set; }
        public List<Peoplemvc> People { get; set; }
        public Linksmvccore Link { get; set; }
        public List<SelectListItem> Options { get; set; }
    }


}
