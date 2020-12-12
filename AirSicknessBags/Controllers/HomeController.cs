using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AirSicknessBags.Models;
using Microsoft.Extensions.Caching.Memory;

namespace AirSicknessBags.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
//        private ISimpleCacheService _cache;
        private IGenericCacheService _cache;
        private readonly BagContext _context;

        //        public HomeController(ILogger<HomeController> logger, ISimpleCacheService cache)
        public HomeController(ILogger<HomeController> logger, IGenericCacheService cache, BagContext context)
        {
            _logger = logger;
            _cache = cache;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var allbags = await _cache.GetFromTable(_context.Bags);
            var random = new Random();
            int index = random.Next(allbags.Count);
            ViewBag.Count = allbags.Count;

            return View(allbags[index]);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
