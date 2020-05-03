using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirSicknessBags.Models
{

    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public virtual DbSet<Bagsmvc> Bagsmvc { get; set; }
        public virtual DbSet<Linksmvccore> Linksmvccore { get; set; }
        public virtual DbSet<Peoplemvc> Peoplemvc { get; set; }

    }
}
