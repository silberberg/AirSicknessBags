using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AirSicknessBags.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static AirSicknessBags.Models.BagContext;

namespace AirSicknessBags.Controllers
{
    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }
    }

    public class BagsController : Controller
    {
        private readonly BagContext _db;
        private IMemoryCache _cache;
        [BindProperty]
        public BagViewModel bvm { get; set; }
        public Bagsmvc Bags { get; set; }
        public Bagtypes TypesARooney { get; set; }

        public BagsController(BagContext db)
        {
            _db = db;
//            _cache = memoryCache;
        }

        //public static class CacheKeys
        //{
        //    public static List<Bagtypes> bagtypes { get { return _db.Types; } }
        //}

        // List of bagtypes.  Only want to get these once
        public static List<Bagtypes> bagtypes  {get; set;} = null;

        //        private static async DbSet<Bagtypes> GetBagtypes()
//        public static async Task<IActionResult> GetBagtypes(BagContext _db)
        public DbSet<Bagtypes> GetBagtypes()
        {
            var bagtypes = _db.Types;
            return (bagtypes);
        }

        #region API Calls
        // GET: Bags
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        //        public async Task<IActionResult> GetBags(string? SearchString)
        public async Task<IActionResult> GetBags(IFormCollection? collection)
//        public async Task<IActionResult> GetBags(HttpRequest collection)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        {
            OkObjectResult Response;
            List<Bagsmvc> SortedData;
            int chosenYear;

            if (collection != null)
            {
                string SearchString = collection["Airline"].ToString();
                string DetailString = collection["Detail"].ToString();
                string BagTypeString = collection["BagType"].ToString();
                string ChosenYear = collection["Year"].ToString();
                string Compare = collection["DateCompare"].ToString();
                if (Compare == "")
                {
                    Compare = "After";
                    chosenYear = 0;
                } else
                {
                    chosenYear = ChosenYear == "Any" ? 0 : Convert.ToInt32(ChosenYear);
                }

                int swapcount = collection["Swaps"].ToString() == "on" ? 1 : 0;
                // Below, Convert.ToInt32 will return 0 when a.Year is null
                List<Bagsmvc> ResponseData = await _db.Bags.Where(x => x.Airline.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
                    .Where(y => y.Detail.Contains(DetailString))
                    .Where(z => z.BagType.Contains(BagTypeString))
                    .Where(w => w.NumberOfSwaps >= swapcount)
                    .Where(a => chosenYear == 0 ? true :
                        Compare == "After" ? Convert.ToInt32(a.Year) > chosenYear :
                            (Compare == "Before" ? Convert.ToInt32(a.Year) < chosenYear && Convert.ToInt32(a.Year) != 0 :
                                Convert.ToInt32(a.Year) == chosenYear))
                    .ToListAsync();

                if (swapcount > 0)
                {
                    SortedData = ResponseData.OrderByDescending(x => x.DateSwapAdded).ToList();
                } else
                {
                    SortedData = ResponseData.OrderBy(x => x.Airline).ToList();
                }

                Response = Ok(SortedData) as OkObjectResult;
            } else
            {
                Response = null as OkObjectResult;
            }
            return Response;
        }
        #endregion

        // GET: Bags
        [HttpGet]
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public async Task<IActionResult> Index(IFormCollection collection)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        {
            IActionResult bags;
//            var stupid = HttpUtility.ParseQueryString(Request.QueryString);

            if (collection.Count == 0)
            {
                bags = await GetBags(null);
            } else
            {
                bags = await GetBags(collection);
            }

            List<Bagsmvc> baglist = new List<Bagsmvc>();

            if (bags is null)
            {
                baglist = null;
            } else
            {
                var okResult = bags as OkObjectResult;
                baglist = okResult.Value as List<Bagsmvc>;
            }

            return View(baglist);
        }

        // Post: Bags
        [HttpPost]
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public async Task<IActionResult> Index()
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        {
            string airline = Request.Form["Airline"].ToString();
            string swaps = Request.Form["Swaps"];
            // THIS WILL WORK TOO!
            //string airline2 = collection["Airline"].ToString();
//            IActionResult bags = await GetBags(collection);
            IActionResult bags = await GetBags(Request.Form);

            var okResult = bags as OkObjectResult;
            var baglist = okResult.Value as List<Bagsmvc>;

            return View(baglist);
        }

        // GET: Bags/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Bags/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Bags/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public static Bagsmvc Clone<Bagsmvc>(Bagsmvc source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<Bagsmvc>(serialized);
        }


        // GET: Bags/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, bool? copy)
        {
            Bagsmvc bag = new Bagsmvc();
            BagViewModel bvm = new BagViewModel();
            ViewBag.Operation = "Edit This";

            // First set up the bagtype dropdown

            // When id is not null, we are editing or copying.  Otherwise, we're creating 
            // EDIT a bag
            if(id != null)
            {
                bag = await _db.Bags.FirstOrDefaultAsync(x => x.Id == id);
            } else
            // CREATE a new blank bag
            {
                ViewBag.Operation = "Newly Created";
                await _db.Bags.AddAsync(bag);
                await _db.SaveChangesAsync(); ;
            }

            // COPY a bag
            // This is a real hack.  Once the button is hit, the bag is copied and saved to the db.  Setting Id = 0 somehow
            // indicates to the Entity Framework to add the record in with a newly generated ID.  
            // Also, since we don't want an exact copy, bring user to edit screen to make the changes.
            if (copy != null)
            {
                ViewBag.Operation = "Recently Copied";
                bag.Id = 0;
                await _db.Bags.AddAsync(bag);
                await _db.SaveChangesAsync(); ;
            }

            bvm.Bag = bag;
            //            bvm.TypeOfBag = _db.Types;
            if (bagtypes == null)
            {
                bagtypes = await _db.Types.ToListAsync();
            }
            bvm.TypeOfBag = bagtypes;


            return View(bvm);
        }

        // POST: Bags/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BagViewModel bvm, IFormCollection collection)
//        public ActionResult Edit(Bagsmvc bag, IFormCollection collection)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Bags.Update(bvm.Bag);
                    _db.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }

            return View(bvm.Bag);
        }

        // GET: Bags/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Bags/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([Bind("Id")] int id, IFormCollection collection)
        {
            try
            {
                var bag = await _db.Bags.FirstOrDefaultAsync(x => x.Id == id);
                _db.Bags.Remove(bag);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Bags/Swaps5
        public ActionResult Swaps()
        {
            var list = new List<KeyValuePair<string, string>>();
            list.Add(new KeyValuePair<string, string>("Swaps", "on"));
            //List<string> DumbList = new List<string> { Swaps = "on" };
            //return Index(DumbList);
            //var duh = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>{Swaps = "on"});
            //var duh2 = typeof(duh);
            return RedirectToAction("Index", new { Swaps = "on" });
        }


    }
}