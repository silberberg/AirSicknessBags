using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AirSicknessBags.Models;
using static AirSicknessBags.Models.BagContext;

namespace AirSicknessBags.Controllers
{
    public class PeopleController : Controller
    {
        private readonly BagContext _context;

        public PeopleController(BagContext context)
        {
            _context = context;
        }

        // List of countries.  Only want to get these once
        public static List<Country> countries { get; set; } = null;

        // Get all the countries, hopefully just once
        public async Task<List<Country>> GetAllCountries()
        {
            if (countries == null)
            {
                countries = await _context.Countries.ToListAsync();
            }

            return (countries);
        }

        // GET: People
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            PeopleViewModel pvm = new PeopleViewModel();

            await GetAllCountries();
            pvm.Countries = countries;

            // Start out with blank page
            return View(pvm);
        }

        // POST: People
        [HttpPost]
        public async Task<IActionResult> Index(int? id)
        {
            Peoplemvc person = new Peoplemvc();
            person.FirstName = "No Matching Records Found";
            List<Peoplemvc> peoplemvc = new List<Peoplemvc>();
            PeopleViewModel pvm = new PeopleViewModel();
            bool EmptyRequest = true;
            bool FullRequest = Request.Form["AllPeople"] == "on" ? true : false;

            await GetAllCountries();
            pvm.Countries = countries;

            if (Request.Form["HasWebsite"] == "on" ||
                Request.Form["PersonName"] != "" ||
                Request.Form["Detail"] != "" ||
                Request.Form["Country"] != "" ||
                Request.Form["AllPeople"] == "on" ||
                Request.Form["Collector"] == "on" ||
                Request.Form["Donor"] == "on" ||
                Request.Form["Swapper"] == "on" ||
                Request.Form["Seller"] == "on" ||
                Request.Form["StarterKit"] == "on" 
                )
            {
                EmptyRequest = false;
            }

            if (EmptyRequest)
            {
                peoplemvc.Add(person);
            }
            else
            {
                peoplemvc = await _context.People.OrderBy(x => x.LastName).ToListAsync();
//                peoplemvc = await _context.People.ToListAsync();
            }

            if (!FullRequest) { 
                if (Request.Form["HasWebsite"] == "on")
                {
                    peoplemvc = peoplemvc.FindAll(x => x.PrimarySite != null && x.PrimarySite != "").ToList();
//                    peoplemvc = peoplemvc.Where(x => x.PrimarySite != null).ToList();
                }

                if (Request.Form["Collector"] == "on")
                {
                    peoplemvc = peoplemvc.FindAll(x => x.Collector == 1).ToList();
                }

                if (Request.Form["Donor"] == "on")
                {
                    peoplemvc = peoplemvc.FindAll(x => x.Donor == 1).ToList();
                }

                if (Request.Form["Swapper"] == "on")
                {
                    peoplemvc = peoplemvc.FindAll(x => x.Swapper == 1).ToList();
                }

                if (Request.Form["Seller"] == "on")
                {
                    peoplemvc = peoplemvc.FindAll(x => x.Seller == 1).ToList();
                }

                if (Request.Form["StarterKit"] == "on")
                {
                    peoplemvc = peoplemvc.FindAll(x => x.StarterKit == 1).ToList();
                }

                if (Request.Form["Country"] != "")
                {
                    peoplemvc = peoplemvc.FindAll(x => x.IsoCountry == Request.Form["Country"]).ToList();
                }

                // Search for a name match
                if (Request.Form["PersonName"] != "")
                {
                    peoplemvc = peoplemvc
                   .Where(x =>
                        (x.FirstName != null && x.FirstName.Contains(Request.Form["PersonName"], StringComparison.OrdinalIgnoreCase)) ||
                        (x.MiddleName != null && x.MiddleName.Contains(Request.Form["PersonName"], StringComparison.OrdinalIgnoreCase)) ||
                        (x.LastName != null && x.LastName.Contains(Request.Form["PersonName"], StringComparison.OrdinalIgnoreCase))
                        ).ToList();
                }

                // Search for a detail match
                if (Request.Form["Detail"] != "")
                {
                    peoplemvc = peoplemvc
                        .Where(x => x.Comments != null && x.Comments.Contains(Request.Form["Detail"], StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
            }

            pvm.People = peoplemvc;

            return View(pvm);
        }

        // GET: People/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            List<Peoplemvc> peoplemvc = null;
            Peoplemvc person = new Peoplemvc();

            if (id == null)
            {
                return NotFound();
            } else
            {
                person = await _context.People
                    .FirstOrDefaultAsync(m => m.PersonNumber == id);
                List<Peoplemvc> list = new List<Peoplemvc>();
                list.Add(person);
                peoplemvc = list;
            }

            if (peoplemvc == null)
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

            var peoplemvc = await _context.People.FindAsync(id);
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

            var peoplemvc = await _context.People
                .FirstOrDefaultAsync(m => m.PersonNumber == id);
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
            var peoplemvc = await _context.People.FindAsync(id);
            _context.People.Remove(peoplemvc);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PeoplemvcExists(int id)
        {
            return _context.People.Any(e => e.PersonNumber == id);
        }
    }
}
