using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SampleShopApp.Helpers;
using SampleShopApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace SampleShopApp.Controllers
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
        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult Products()
        {
            var sessionUser = UserService.GetNameFromUsersession(User);

            var user = UserService.GetUser(sessionUser, HttpContext);

            ViewData["sessionUser"] = sessionUser;

            return View();
        }
       
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
