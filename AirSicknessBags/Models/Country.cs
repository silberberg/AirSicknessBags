using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirSicknessBags.Models
{
    [Table("country")]
    public partial class Country
    {
        [Key]
        [Column("iso", TypeName = "char(2)")]
        public string Iso { get; set; }
        [Required]
        [Column("name", TypeName = "varchar(80)")]
        public string Name { get; set; }
        [Required]
        [Column("printable_name", TypeName = "varchar(80)")]
        public string PrintableName { get; set; }
        [Column("iso3", TypeName = "char(3)")]
        public string Iso3 { get; set; }
        [Column("numcode", TypeName = "smallint(6)")]
        public short? Numcode { get; set; }
    }
}
