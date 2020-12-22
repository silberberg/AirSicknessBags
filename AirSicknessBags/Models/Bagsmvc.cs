using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Net;

namespace AirSicknessBags.Models
{
    //public class BagList
    //{
    //    public List<Bagsmvc> baglist { get; set; }
    //}

    [Table("bags")]
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
        [Display(Name = "Bag Type")]
        public string BagType { get; set; }
        //[Column(TypeName = "varchar(255)")]
        //public string ObtainedFrom { get; set; }
        //[ForeignKey("PersonNumber")]
        //[Column(TypeName = "int(11)")]
        //public int? ObtainedFromPerson { get; set; }
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

        public int? PersonID { get; set; } // Only 1 person obtained from
        public Peoplemvc Person { get; set; } // This is the person bag was obtained from 

        public ICollection<Linksmvccore> Links { get; set; }

        //public String CopyFile(String filename) 
        //{
        //    if (filename != null)
        //    {
        //        //string sourcePath = "C:/airsick/100 dpi with no matching 300 dpi scans/" +
        //        //    filename.Substring(0, 1) + "/";
        //        string sourcePath = @"C:\airsick\100 dpi with no matching 300 dpi scans\" +
        //            filename.Substring(0, 1) + @"\";
        //        string targetPath = @"wwwroot\images\";
        //        // targetPath = @"C:\doc\";



        //        //// Get the object used to communicate with the server.
        //        FtpWebRequest request = (FtpWebRequest)WebRequest
        //            .Create("ftp://192.185.11.66/mvc.fitpacking.com/wwwroot/images/" + filename + ".jpg");
        //        request.Method = WebRequestMethods.Ftp.UploadFile;
        //        request.UsePassive = false;
        //        request.UseBinary = true;

        //        //// FTP login
        //        string pass = Environment.GetEnvironmentVariable("FTP_ACCESS");
        //        request.Credentials = new NetworkCredential("fitpacking", pass);

        //        // Copy the contents of the file to the request stream.  TEXT FILES ONLY
        //        //StreamReader sourceStream = new StreamReader(sourcePath + filename + ".jpg");
        //        //byte[] fileContents = System.Text.Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
        //        //                request.ContentLength = sourceStream.ReadToEnd().Length;
        //        //                sourceStream.Close();

        //        // Copy the contents of the file.  IMAGE FILES ONLY
        //        String tempFilename = sourcePath + filename + ".jpg";
        //        return (tempFilename);
        //        //byte[] fileContents = File.ReadAllBytes(tempFilename);
        //        //request.ContentLength = fileContents.Length;

        //        //Stream requestStream = request.GetRequestStream();
        //        //requestStream.Write(fileContents, 0, fileContents.Length);
        //        //requestStream.Close();

        //        //FtpWebResponse response = (FtpWebResponse)request.GetResponse();

        //        //response.Close();





        //        // Use Path class to manipulate file and directory paths.
        //        //string sourceFile = Path.Combine(sourcePath, filename) + ".jpg";
        //        //string destFile = Path.Combine(targetPath, filename) + ".jpg";

        //        //if (!File.Exists(destFile) &&
        //        //    File.Exists(sourceFile))
        //        //{
        //        //    File.Copy(sourceFile, destFile);
        //        //}


        //    }

        //    return ("No image found");

        //}
    }

}
