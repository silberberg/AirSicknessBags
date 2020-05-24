using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AirSicknessBags.Models;
using static AirSicknessBags.Models.BagContext;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography.X509Certificates;

namespace AirSicknessBags.Controllers
{
    public class PeopleController : Controller
    {
        private readonly ILogger<PeopleController> _logger;
        private IGenericCacheService _cache;
        private readonly BagContext _context;

        public static IFormCollection FormData;
        //public static int WhichPage = 1;
        //public static int PerPage = 5;
        //public static int NumPages = 0;

        public PeopleController(ILogger<PeopleController> logger, IGenericCacheService cache, BagContext context)
        {
            _logger = logger;
            _cache = cache;
            _context = context;
        }

        // List of pertinent countries.  Only want to get these once
        // public static List<Country> Allcountries { get; set; } = null;
        public static List<Country> Allpertinentcountries { get; set; } = null;

        // Get all the pertinent countries, hopefully just once
        public async Task<List<Country>> GetAllPertinentCountries()
        {
            var allcountries = await _cache.GetFromTable(_context.Countries);
            var allpeople = await _cache.GetFromTable(_context.People);

            if (Allpertinentcountries == null)
            {
                Allpertinentcountries = new List<Country>();
                foreach (var c in allcountries)
                {
                    if (allpeople.Any(x => x.IsoCountry == c.Iso))
                    {
                        Allpertinentcountries.Add(c);
                    }
                }
            }

            return (Allpertinentcountries);
        }

        // List of people.  Only want to get these once
        public static List<Peoplemvc> Allpeople { get; set; } = null;
        public virtual List<Peoplemvc> AllPeople => Allpeople;

        // Get all the people, hopefully just once
        public async Task<List<Peoplemvc>> GetAllPeople()
        {
            if (Allpeople == null)
            {
                Allpeople = await _context.People.ToListAsync();
            }

            return (Allpeople);
        }

        /* This will be called by both httpget and httppost of the index action */
        public async Task<List<Peoplemvc>> GetPeople()
        {
            // Start with a new list of people
            List<Peoplemvc> peoplemvc = new List<Peoplemvc>();
            List<Linksmvccore> links = new List<Linksmvccore>();

            // Get all people from the cache
            var allpeople = await _cache.GetFromTable(_context.People);
            //            _context.Entry<Peoplemvc>(allpeople[5]).Collection(b => b.Links).Load();
            
            // List<Peoplemvc> ap = await _context.People.Include(x => x.Links).ToListAsync();

            // Now we have to populate the stupid navigation property: ICollection<Linksmvccore> Links
            //foreach (var person in allpeople)
            //{
            //    var alllinks = await _cache.GetFromTable(_context.Links);
            //    links = alllinks.ToList();
            //    links.RemoveAll(x => x.PersonId != person.PersonNumber);
            //    if (links.Count > 0)
            //    {
            //        person.Links = links;
            //    }
            //}
            //var peoplewithlinks = allpeople.Where(x => alllinks.Any(y => x.PersonNumber == y.PersonId));

            // Here's a placeholder person for when nobody fits search criteria
            Peoplemvc emptyperson = new Peoplemvc
            {
                FirstName = "No Matching Records Found"
            };

            // Keep track of whether someone wants everyone or no-one.
            bool EmptyRequest = true;
            bool FullRequest = FormData != null && FormData["AllPeople"] == "on" ? true : false;

            // If somethine was selected, we no longer have an empty query
            if  ((FormData != null) &&
                (FormData["HasWebsite"] == "on" ||
                FormData["PersonName"] != "" ||
                FormData["Detail"] != "" ||
                FormData["Country"] != "" ||
                FormData["AllPeople"] == "on" ||
                FormData["Collector"] == "on" ||
                FormData["Donor"] == "on" ||
                FormData["Swapper"] == "on" ||
                FormData["Seller"] == "on" ||
                FormData["StarterKit"] == "on"
                ))
            {
                EmptyRequest = false;
            }

            // If nobody matches the criteria, return a blank informative message, otherwise return a list sorted on last name
            if (EmptyRequest)
            {
                peoplemvc.Add(emptyperson);
            }
            else
            {
                // Sort everyone by lastname.  No reason to sort any other way for people
                peoplemvc = allpeople.OrderBy(x => x.LastName).ToList();
            }

            // Not Full Request, so filter people records
            if (!FullRequest && FormData != null)
            {
                if (FormData["HasWebsite"] == "on")
                {
                    peoplemvc = peoplemvc.FindAll(x => x.PrimarySite != null && x.PrimarySite != "").ToList();
                    //                    peoplemvc = peoplemvc.Where(x => x.PrimarySite != null).ToList();
                }

                if (FormData["Collector"] == "on")
                {
                    peoplemvc = peoplemvc.FindAll(x => x.Collector == 1).ToList();
                }

                if (FormData["Donor"] == "on")
                {
                    peoplemvc = peoplemvc.FindAll(x => x.Donor == 1).ToList();
                }

                if (FormData["Swapper"] == "on")
                {
                    peoplemvc = peoplemvc.FindAll(x => x.Swapper == 1).ToList();
                }

                if (FormData["Seller"] == "on")
                {
                    peoplemvc = peoplemvc.FindAll(x => x.Seller == 1).ToList();
                }

                if (FormData["StarterKit"] == "on")
                {
                    peoplemvc = peoplemvc.FindAll(x => x.StarterKit == 1).ToList();
                }

                if (FormData["Country"] != "")
                {
                    peoplemvc = peoplemvc.FindAll(x => x.IsoCountry == FormData["Country"]).ToList();
                }

                // Search for a name match
                if (FormData["PersonName"] != "")
                {
                    peoplemvc = peoplemvc
                   .Where(x =>
                        (x.FirstName != null && x.FirstName.Contains(FormData["PersonName"], StringComparison.OrdinalIgnoreCase)) ||
                        (x.MiddleName != null && x.MiddleName.Contains(FormData["PersonName"], StringComparison.OrdinalIgnoreCase)) ||
                        (x.LastName != null && x.LastName.Contains(FormData["PersonName"], StringComparison.OrdinalIgnoreCase))
                        ).ToList();
                }

                // Search for a detail match
                if (FormData["Detail"] != "")
                {
                    peoplemvc = peoplemvc
                        .Where(x => x.Comments != null && x.Comments.Contains(FormData["Detail"], StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
            }


            return peoplemvc;
        }

        // GET: People
        [HttpGet]
        public async Task<IActionResult> Index(int? whichpage, int? perpage, int? numpages)
        {
            PeopleViewModel pvm = new PeopleViewModel();

             
            int WhichPage = whichpage ?? 1;
            int NumPages = numpages ?? 0;
            int PerPage = perpage ?? 5;

            pvm.Countries = await GetAllPertinentCountries();
            pvm.People = await GetPeople();
            pvm.People = CreatePagination(pvm.People, WhichPage, PerPage, ref NumPages);
            ViewBag.WhichPage = WhichPage;
            ViewBag.NumPages = NumPages;
            ViewBag.PerPage = PerPage;

            // Start out with blank page
            return View(pvm);
        }

        //private List<Peoplemvc> CreatePagination(List<Peoplemvc> people)
        //{
        //    if (people != null)
        //    {
        //        double stupid = Convert.ToDouble(people.Count) / PerPage;
        //        NumPages = Convert.ToInt32(Math.Ceiling(stupid));
        //        ViewBag.NumPages = NumPages;
        //        return (people.GetRange((WhichPage - 1) * PerPage, Math.Min(PerPage, people.Count - ((WhichPage - 1) * PerPage))));
        //    } else
        //    {
        //        return (null);
        //    }
        //}

        //public List<T> CreatePagination<T>(List<T> items, int whichpage, int perpage, ref int numpages)
        //{
        //    if (items != null)
        //    {
        //        double stupid = Convert.ToDouble(items.Count) / perpage;
        //        numpages = Convert.ToInt32(Math.Ceiling(stupid));
        //        return (items.GetRange((whichpage - 1) * perpage, Math.Min(perpage, items.Count - ((whichpage - 1) * perpage))));
        //    }
        //    else
        //    {
        //        return (null);
        //    }
        //}

        // POST: People
        [HttpPost]
        public async Task<IActionResult> Index(int? id)
        {
            // Save the request for pagination
            if (Request.Form != null)
            {
                FormData = Request.Form;
            }

            int WhichPage = 1;
            int NumPages = 0;
            int PerPage = 5;

            // View Model will have both people and country list
            PeopleViewModel pvm = new PeopleViewModel();

            // Get the country list
            pvm.Countries = await GetAllPertinentCountries();

            // Apply filters and put results into the list
            List<Peoplemvc> peoplemvc = await GetPeople();

            // Now we have to fetch only the page of interest.
            peoplemvc = CreatePagination(peoplemvc, WhichPage, PerPage, ref NumPages);
            ViewBag.WhichPage = WhichPage;
            ViewBag.NumPages = NumPages;
            ViewBag.PerPage = PerPage;

            // Put it in View MOdel
            pvm.People = peoplemvc;

            return View(pvm);
        }

        // GET: People/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            // Get all people
            var allpeople = await _cache.GetFromTable(_context.People);

            Peoplemvc person = new Peoplemvc();

            if (id == null)
            {
                return NotFound();
            } else
            {
                person = allpeople.FirstOrDefault(m => m.PersonNumber == id);
            }

            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // GET: People/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PersonNumber,PrimarySiteName,SecondarySiteName,PrimarySite,SecondarySite,FirstName,MiddleName,LastName,Country,IsoCountry,PrimaryEmail,SecondaryEmail,TertiaryEmail,Collector,Donor,Swapper,Seller,StarterKit,Comments")] Peoplemvc peoplemvc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(peoplemvc);
                await _context.SaveChangesAsync();
                //await _cache.AddItem(peoplemvc, _context.People);
                await _cache.GetFromTable(true, _context.People);
                return RedirectToAction(nameof(Index));
            }
            return View(peoplemvc);
        }

        // GET: People/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get all people
            var allpeople = await _cache.GetFromTable(_context.People);

            var peoplemvc = allpeople.FirstOrDefault(x => x.PersonNumber == id);
            if (peoplemvc == null)
            {
                return NotFound();
            }
            return View(peoplemvc);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PersonNumber,PrimarySiteName,SecondarySiteName,PrimarySite,SecondarySite,FirstName,MiddleName,LastName,Country,IsoCountry,PrimaryEmail,SecondaryEmail,TertiaryEmail,Collector,Donor,Swapper,Seller,StarterKit,Comments")] Peoplemvc peoplemvc)
        {
            if (id != peoplemvc.PersonNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(peoplemvc);
                    await _context.SaveChangesAsync();

                    // Get refreshed people cache
                    await _cache.GetFromTable(true, _context.People);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PeoplemvcExists(peoplemvc.PersonNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(peoplemvc);
        }

        // GET: People/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get all people
            var allpeople = await _cache.GetFromTable(_context.People);

            var peoplemvc = allpeople.FirstOrDefault(m => m.PersonNumber == id);
            if (peoplemvc == null)
            {
                return NotFound();
            }

            return View(peoplemvc);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Get all people
            var allpeople = await _cache.GetFromTable(_context.People);

            var peoplemvc = allpeople.FirstOrDefault(m => m.PersonNumber == id);
            _context.People.Remove(peoplemvc);
            await _context.SaveChangesAsync();

            // Refresh the people cache
            await _cache.GetFromTable(true, _context.People);

            return RedirectToAction(nameof(Index));
        }

        private bool PeoplemvcExists(int id)
        {
            return _context.People.Any(e => e.PersonNumber == id);
        }
    }
}
