using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PhonebookApp.Areas.Identity.Data;
using PhonebookApp.Models;
using System.Diagnostics;

namespace PhonebookApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<PhonebookUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<PhonebookUser> userManager)
        {
            _logger = logger;
            this._userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Contacts");
            }

            var user = await _userManager.GetUserAsync(this.User);
            if (user != null)
            {
                ViewBag.UserEmail = user.Email;
                ViewBag.Id = user.Id;
            }
            return View();
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
