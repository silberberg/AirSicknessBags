using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirSicknessBags.Models
{
    [Bind("PersonNumber,PrimarySiteName,SecondarySiteName,PrimarySite,SecondarySite,FirstName,MiddleName,LastName,IsoCountry,PrimaryEmail,SecondaryEmail,TertiaryEmail,Collector,Donor,Swapper,Seller,StarterKit,Comments")]
    [Table("peoplemvc")]
    public partial class Peoplemvc
    {
        [Key]
        [Column(TypeName = "int(11)")]
        public int PersonNumber { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string PrimarySiteName { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string SecondarySiteName { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string PrimarySite { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string SecondarySite { get; set; }
        [Column(TypeName = "varchar(255)")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string MiddleName { get; set; }
        [Column(TypeName = "varchar(255)")]
        [Display(Name = "Name")]
        public string LastName { get; set; }
        //[Column(TypeName = "varchar(255)")]
        //public string Country { get; set; }
        [Display(Name = "Country")]
        [Column(TypeName = "char(2)")]
        public string IsoCountry { get; set; }
        [Column(TypeName = "varchar(255)")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string PrimaryEmail { get; set; }
        [Column(TypeName = "varchar(255)")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string SecondaryEmail { get; set; }
        [Column(TypeName = "varchar(255)")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string TertiaryEmail { get; set; }
        [Column(TypeName = "tinyint(4)")]
        public int? Collector { get; set; }
        [Column(TypeName = "tinyint(4)")]
        public int? Donor { get; set; }
        [Column(TypeName = "tinyint(4)")]
        public int? Swapper { get; set; }
        [Column(TypeName = "tinyint(4)")]
        public int? Seller { get; set; }
        [Column(TypeName = "tinyint(4)")]
        [Display(Name = "Starter Kit")]
        public int? StarterKit { get; set; }
        [Column(TypeName = "longtext")]
        public string Comments { get; set; }

        public ICollection<Linksmvccore> Links { get; set; } // Each person may be associated with lots of links
        // Each person may be associated with lots of bags, but I don't really care about this because it's a collection
        // of bags that sent me this bag *first*.  The Links table has all bags sent to me by this person, so this
        // line is probably unnecessary.
        public ICollection<Bagsmvc> Bags { get; set; } 
        // public Bagsmvc Bag { get; set; } // Each person may be associated with lots of bags
    }
}
