using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirSicknessBags.Models
{
    //public class BagList
    //{
    //    public List<Bagsmvc> baglist { get; set; }
    //}

    [Table("bagsmvc")]
    public partial class Bagsmvc
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column(TypeName = "varchar(255)")]
        [Required]
        public string Airline { get; set; } = "No Airline";
        [Column(TypeName = "varchar(255)")]
        public string TextColor { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string BackgroundColor { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string Year { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string FrontFileName { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string BackFileName { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string BottomFileName { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string BagType { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string ObtainedFrom { get; set; }
        [Column(TypeName = "int(11)")]
        public int? NumberOfSwaps { get; set; } = 0;
        [Column(TypeName = "int(11)")]
        public int? Lost { get; set; }
        // WORKS        [Column(TypeName = "datetime")]
        // WORKS        public DateTime? DateSwapAdded { get; set; } = DateTime.Today;
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]  // Format doesn't seem to work
        public DateTime? DateSwapAdded { get; set; } = DateTime.Today;
        [Column(TypeName = "longtext")]
        [Display(Name = "Pithy Description")]
        public string Detail { get; set; }
    }
}
