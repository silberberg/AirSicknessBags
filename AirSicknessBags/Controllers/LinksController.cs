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
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using AirSicknessBags.ViewModels;

namespace AirSicknessBags.Controllers
{
    public class LinksController : Controller
    {
        private readonly ILogger<LinksController> _logger;
        private IGenericCacheService _cache;
        private readonly BagContext _context;

        public LinksController(ILogger<LinksController> logger, IGenericCacheService cache, BagContext context)
        {
            _logger = logger;
            _cache = cache;
            _context = context;
        }

        public List<SelectListItem> Options { get; set; }

        // GET: Links
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            AllLinkViewModel alvm = new AllLinkViewModel();

            alvm.Links = await _cache.GetFromTable(_context.Links);
            alvm.People = await _cache.GetFromTable(_context.People);
            alvm.Bags = await _cache.GetFromTable(_context.Bags);

            alvm.Links = alvm.Links.OrderByDescending(x => x.LinkNumber).Take(10).ToList();
            return View(alvm);
        }

        // GET: Links/Details/5 - Display all bags for a particular person
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch all people and bags (like always)
            var people = await _cache.GetFromTable(_context.People) as List<Peoplemvc>;
            var allbags = await _cache.GetFromTable(_context.Bags);


            // A Patron instance contains a person and their bags
            // Instantiate patron with the person the user clicked as well as a blank list of bags
            Patron patron = new Patron
            {
                Person = people.FirstOrDefault(x => x.PersonNumber == id),
                Bags = new List<Bagsmvc>()
            };

            // Slightly faster way to find all links for the person using Navigation Properties
            var links = patron.Person.Links;
            // Find all links for this person -  THIS WORKS!!
            //var alllinks = await _cache.GetFromTable(_context.Links);
            //var links = alllinks.Where(x => x.PersonId == id).ToList();

            // When someone clicks "See this person's bags", but they haven't sent me any (such as Starter Kit people), put in dummy record
            if (links == null)
            {
                // Instantiate and initialize new dummy bag
                Bagsmvc bag = new Bagsmvc
                {
                    Airline = "No Bags Found For " + patron.Person.FirstName + ' ' + patron.Person.MiddleName + ' ' + patron.Person.LastName
                };
                patron.Bags.Add(bag);
            } else
            {
                foreach(var l in links)
                {
                    var x = allbags.Find(x => x.Id == l.BagId);
                    if (x != null)
                    {
                        patron.Bags.Add(x);
                    } else
                    {
                        throw new FieldAccessException  ("Link to non-existent bag, Link #" + l.LinkNumber);
                    }
                }
            }

            return View(patron);
        }

        // GET: Links/Create
        // In order to create a link, I am arbitrarily deciding that you must first select the bag and then match it up
        // to a person.  That's why a bag id must be passed to this method
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            // If someone goes directly to /Links/Create without an Bag ID
            if (id == null)
            {
                return (RedirectToAction("Index", "Home"));
            }

            LinkViewModel lvm = new LinkViewModel();

            // Load up the bag
            var allbags = await _cache.GetFromTable(_context.Bags);
            lvm.Bag = allbags.FirstOrDefault(x => x.Id == id);

            // Get the full list of people
            lvm.People = await _cache.GetFromTable(_context.People);

            // The link is for this bag
            lvm.Link = new Linksmvccore();
            lvm.Link.BagId = lvm.Bag.Id;

            // List all the people sorted by lastname
            lvm.Options = lvm.People.OrderBy(x => x.LastName).Select(a =>
                new SelectListItem
                {
                    Value = a.PersonNumber.ToString(),
                    Text = a.FirstName + ' ' + a.MiddleName + ' ' + a.LastName
                }).ToList();

            return View(lvm);
        }

        // POST: Links/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LinkNumber,BagId,PersonId")] Linksmvccore linksmvccore)
        {
            if (ModelState.IsValid)
            {
                // Need to get links in order to update the list in memory
                var alllinks = await _cache.GetFromTable(_context.Links);

                // Prepare the database update
                _context.Links.Add(linksmvccore);

                // Perform the update
                await _context.SaveChangesAsync();

                // Have to refresh the cache
                await _cache.GetFromTable(true, _context.Links);

                // Go back to the bag controller to display just this bag
                return RedirectToAction("Index", "Bags", new { id = linksmvccore.BagId });
            }
            return View(linksmvccore);
        }

        private Linksmvccore UpdateZeroLinkNumber(Linksmvccore linksmvccore, List<Linksmvccore> alllinks)
        {
            Linksmvccore link = new Linksmvccore
            {
                BagId = linksmvccore.BagId,
                PersonId = linksmvccore.PersonId
            };

            link.LinkNumber = alllinks
                .Where(x => x.BagId == linksmvccore.BagId && x.PersonId == linksmvccore.PersonId)
                .ToList()
                .Max(z => z.LinkNumber);

            return (link);
        }

        // GET: Links/Edit/5
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alvm = new AllLinkViewModel();

            // Get all the links then find the one to edit
            alvm.Links = await _cache.GetFromTable(_context.Links);
            alvm.Link = alvm.Links.FirstOrDefault(x => x.LinkNumber == id);
            alvm.People = await _cache.GetFromTable(_context.People);
            alvm.Person = alvm.People.FirstOrDefault(x => x.PersonNumber == alvm.Link.PersonId);
            alvm.Bags = await _cache.GetFromTable(_context.Bags);
            alvm.Bag = alvm.Bags.FirstOrDefault(x => x.Id == alvm.Link.BagId);

            if (alvm.Link == null)
            {
                return NotFound();
            }
            return View(alvm);
        }

        // POST: Links/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Link")] AllLinkViewModel alvm)
        {
            if (id != alvm.Link.LinkNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alvm.Link);
                    await _context.SaveChangesAsync();

                    // I have no idea how to update the cache, so the hell with it, just refresh it completely.
                    await _cache.GetFromTable(true, _context.Links);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LinksmvccoreExists(alvm.Link.LinkNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw new Exception ("Could not save link #" + alvm.Link.LinkNumber + " but I have no idea why");
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            // This probably won't work because the binding omits people and bags.  Actually, the code shouldn't even get here
            return View(alvm);
        }

        // GET: Links/Delete/5
        // I suppose this might be useful, but there never seems to be a reason to delete a link
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alvm = new AllLinkViewModel();

            // Get all the links and find the one to delete
            alvm.Links  = await _cache.GetFromTable(_context.Links);
            alvm.Link = alvm.Links.FirstOrDefault(m => m.LinkNumber == id);

            // Find the Person and Bag this link links
            alvm.People = await _cache.GetFromTable(_context.People);
            alvm.Person = alvm.People.FirstOrDefault(x => x.PersonNumber == alvm.Link.PersonId);
            alvm.Bags = await _cache.GetFromTable(_context.Bags);
            alvm.Bag = alvm.Bags.FirstOrDefault(x => x.Id == alvm.Link.BagId);

            if (alvm.Link == null || alvm.Bag == null || alvm.Person == null)
            {
                return NotFound("Invalid link: Link #" + alvm.Link.LinkNumber + " Person:" + alvm.Person.PersonNumber +
                    " Bag:" + alvm.Bag.Id);
            }

            return View(alvm);
        }

        // POST: Links/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find the record to be deleted
            var alllinks = await _cache.GetFromTable(_context.Links);
            var linksmvccore = alllinks.Find(x => x.LinkNumber ==  id);

            // The actual DB deletion
            _context.Links.Remove(linksmvccore);
            await _context.SaveChangesAsync();

            // I have no idea how to delete an item from the cache, so the hell with it, just refresh it completely.
            await _cache.GetFromTable(true, _context.Links);

            return RedirectToAction(nameof(Index));
        }

        private bool LinksmvccoreExists(int id)
        {
            // Doesn't use cache but this is only called in a catch condition
            return _context.Links.Any(e => e.LinkNumber == id);
        }
    }
}
