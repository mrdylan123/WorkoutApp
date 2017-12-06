using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WorkoutApp.Config;
using WorkoutApp.Models;

namespace WorkoutApp.Controllers
{
    public class HomeController : Controller
    {
        private IUserRepository repo;

        public HomeController(IUserRepository repo)
        {
            this.repo = repo;
        }

        // GET: Home
        public ActionResult Index(string error)
        {
            return View();
        }

        public ActionResult Page1(string error)
        {
            if (error != null)
            {
                ViewBag.errorPage1 = error;
            }
            return View();
        }

        public ActionResult AllUsers()
        {
            IEnumerable<User> users = repo.GetUsers();
            List<User> userslist = users.ToList();
            ViewBag.userstotal = userslist.Count;
            ViewBag.userslist = userslist;
            return View("AllUsers");
        }

    }
}