using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AirSicknessBags.Models
{

    public interface IGenericCacheService
    {
        string getSecret { get; }

        Task<List<T>> GetFromTable<T>(DbSet<T> TableToRead) where T : class;
        Task<List<T>> GetFromTable<T>(bool refresh, DbSet<T> TableToRead) where T : class;
        Task<List<T>> AddItem<T>(T AddedItem, DbSet<T> TableToRead) where T : class;
    }

    public class ComplexCacheService : IGenericCacheService
    {
        private readonly BagContext _context;
        private readonly ILogger _logger;
        private IMemoryCache _cache;

        private const int hours = 4;

        string IGenericCacheService.getSecret => "I#am#Bree";

        public ComplexCacheService(BagContext context,
            ILogger<SimpleCacheService> logger,
            IMemoryCache memoryCache)
        {
            _context = context;
            _logger = logger;
            _cache = memoryCache;
        }


        public async Task<List<T1>> GetFromTable<T1>(DbSet<T1> TableToRead) where T1 : class
        {
            return (await GetFromTable(false, TableToRead));
        }

        public async Task<List<T1>> GetFromTable<T1>(bool refresh, DbSet<T1> TableToRead) where T1 : class
        {
            string cacheKey = typeof(T1).Name;
            List<T1> results = new List<T1>();

            if (_cache.TryGetValue(cacheKey, out results) && !refresh)
            {
                _logger.LogInformation($"Data pulled from cache: {cacheKey}");
            }
            else
            {
                // First let's see if there are any foreign key navigation properties into another table (Links table)
                // We do this by seeing if there's an ICollection property that references the "many" table
                PropertyInfo property = typeof(T1).GetProperties()
                    .FirstOrDefault(x => x.PropertyType.Name.Contains("ICollection"));
                //List<PropertyInfo> properties = typeof(T1).GetProperties()
                //    .Where(x => x.PropertyType.Name.Contains("ICollection")).ToList();

                // If no Navigation Property is found, just read the table.  Otherwise read the table AND the related table
                if (property == null)
                {
                    results = await TableToRead.ToListAsync() as List<T1>;
                } else
                {
                    if (cacheKey == "Bagsmvc")
                    {
                        results = await TableToRead.Include(property.Name).Include("Person").ToListAsync() as List<T1>;
                    }
                    else
                    {
                        results = await TableToRead.Include(property.Name).ToListAsync() as List<T1>;
                    }
                    //results = await TableToRead.Include("Person").ToListAsync() as List<T1>;
                }

                // Configure the cache
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(hours));
                _cache.Set(cacheKey, results, cacheEntryOptions);
                _logger.LogInformation($"Data pulled from the Database: {cacheKey}");
            }

            return results;
        }

        // Deprecated. Never really worked.  I had hoped to manually update the cache without going back to the server
        // to read the entire table again
        public async Task<List<T>> AddItem<T>(T AddedItem, DbSet<T> TableToRead) where T : class
        {
            string cacheKey = typeof(T).Name;
            List<T> results = new List<T>();

            if (_cache.TryGetValue(cacheKey, out results))
            {
                // The item is still in memory, so add item to it
                results.Add(AddedItem);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(hours));
                _cache.Set(cacheKey, results, cacheEntryOptions);
            }
            else
            {
                // Item no longer in memory, so refresh the whole cache
                results = await GetFromTable(true, TableToRead);
            }
            
            // No need really, except to satisfy async can't be void
            return (results);
        }
    }

    public interface ISimpleCacheService
    {
        Task<List<Bagsmvc>> GetBags();
        Task<List<Bagsmvc>> GetBags(bool refreshCache);
    }

    public class SimpleCacheService : ISimpleCacheService
    {
        private readonly BagContext _context;
        private readonly ILogger _logger;
        private IMemoryCache _cache;

        private const int hours = 4;

        public SimpleCacheService(BagContext context,
            ILogger<SimpleCacheService> logger,
            IMemoryCache memoryCache)
        {
            _context = context;
            _logger = logger;
            _cache = memoryCache;
        }


        public async Task<List<Bagsmvc>> GetBags()
        {
            return await GetBags(false);
        }

        public async Task<List<Bagsmvc>> GetBags(bool refreshCache)
        {
            string cacheKey = nameof(SimpleCacheService.GetBags);
            List<Bagsmvc> results = new List<Bagsmvc>();

            if (_cache.TryGetValue(cacheKey, out results) && !refreshCache)
            {
                _logger.LogInformation($"Data pulled from cache: {cacheKey}");
            }
            else
            {
                results = await _context.Bags.ToListAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(hours));
                _cache.Set(cacheKey, results, cacheEntryOptions);
                _logger.LogInformation($"Data pulled from the Database: {cacheKey}");
            }

            return results;
        }
    }


}
