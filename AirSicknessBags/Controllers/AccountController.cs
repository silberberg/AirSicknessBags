using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirSicknessBags.Models;
using AirSicknessBags.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AirSicknessBags.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private IGenericCacheService _cache;
        private readonly BagContext _context;

        public AccountController(UserManager<IdentityUser> userManager,
                                SignInManager<IdentityUser> signInManager,
                                IGenericCacheService cache, BagContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _cache = cache;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel rvm)
        {
            if (rvm.Secret == _cache.getSecret)
            {
                if (ModelState.IsValid)
                {
                    //var usertodelete = await userManager.FindByNameAsync("duh@ghilky.com");
                    //var res = await userManager.DeleteAsync(usertodelete);

                    var user = new IdentityUser
                    {
                        UserName = rvm.Email,
                        Email = rvm.Email
                    };
                    var result = await userManager.CreateAsync(user, rvm.Password);


                    if (result.Succeeded)
                    {
                        await signInManager.SignInAsync(user, isPersistent: rvm.Persistent);
                        return RedirectToAction("Index", "Home");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            } else
            {
                ModelState.AddModelError("Secret", "Secret Word Incorrect");
            }
            return View(rvm);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel lvm)
        {
            if (ModelState.IsValid)
                {
                var result = await signInManager.PasswordSignInAsync(lvm.Email, lvm.Password, lvm.RememberMe, false);


                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                }
            return View(lvm);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }


    }
}