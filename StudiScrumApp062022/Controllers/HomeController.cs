using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScrumApp.Models;
using StudiScrumApp062022.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudiScrumApp062022.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login(string ReturnUrl)
        {
            var vm = new LoginModel()
            {
                ReturnUrl = ReturnUrl
            };
            return View("Login", vm);
        }

        /// <summary>
        /// Fonction de connexion, la méthode SignInAsync nous permet de nous connecter
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel user)
        {
            //Checker la connexion en BDD au lieu de le faire en dur
            if(user.Login == "Thomas" && user.Password == "BDCpwd")//Si OK
            {
                var claim = new List<Claim>();
                claim.Add(new Claim(ClaimTypes.Name, user.Login));

                var claimPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme));
                
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal);

            }
            else
            {
                ModelState.AddModelError(nameof(LoginModel.Password), "Informations de connexion incorrectes");
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            if (string.IsNullOrWhiteSpace(user.ReturnUrl))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return Redirect(user.ReturnUrl);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(UserModel userModel)
        {
            //faire l'inscription en BDD 

            if (!ModelState.IsValid)
            {
                return View(userModel);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
    }
}
