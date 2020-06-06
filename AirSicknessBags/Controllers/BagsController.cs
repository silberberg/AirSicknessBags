﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AirSicknessBags.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<BagsController> _logger;
        private IGenericCacheService _cache;
        private readonly BagContext _context;
        [BindProperty]
        public BagViewModel bvm { get; set; }
        public Bagsmvc Bags { get; set; }
        public Bagtypes TypesARooney { get; set; }
        public static IFormCollection SortedCriteria;

        public BagsController(ILogger<BagsController> logger, IGenericCacheService cache, BagContext context)
        {
            _logger = logger;
            _cache = cache;
            _context = context;
        }

        public async Task<List<Bagtypes>> GetBagtypes()
        {
            var bagtypes = await _cache.GetFromTable(_context.Types);
            return bagtypes;
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
                // Get all the bags from the cache
                var allbags = await _cache.GetFromTable(_context.Bags);
                if (collection["SeeEveryBag"].ToString() == "on")
                {
                    Response = Ok(allbags) as OkObjectResult; 
                } else
                {
                    string SearchString = collection["Airline"].ToString();
                    string DetailString = collection["Detail"].ToString();
                    string ColorString = collection["TextColor"].ToString();
                    string BackgroundString = collection["BackgroundColor"].ToString();
                    string BagTypeString = collection["BagType"].ToString();
                    string ChosenYear = collection["Year"].ToString();
                    string Compare = collection["DateCompare"].ToString();

                    if (Compare == "")
                    {
                        Compare = "After";
                        chosenYear = 0;
                    }
                    else
                    {
                        chosenYear = ChosenYear == "Any" ? 0 : ReturnInt(ChosenYear);
                        chosenYear = ChosenYear == "Any" ? 0 : ReturnInt(ChosenYear);
                    }

                    int swapcount = collection["Swaps"].ToString() == "on" ? 1 : 0;
                    // Below, Convert.ToInt32 will return 0 when a.Year is null
                    List<Bagsmvc> ResponseData = allbags.Where(x => x.Airline.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
                        .Where(y => y.Detail != null && y.Detail.Contains(DetailString, StringComparison.OrdinalIgnoreCase))
                        .Where(z => z.BagType != null && z.BagType.Contains(BagTypeString, StringComparison.OrdinalIgnoreCase))
                        .Where(b => b.TextColor != null && b.TextColor.Contains(ColorString, StringComparison.OrdinalIgnoreCase))
                        .Where(c => c.BackgroundColor != null && c.BackgroundColor.Contains(BackgroundString, StringComparison.OrdinalIgnoreCase))
                        .Where(w => w.NumberOfSwaps >= swapcount)
                        .Where(a => chosenYear == 0 ? true :
                            Compare == "After" ? ReturnInt(a.Year) > chosenYear :
                                (Compare == "Before" ? ReturnInt(a.Year) < chosenYear && ReturnInt(a.Year) != 0 :
                                    ReturnInt(a.Year) == chosenYear))
                        .ToList();

                    if (swapcount > 0)
                    {
                        SortedData = ResponseData.OrderByDescending(x => x.DateSwapAdded).ToList();
                    }
                    else
                    {
                        SortedData = ResponseData.OrderBy(x => x.Airline).ToList();
                    }

                    Response = Ok(SortedData) as OkObjectResult;

                }

            } else
            {
                Response = null as OkObjectResult;
            }
            return Response;
        }

        private int ReturnInt(string year)
        {
            if (year == "")
            {
                return 0;
            } else
            {
                return (Convert.ToInt32(year));
            }
        }
        #endregion

        // GET: Bags
        [HttpGet]
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public async Task<IActionResult> Index(int? id, string sortorder, int? whichpage, int? perpage, int? numpages, IFormCollection collection)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        {
            List<Bagsmvc> baglist = new List<Bagsmvc>();
            IActionResult bags;

            ViewBag.Swaps = 0;

            // If there's pagination or sorting, we have to get data using the saved state
            if (whichpage != null || sortorder != null)
            {
                collection = SortedCriteria;
            }

            // Default to sorting by airline
            string SortOrder = sortorder ?? "airline";
//            sortorder = sortorder != null ? sortorder : "airline";

            int WhichPage = whichpage ?? 1;
            int NumPages = numpages ?? 0;
            int PerPage = perpage ?? 5;

            if (collection.Count == 0 || id != null)
            {
                // Returns no bags
                bags = await GetBags(null);
            } else
            {
                // Returns bags selected on the form
                bags = await GetBags(collection);
            }

            if (bags is null)
            {
                if (id == null)
                {
                    baglist = null;
                }
                else
                {
                    // Get this bag from the cache and add it to the list
                    var allbags = await _cache.GetFromTable(_context.Bags);
                    var b = allbags.FirstOrDefault(x => x.Id == id);
                    baglist.Add(b);
                }
            } else
            {
                var okResult = bags as OkObjectResult;
                baglist = okResult.Value as List<Bagsmvc>;
            }

            // Sorting columns
            if (baglist != null)
            {
                if (collection["Swaps"].ToString() == "on")
                {
                    ViewBag.Swaps = 1;

                    switch (SortOrder)
                    {
                        case "airline_desc":
                            baglist = baglist.OrderByDescending(x => x.DateSwapAdded).ThenByDescending(s => s.Airline).ToList();
                            ViewBag.NameSortParm = "airline";
                            ViewBag.YearSortParm = "year";
                            break;
                        case "year":
                            baglist = baglist.OrderByDescending(x => x.DateSwapAdded).ThenBy(s => s.Year).ToList();
                            ViewBag.NameSortParm = "airline";
                            ViewBag.YearSortParm = "year_desc";
                            break;
                        case "year_desc":
                            baglist = baglist.OrderByDescending(x => x.DateSwapAdded).ThenByDescending(s => s.Year).ToList();
                            ViewBag.NameSortParm = "airline";
                            ViewBag.YearSortParm = "year";
                            break;
                        case "airline":
                            baglist = baglist.OrderByDescending(x => x.DateSwapAdded).ThenBy(s => s.Airline).ToList();
                            ViewBag.NameSortParm = "airline_desc";
                            ViewBag.YearSortParm = "year";
                            break;
                        default:
                            baglist = null;
                            break;
                    }
                }
                else
                {
                    switch (SortOrder)
                    {
                        case "airline_desc":
                            baglist = baglist.OrderByDescending(s => s.Airline).ToList();
                            ViewBag.NameSortParm = "airline";
                            ViewBag.YearSortParm = "year";
                            break;
                        case "year":
                            baglist = baglist.OrderBy(s => s.Year).ToList();
                            ViewBag.NameSortParm = "airline";
                            ViewBag.YearSortParm = "year_desc";
                            break;
                        case "year_desc":
                            baglist = baglist.OrderByDescending(s => s.Year).ToList();
                            ViewBag.NameSortParm = "airline";
                            ViewBag.YearSortParm = "year";
                            break;
                        case "airline":
                            baglist = baglist.OrderBy(s => s.Airline).ToList();
                            ViewBag.NameSortParm = "airline_desc";
                            ViewBag.YearSortParm = "year";
                            break;
                        default:
                            baglist = null;
                            break;
                    }
                }
            }

            // Now we have to fetch only the page of interest.
            baglist = CreatePagination(baglist, WhichPage, PerPage, ref NumPages);

            ViewBag.SortOrder = SortOrder;
            ViewBag.WhichPage = WhichPage;
            ViewBag.NumPages = NumPages;
            ViewBag.PerPage = PerPage;

            return View(baglist);
        }

        // Post: Bags
        [HttpPost]
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public async Task<IActionResult> Index(int? whichpage, int? perpage, int? numpages, IFormCollection collection)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        {
            int WhichPage = whichpage ?? 1;
            int PerPage = perpage ?? 5;
            int NumPages = numpages ?? 0;
            string SortOrder = "airline";

            string airline = Request.Form["Airline"].ToString();
            string swaps = Request.Form["Swaps"];
            // THIS WILL WORK TOO!
            //string airline2 = collection["Airline"].ToString();
            //            IActionResult bags = await GetBags(collection);
            SortedCriteria = Request.Form;
            IActionResult bags = await GetBags(Request.Form);

            var okResult = bags as OkObjectResult;
            var baglist = okResult.Value as List<Bagsmvc>;
            if (collection["Swaps"].ToString() == "on")
            {
                ViewBag.Swaps = 1;
                baglist = baglist.OrderByDescending(s => s.DateSwapAdded).ThenBy(x => x.Airline).ToList();
            }
            else
            {
                ViewBag.Swaps = 0;
                baglist = baglist.OrderBy(s => s.Airline).ToList();
            }

            // Now we have to fetch only the page of interest.
            baglist = CreatePagination(baglist, WhichPage, PerPage, ref NumPages);

            ViewBag.SortOrder = SortOrder;
            ViewBag.WhichPage = WhichPage;
            ViewBag.NumPages = NumPages;
            ViewBag.PerPage = PerPage;

            ViewBag.NameSortParm = "airline_desc";
            ViewBag.YearSortParm = "year";
            return View(baglist);
        }

        // GET: Bags/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Bags/Create
        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Bags/Create
        [Authorize]
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
        [Authorize]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int? id, bool? copy)
        {
            Bagsmvc bag = new Bagsmvc();
            BagViewModel bvm = new BagViewModel();
            bvm.People = await _cache.GetFromTable(_context.People);
            bvm.People = bvm.People.OrderBy(x => x.FirstName).ToList();
            ViewBag.Operation = "Edit This";

            // First set up the bagtype dropdown

            // When id is not null, we are editing or copying.  Otherwise, we're creating 
            // EDIT a bag
            if(id != null)
            {
                var allbags = await _cache.GetFromTable(_context.Bags);
                bag = allbags.FirstOrDefault(x => x.Id == id);
            } else
            // CREATE a new blank bag
            {
                ViewBag.Operation = "Newly Created";
                _context.Bags.Add(bag);
                await _context.SaveChangesAsync(); ;
                await _cache.GetFromTable(true, _context.Bags);
            }

            // COPY a bag
            // This is a real hack.  Once the button is hit, the bag is copied and saved to the db.  Setting Id = 0 somehow
            // indicates to the Entity Framework to add the record in with a newly generated ID.  
            // Also, since we don't want an exact copy, bring user to edit screen to make the changes.
            if (copy != null)
            {
                ViewBag.Operation = "Recently Copied";
                bag.Id = 0;
                bag.Links = null;
                await _context.Bags.AddAsync(bag);
                await _context.SaveChangesAsync(); 
//                await _cache.AddItem(bag, _context.Bags);
                await _cache.GetFromTable(true, _context.Bags);
            }

            bvm.Bag = bag;
            bvm.TypeOfBag = await _cache.GetFromTable(_context.Types);

            return View(bvm);
        }

        // POST: Bags/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(BagViewModel bvm, IFormCollection collection)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Bags.Update(bvm.Bag);
                    await _context.SaveChangesAsync();
                    
                    // Refresh the cache
                    await _cache.GetFromTable(true, _context.Bags);

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
        [Authorize]
        [HttpGet]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Bags/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([Bind("Id")] int id, IFormCollection collection)
        {
            try
            {
                // Can this be done using the Links Navigation Property?
                // Don't just delete the bag, delete all the links to it as well
                // The Foreign Key should work, but doesn't reliably do so.  Who can tell why?
                var allbags = await _cache.GetFromTable(_context.Bags);
                var alllinks = await _cache.GetFromTable(_context.Links);
                var bag = allbags.FirstOrDefault(x => x.Id == id);
                var links = alllinks.Where(x => x.BagId == id).ToList();

                // Remove all the links
                links.ForEach(x => _context.Links.Remove(x));

                // Remove the bag
                _context.Bags.Remove(bag);
                await _context.SaveChangesAsync();

                // Refresh the cache
                await _cache.GetFromTable(true, _context.Bags);
                await _cache.GetFromTable(true, _context.Links);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                string msg = "Fatal Error: You cannot delete a bag without first deleting its links";
                return RedirectToAction(nameof(Errors), "Bags", new { message = msg } );
            }
        }

        // GET: Bags/Swaps5
        public ActionResult Swaps()
        {
            return RedirectToAction("Index", new { Swaps = "on" });
        }

        // GET: Bags/Error
        public ActionResult Errors(string message)
        {
            return View(message);
        }


    }
}