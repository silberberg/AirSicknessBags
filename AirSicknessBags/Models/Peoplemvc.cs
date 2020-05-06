using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirSicknessBags.Models
{
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
        public string LastName { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string Country { get; set; }
        [Column(TypeName = "char(2)")]
        public string IsoCountry { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string PrimaryEmail { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string SecondaryEmail { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string TertiaryEmail { get; set; }
        [Column(TypeName = "int(11)")]
        public int? Collector { get; set; }
        [Column(TypeName = "int(11)")]
        public int? Donor { get; set; }
        [Column(TypeName = "int(11)")]
        public int? Swapper { get; set; }
        [Column(TypeName = "int(11)")]
        public int? Seller { get; set; }
        [Column(TypeName = "int(11)")]
        public int? StarterKit { get; set; }
        [Column(TypeName = "longtext")]
        public string Comments { get; set; }
    }
}
