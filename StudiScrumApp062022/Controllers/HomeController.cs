using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScrumApp.Models;
using SrumApp.Repository.UserRepository;
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
        private readonly AuthRepository _authRepository;
        private readonly UserRepository _userRepository;
        public HomeController(ILogger<HomeController> logger, AuthRepository authRepository, UserRepository userRepository)
        {
            _logger = logger;
            _authRepository = authRepository;
            _userRepository = userRepository;
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
        public async Task<IActionResult> Login(LoginModel userInput)
        {
            var user = _userRepository.GetUser(userInput.Login);

            if (user == null)
            {
                ModelState.AddModelError(nameof(LoginModel.Login), "Le compte n'existe pas");
            }
            else
            {
                var isGoodPwd = Helpers.PasswordValidation(user.Password, user.PasswordKey, userInput.Password);

                //Checker la connexion en BDD au lieu de le faire en dur
                if (isGoodPwd)//Si OK
                {
                    var claim = new List<Claim>();
                    claim.Add(new Claim(ClaimTypes.Surname, user.Surname));
                    claim.Add(new Claim(ClaimTypes.Name, user.Forename));
                    claim.Add(new Claim(ClaimTypes.Email, user.Mail));

                    var claimPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme));

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal);

                }
                else
                {
                    ModelState.AddModelError(nameof(LoginModel.Password), "Informations de connexion incorrectes");
                }
            }
            

            if (!ModelState.IsValid)
            {
                return View(userInput);
            }

            if (string.IsNullOrWhiteSpace(userInput.ReturnUrl))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return Redirect(userInput.ReturnUrl);
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
            if(userModel == null)
            {
                return StatusCode(400);
            }

            try
            {

                //Générer ma clé d'encryptage (passwordSalt)
                string passwordSalt = Helpers.PasswordSaltInBase64();

                //Hasher mon mdp avec ma clé d'encryptage
                string passwordHashed = Helpers.PasswordToHashBase64(userModel.Password, passwordSalt);

                //enregistrer en BDD
                _authRepository.SignUp(userModel, passwordSalt, passwordHashed);
            }
            catch(Exception e)
            {
                return StatusCode(500);
            }

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
