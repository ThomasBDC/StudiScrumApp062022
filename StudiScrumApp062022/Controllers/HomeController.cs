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
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Threading;

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
                    claim.Add(new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()));

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

        public IActionResult ForgottenPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendEmailResetPassword(string email)
        {
            //Vérifier si l'utilisateur existe
            var user = _userRepository.GetUser(email);

            if(user == null)
            {
                return NotFound();
            }
            else
            {
                //Si il existe on démarre le processus
                var cleRecuperation = Guid.NewGuid().ToString(); 
                _authRepository.SetCleRecuperationForUser(user.IdUser, cleRecuperation);

                ////Envoyer le mail
                //MailMessage mail = new MailMessage();

                //mail.From = new MailAddress("studiscrumapp@gmail.com");
                //mail.To.Add(email);
                //mail.Subject = "Réinitialisation du mot de passe";

                //string link = "<a href='https://localhost:44319/Home/ResetPassword?p1="+cleRecuperation+"&p2="+user.IdUser+"'>ce lien</a>";
                
                //mail.Body = "Vous pouvez réinitialiser votre mdp via "+link;

                //SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                //smtpClient.Port = 587;
                //smtpClient.UseDefaultCredentials = true;
                //smtpClient.Credentials = new NetworkCredential("studithomasbdc@gmail.com", "azerty2022");
                //smtpClient.EnableSsl = true;
                //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                //smtpClient.Send(mail);

                return RedirectToAction("Login");
            }
        }

        [HttpGet("TestMail")]
        public async Task<IActionResult> SendMailTest()
        {
            try
            {
                string clientId = "63749115380-ueur2g9gj9b5ed87grb733mo6pv92m79.apps.googleusercontent.com";
                string clientSecret = "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";
                string fromMail = "studithomasbdc@gmail.com";
                string[] scopes = new string[] { "https://mail.google.com/" };
                ClientSecrets clientSecrets = new ClientSecrets()
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                };
                //Requesting authorization
                UserCredential userCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets, scopes, "user", CancellationToken.None).Result;
                //Authorization granted or not required (if the saved access token already available)
                if (userCredential.Token.IsExpired(userCredential.Flow.Clock))
                {
                    //The access token has expired, refreshing it
                    if (!userCredential.RefreshTokenAsync(CancellationToken.None).Result)
                    {
                        return StatusCode(500);
                    }
                }
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(fromMail));
                email.To.Add(MailboxAddress.Parse("thomasbdc@yopmail.com"));
                email.Subject = "Sending Email Using OAuth Net 6.0";
                email.Body = new TextPart(TextFormat.Html) { Text = "<h3>Mail Body</h3>" };
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    var oauth2 = new SaslMechanismOAuth2(fromMail, userCredential.Token.AccessToken);
                    client.Authenticate(oauth2);
                    await client.SendAsync(email);
                    client.Disconnect(true);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1">Clé de récupération</param>
        /// <param name="p2">Identifiant utilisateur</param>
        /// <returns></returns>
        public IActionResult ResetPassword(string p1, int p2)
        {
            return View();
        }
    }
}
