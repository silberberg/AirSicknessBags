using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace AirSicknessBags.Models
{
    public partial class BagContext : DbContext
    {
        public BagContext(DbContextOptions<BagContext> options) : base(options)
        {
        }

        //private readonly IConfiguration configuration;

        //public BagContext(IConfiguration config)
        //{
        //    configuration = config;
        //}

        public virtual DbSet<Bagsmvc> Bags { get; set; }
        public virtual DbSet<Linksmvccore> Links { get; set; }
        public virtual DbSet<Peoplemvc> People { get; set; }
        //public virtual DbSet<Bagsmvc> Bagsmvc { get; set; }
        //public virtual DbSet<Linksmvccore> Linksmvccore { get; set; }
        //public virtual DbSet<Peoplemvc> Peoplemvc { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        string constr = configuration.GetConnectionString("DefaultConnection");
        //        optionsBuilder.UseMySql(constr, x => x.ServerVersion("5.7.21-mysql"));
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bagsmvc>(entity =>
            {
                entity.Property(e => e.Airline)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.BackFileName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.BackgroundColor)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.BagType)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.BottomFileName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Detail)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FrontFileName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Lost).HasDefaultValueSql("'0'");

                entity.Property(e => e.ObtainedFrom)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.TextColor)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Year)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Linksmvccore>(entity =>
            {
                entity.HasKey(e => e.LinkNumber)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.LinkNumber)
                    .HasName("LinkNumber")
                    .IsUnique();

                entity.Property(e => e.BackFileName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.BottomFileName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FirstName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FrontFileName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.MiddleName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Peoplemvc>(entity =>
            {
                entity.HasKey(e => e.PersonNumber)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.PersonNumber)
                    .HasName("PersonNumber")
                    .IsUnique();

                entity.Property(e => e.Comments)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Country)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FirstName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.MiddleName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PrimaryEmail)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PrimarySite)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PrimarySiteName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.SecondaryEmail)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.SecondarySite)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.SecondarySiteName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.TertiaryEmail)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
