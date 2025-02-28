using Microsoft.AspNetCore.Mvc;
using Biblioteka.Models;
using Biblioteka.Data;
using System.Linq;

namespace Biblioteka.Controllers
{
    public class AccountController : Controller
    {
        private readonly BibliotekaContext _context;

        public AccountController(BibliotekaContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult InfoRej()
        {
            return View();
        }

        public string GetUserType()
        {
            return HttpContext.Session.GetString("UserType");
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string cardNumber, string password)
        {
            var reader = _context.Reader.FirstOrDefault(r => r.CardNumber == cardNumber && r.Password == password);
            if (reader != null)
            {
                HttpContext.Session.SetString("UserType", "Reader");
                HttpContext.Session.SetInt32("UserId", reader.Id);
                return RedirectToAction("Index", "Home");
            }

            var employee = _context.Employee.FirstOrDefault(e => e.CardNumber == cardNumber && e.Password == password);
            if (employee != null)
            {
                HttpContext.Session.SetString("UserType", "Employee");
                HttpContext.Session.SetInt32("UserId", employee.Id);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Niepoprawne dane logowania.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}