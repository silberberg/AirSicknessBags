﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirSicknessBags.ViewModels
{
    public class BagsImageViewModel
    {
        public List<AirSicknessBags.Models.Bagsmvc> Bags { get; set; }
        public List<AirSicknessBags.Models.Bagtypes> Bagtypes { get; set; }
        public IFormFile MyImage { set; get; }
    }
}
