using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirSicknessBags.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name ="Confirm Password")]
        [Compare("Password", ErrorMessage = "Password Mismatch" )]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Secret Word You Were Given")]
        public string Secret { get; set; }

        [Display(Name = "Stay Logged In?")]
        public bool Persistent { get; set; }
    }
}
