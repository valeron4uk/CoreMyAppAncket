using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreMyAppAncket.Models;
using Microsoft.AspNetCore.Identity;

namespace CoreMyAppAncket.Controllers
{
    

    [RequireHttps]
    public class HomeController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(UserManager<ApplicationUser> manager)
        {
            _userManager = manager;
        }

        public IActionResult Index()
        {
            if (User.Identity.Name==null)
            {
                return View();
            }
            else
            {
                return RedirectToAction(nameof(Index),"Ancket");
            }
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
