using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirSicknessBags.Models
{
    [Bind("BagId,PersonId,LinkNumber")]
    [Table("linksmvccore")]
    public partial class Linksmvccore
    {
        [Key]
        [Column(TypeName = "int(11)")]
        public int LinkNumber { get; set; }
        [Column("BagID", TypeName = "int(11)")]
        public int BagId { get; set; }
        [Column("PersonID", TypeName = "int(11)")]
        public int PersonId { get; set; }

        public Bagsmvc Bag { get; set; }
        public Peoplemvc Person { get; set; }
    }
}
