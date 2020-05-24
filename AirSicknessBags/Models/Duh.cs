using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirSicknessBags.Models
{
    public class Duh
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }
        public int Version { get; set; }
    }
}
