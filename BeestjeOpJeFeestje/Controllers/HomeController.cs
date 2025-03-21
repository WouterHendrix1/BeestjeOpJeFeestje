using System.Diagnostics;
using BeestjeOpJeFeestje.Models;
using Microsoft.AspNetCore.Mvc;

namespace BeestjeOpJeFeestje.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
           
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SelectDate(DateTime bookingDate)
        {
            if (bookingDate < DateTime.Today.AddDays(1))
            {
                TempData["ErrorMessage"] = "Je moet een datum in de toekomst kiezen.";
                return RedirectToAction("Index");
            }

            return RedirectToAction("SelectAnimals", "Bookings", new { date = bookingDate.ToString("yyyy-MM-dd") });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
