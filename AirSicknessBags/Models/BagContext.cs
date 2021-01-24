using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Xceed.Wpf.Toolkit;

namespace AirSicknessBags.Models
{
    public partial class BagContext : IdentityDbContext
    {
        public BagContext(DbContextOptions<BagContext> options) : base(options)
        {
        }

        //private readonly IConfiguration configuration;

        //public BagContext(IConfiguration config)
        //{
        //    configuration = config;
        //}

        // These statements allow the code to communicate with the database 
        public virtual DbSet<Bagsmvc> Bags { get; set; }
        public virtual DbSet<Linksmvccore> Links { get; set; }
        public virtual DbSet<Peoplemvc> People { get; set; }
        public virtual DbSet<Bagtypes> Types { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
//        public virtual DbSet<Duh> Duh { get; set; }

        public static string DisplayImage(string bagname, int width = 100)
        {
            if (bagname != null)
            {
                string img;
                string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (env == "Production")
                {
                    img = "http://www.airsicknessbags.com/components/com_airsicknessbag/images/" + bagname + ".jpg";
                } else
                {
                    img = "/images/" + bagname + ".jpg";
                }
                string result = "<a href=" + img + " target=\"_blank\" title=\"Click on image to supersize\"> ";
                result += "<span><img asp-append-version=\"true\" class=\"border border-primary border-width:8px\" src=" + img + " style=\"max-width: " + width + "px\" />";
                result += "</a></span>";
                return (result);
            }
            else
            {
                return ("");
            }
        }

        static public List<T> CreatePagination<T>(List<T> items, int whichpage, int perpage, ref int numpages)
        {
            if (items != null)
            {
                double stupid = Convert.ToDouble(items.Count) / perpage;
                numpages = Convert.ToInt32(Math.Ceiling(stupid));
                return (items.GetRange((whichpage - 1) * perpage, Math.Min(perpage, items.Count - ((whichpage - 1) * perpage))));
            }
            else
            {
                return (null);
            }
        }

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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Bagsmvc>(entity =>
            {
                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                //entity.HasIndex(e => e.ObtainedFromPerson)
                //    .HasName("FK_ObtainedFrom_idx");

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

                //entity.Property(e => e.ObtainedFromPerson)
                //    .HasCharSet("utf8")
                //    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PersonID)
                    .HasColumnName("PersonID");

                entity.Property(e => e.TextColor)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Year)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

//                entity.HasForeignKey(a => a.ObtainedFromPerson);
//                entity.Property(e => e.Links).HasColumnName("BagID");
            });

            modelBuilder.Entity<Linksmvccore>(entity =>
            {
                entity.HasKey(e => e.LinkNumber)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.LinkNumber)
                    .HasName("LinkNumber")
                    .IsUnique();

                //entity.Property(e => e.BackFileName)
                //    .HasCharSet("utf8")
                //    .HasCollation("utf8_general_ci");

                //entity.Property(e => e.BottomFileName)
                //    .HasCharSet("utf8")
                //    .HasCollation("utf8_general_ci");

                //entity.Property(e => e.FrontFileName)
                //    .HasCharSet("utf8")
                //    .HasCollation("utf8_general_ci");

                //entity.Property(e => e.FirstName)
                //    .HasCharSet("utf8")
                //    .HasCollation("utf8_general_ci");

                //entity.Property(e => e.LastName)
                //    .HasCharSet("utf8")
                //    .HasCollation("utf8_general_ci");

                //entity.Property(e => e.MiddleName)
                //    .HasCharSet("utf8")
                //    .HasCollation("utf8_general_ci");
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

                //entity.Property(e => e.Country)
                //    .HasCharSet("utf8")
                //    .HasCollation("utf8_general_ci");

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
