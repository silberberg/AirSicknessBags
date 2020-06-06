using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

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

        public virtual DbSet<Bagsmvc> Bags { get; set; }
        public virtual DbSet<Linksmvccore> Links { get; set; }
        public virtual DbSet<Peoplemvc> People { get; set; }
        public virtual DbSet<Bagtypes> Types { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
//        public virtual DbSet<Duh> Duh { get; set; }

        public class BagViewModel
        {
            public Bagsmvc Bag { get; set; }
            public List<Peoplemvc> People { get; set; }
            public List<Bagtypes> TypeOfBag { get; set; }
        }

        public class PeopleViewModel
        {
            public List<Peoplemvc> People{ get; set; }
            public List<Country> Countries { get; set; }
        }

        public class PersonViewModel
        {
            public Peoplemvc Person { get; set; }
            public List<Country> Countries { get; set; }
        }

        public class LinkViewModel
        {
            public Bagsmvc Bag { get; set; }
            public List<Peoplemvc> People { get; set; }
            public Linksmvccore Link { get; set; }
            public List<SelectListItem> Options { get; set; }
        }

        public class AllLinkViewModel
        {
            public Bagsmvc Bag { get; set; }
            public Peoplemvc Person { get; set; }
            public Linksmvccore Link { get; set; }
            public List<Peoplemvc> People { get; set; }
            public List<Bagsmvc> Bags{ get; set; }
            public List<Linksmvccore> Links { get; set; }

            public AllLinkViewModel()
            {
                Bag = new Bagsmvc();
                Person = new Peoplemvc();
                Link = new Linksmvccore();
                People = new List<Peoplemvc>();
                Bags = new List<Bagsmvc>();
                Links = new List<Linksmvccore>();
            }
        }

        public class Patron
        {
            public Peoplemvc Person { get; set; }
            public List<Bagsmvc> Bags { get; set; }
        }

        public static string DisplayImage(string bagname, int width = 100)
        {
            if (bagname != null)
            {
                string img = "http://www.airsicknessbags.com/components/com_airsicknessbag/images/" + bagname + ".jpg";
                string result = "<a href=" + img + " target=\"_blank\" title=\"Click on image to supersize\"> ";
                result += "<span><img class=\"border border-primary border-width:8px\" src=" + img + " style=\"width: " + width + "px\" />";
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
