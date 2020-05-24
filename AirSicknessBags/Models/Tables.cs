using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirSicknessBags.Models
{
    public sealed class Tables  // Sealed so nothing can inherit this class
    {
        // You need to create a private static variable that is going to hold a reference to the single created instance of the class if any.
        private static Tables instance;

        private Tables()
        {
            //You need to declare a constructor that should be private and parameterless.This is required because it is not allowed 
            //the class to be instantiated from outside the class. It only instantiates from within the class.
        }

        // Tables to read.  Only want to get these once
        public static List<Linksmvccore> _alllinks { get; set; } = null;
        public static List<Peoplemvc> _allpeople { get; set; } = null;
        public static List<Bagsmvc> _allbags { get; set; } = null;
        public static List<Country> _allcountries { get; set; } = null;
        public static List<Country> _allpertinentcountries { get; set; } = null;


        //You also need to create a public static property/method which will return the single created instance of the singleton class. 
        //This method or property first check if an instance of the singleton class is available or not.If the singleton instance is available, 
        //    then it returns that singleton instance otherwise it will create an instance and then return that instance.

        public static async Task<Tables> GetFromTable<T>(DbSet<T> TableToRead, BagContext _context) where T : class
        {
            if (instance == null)
            {
                instance = new Tables();
            }

            if (_alllinks == null)
            {
                 var x = await TableToRead.ToListAsync();
                _alllinks = x as List<Linksmvccore>;
                //                _alllinks = await _context.Links.ToListAsync();
            }

            return instance;
        }

        // Get all the links, hopefully just once
        public static async Task<Tables> GetLinks(BagContext _context)
        {
            if (instance == null)
            {
                instance = new Tables();
            }

            if (_alllinks == null)
            {
                _alllinks = await _context.Links.ToListAsync();
            }

            return instance;
        }

        // Get all the people, hopefully just once
        public static async Task<Tables> GetPeople(BagContext _context)
        {
            if (instance == null)
            {
                instance = new Tables();
            }

            if (_allpeople == null)
            {
                _allpeople = await _context.People.ToListAsync();
            }

            return instance;
        }

        // Get all the bags, hopefully just once
        public static async Task<Tables> GetBags(BagContext _context)
        {
            if (instance == null)
            {
                instance = new Tables();
            }

            if (_allbags == null)
            {
                _allbags = await _context.Bags.ToListAsync();
            }

            return instance;
        }

        // Get all the countries, hopefully just once
        public static async Task<Tables> GetCountries(BagContext _context)
        {
            if (instance == null)
            {
                instance = new Tables();
            }

            if (_allcountries == null)
            {
                _allcountries = await _context.Countries.ToListAsync();
            }

            return instance;
        }

        // Get all the countries, hopefully just once
        public static async Task<Tables> GetPertinentCountries(BagContext _context)
        {
            if (instance == null)
            {
                instance = new Tables();
            }

            if (_allpertinentcountries == null)
            {
                if (_allcountries == null)
                {
                    await GetCountries(_context);
                }

                foreach (var c in _allcountries)
                {
                    if (_allpeople.Any(x => x.IsoCountry == c.Iso))
                    {
                        _allpertinentcountries.Add(c);
                    }
                }

            }

            return instance;
        }

    }
}
