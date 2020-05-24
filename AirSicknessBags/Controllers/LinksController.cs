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
            
            //            await Tables.GetLinks(_context);
            //            alvm.Links = Tables._alllinks;
            //await Tables.GetPeople(_context);
            //alvm.People = Tables._allpeople;
            
            return View(alvm);
        }

        // GET: Links/Details/5
        // Display all bags for a particular person
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dude = await _cache.GetFromTable(_context.People) as List<Peoplemvc>;
            // Instantiate patron with the person the user clicked as well as a blank list of bags
            Patron patron = new Patron
            {
                //                Person = await _context.People.FindAsync(id),
                Person = dude.FirstOrDefault(x => x.PersonNumber == id),
                Bags = new List<Bagsmvc>()
            };

            // Find all links for this person
            //            var links = await _context.Links.Where(x => x.PersonId == id).ToListAsync();
            var alllinks = await _cache.GetFromTable(_context.Links);
            var links = alllinks.Where(x => x.PersonId == id).ToList();

            // Get the full list of bags
            var allbags = await _cache.GetFromTable(_context.Bags);
            //await Tables.GetBags(_context);
            //var allbags = Tables._allbags;
            // var bags = await _context.Bags.ToListAsync();

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
                    }
                }
            }

            return View(patron);
        }

        // GET: Links/Create
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
            //            await Tables.GetBags(_context);
            //            lvm.Bag = Tables._allbags.FirstOrDefault(x => x.Id == id);
            //            lvm.Bag = await _context.Bags.FirstOrDefaultAsync(x => x.Id == id);

            // Get the full list of people
            lvm.People = await _cache.GetFromTable(_context.People);
            //await Tables.GetPeople(_context);
            //lvm.People = Tables._allpeople;

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LinkNumber,BagId,PersonId")] Linksmvccore linksmvccore)
        {
            if (ModelState.IsValid)
            {
                // Need to get links in order to update the list in memory
                var alllinks = await _cache.GetFromTable(_context.Links);
                // await Tables.GetLinks(_context);
                // await Tables.GetFromTable(_context.Links, _context);

                // This is a live database update
                _context.Links.Add(linksmvccore);
                await _context.SaveChangesAsync();

                // Have to refresh the cache
                await _cache.GetFromTable(true, _context.Links);

                // Have to add this link to the cache
                // await _cache.AddItem(linksmvccore, _context.Links);
                // Linksmvccore createdlink = UpdateZeroLinkNumber(linksmvccore, alllinks);
                // await _cache.AddItem(createdlink, _context.Links);
                // Tables._alllinks.Add(createdlink); 

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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alllinks = await _cache.GetFromTable(_context.Links);
            var linksmvccore = alllinks.Find(x => x.BagId == id);
            // var linksmvccore = await _context.Links.FindAsync(id);
            // var linksmvccore =  Tables._alllinks.Find(x => x.BagId == id);

            if (linksmvccore == null)
            {
                return NotFound();
            }
            return View(linksmvccore);
        }

        // POST: Links/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LinkNumber,BagId,PersonId")] Linksmvccore linksmvccore)
        {
            if (id != linksmvccore.LinkNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(linksmvccore);
                    await _context.SaveChangesAsync();

                    // I have no idea how to update the cache, so the hell with it, just refresh it completely.
                    await _cache.GetFromTable(true, _context.Links);

                    //Tables._alllinks.FindAll(x => x.LinkNumber == id)
                    //    .ForEach(y => {
                    //        y.BagId = linksmvccore.BagId;
                    //        y.PersonId = linksmvccore.PersonId;
                    //        });

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LinksmvccoreExists(linksmvccore.LinkNumber))
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
            return View(linksmvccore);
        }

        // GET: Links/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get all the links and find the one to delete
            var alllinks = await _cache.GetFromTable(_context.Links);
            var linksmvccore = alllinks.FirstOrDefault(m => m.LinkNumber == id);

            if (linksmvccore == null)
            {
                return NotFound();
            }

            return View(linksmvccore);
        }

        // POST: Links/Delete/5
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

            //Tables._alllinks.Remove(linksmvccore);
            return RedirectToAction(nameof(Index));
        }

        private bool LinksmvccoreExists(int id)
        {
            // Doesn't use cache but this is only called in a catch condition
            return _context.Links.Any(e => e.LinkNumber == id);
        }
    }
}
