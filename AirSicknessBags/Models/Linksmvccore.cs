using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirSicknessBags.Models
{
    [Table("linksmvccore")]
    public partial class Linksmvccore
    {
        [Key]
        [Column(TypeName = "int(11)")]
        public int LinkNumber { get; set; }
        [Column("BagID", TypeName = "int(11)")]
        public int? BagId { get; set; }
        [Column("PersonID", TypeName = "int(11)")]
        public int? PersonId { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string FrontFileName { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string BackFileName { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string BottomFileName { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string FirstName { get; set; }
        [Column(TypeName = "varchar(45)")]
        public string MiddleName { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string LastName { get; set; }
    }
}
