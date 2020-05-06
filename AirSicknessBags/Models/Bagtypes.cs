using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirSicknessBags.Models
{
    [Table("bagtypes")]
    public partial class Bagtypes
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Required]
        [Column("letter", TypeName = "varchar(2)")]
        public string Letter { get; set; }
        [Required]
        [Column("bagtype", TypeName = "varchar(45)")]
        public string Bagtype { get; set; }
    }
}
